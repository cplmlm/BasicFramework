﻿using Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Common.Helper;

namespace Extensions
{
    /// <summary>
    /// JWT权限 认证服务
    /// </summary>
    public static class Authentication_JWTSetup
    {
        public static void AddAuthentication_JWTSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            var symmetricKeyAsBase64 = AppSecretConfig.Audience_Secret_String;
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var Issuer = AppSettings.app(new string[] { "Audience", "Issuer" });
            var Audience = AppSettings.app(new string[] { "Audience", "Audience" });
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            // 令牌验证参数
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = Issuer,//发行人
                ValidateAudience = true,
                ValidAudience = Audience,//订阅人
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30),
                RequireExpirationTime = true,
            };

            // 开启Bearer认证
            services.AddAuthentication(o =>
            {
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = nameof(ApiResponseHandler);
                o.DefaultForbidScheme = nameof(ApiResponseHandler);
            })
             // 添加JwtBearer服务
             .AddJwtBearer(o =>
             {
                 o.TokenValidationParameters = tokenValidationParameters;
                 o.Events = new JwtBearerEvents
                 {
                     OnMessageReceived = context =>
                     {
                         var accessToken = context.Request.Query["access_token"];
                         // If the request is for our hub...
                         var path = context.HttpContext.Request.Path;
                         if (!string.IsNullOrEmpty(accessToken) &&
                             (path.StartsWithSegments("/api2/chathub")))
                         {
                             // Read the token out of the query string
                             context.Token = accessToken;
                         }
                         return Task.CompletedTask;
                     },
                     OnChallenge = context =>
                     {
                         context.Response.Headers["Token-Error"] = context.ErrorDescription;
                         return Task.CompletedTask;
                     },
                     OnAuthenticationFailed = context =>
                     {
                         var jwtHandler = new JwtSecurityTokenHandler();
                         var token = context.Request.Headers["Authorization"].ObjToString().Replace("Bearer ", "");

                         if (token.IsNotEmptyOrNull() && jwtHandler.CanReadToken(token))
                         {
                             var jwtToken = jwtHandler.ReadJwtToken(token);

                             if (jwtToken.Issuer != Issuer)
                             {
                                 context.Response.Headers["Token-Error-Iss"] = "issuer is wrong!";
                             }

                             if (jwtToken.Audiences.FirstOrDefault() != Audience)
                             {
                                 context.Response.Headers["Token-Error-Aud"] = "Audience is wrong!";
                             }
                         }
                         // 如果过期，则把<是否过期>添加到，返回头信息中
                         if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                         {
                             context.Response.Headers["Token-Expired"] = "true";
                         }
                         return Task.CompletedTask;
                     }
                 };
             })
             .AddScheme<AuthenticationSchemeOptions, ApiResponseHandler>(nameof(ApiResponseHandler), o => { });
        }
    }
}
