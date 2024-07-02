using Autofac.Extensions.DependencyInjection;
using Autofac;
using BasicCommonProject.Filter;
using BasicCommonProject.Middle;
using BasicCommonProject.Middleware;
using Common.Helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using Extensions.ServiceExtensions;
using Extensions.AutoMapper;

var builder = WebApplication.CreateBuilder(args);
// 1、配置host与容器
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>(builder =>
            {
              builder.RegisterModule(new AutofacModuleRegister());
            });
// Add services to the container.
builder.Services.AddControllers();
//builder.Services.AddAutoMapperSetup();
builder.Services.AddAutoMapper(typeof(CustomProfile));
//builder.Services.AddControllers(options =>
//{
//    options.Filters.Add<ResultWrapperFilter>();
//    options.Filters.Add<GlobalExceptionFilter>();
//});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging(configure => configure
                .AddFilter("Microsoft", LogLevel.Warning)
               .SetMinimumLevel(LogLevel.Information)  // 设置日志级别
    );

var basePath = AppContext.BaseDirectory;
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JST API", Version = "v1" });
    #region 
    var basePath = AppContext.BaseDirectory;
    var xmlPath = Path.Combine(basePath, "BasicCommonProject.xml");
    c.IncludeXmlComments(xmlPath, true);
    #endregion

    #region Token绑定到ConfigureServices
    // 开启小锁
    c.OperationFilter<AddResponseHeadersFilter>();
    c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
    // 在header中添加token，传递到后台
    c.OperationFilter<SecurityRequirementsOperationFilter>();

    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
        Name = "Authorization",//jwt默认的参数名称
        In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
        Type = SecuritySchemeType.ApiKey
    });
    #endregion
});

var configuration = builder.Configuration;
#region 【简单授权】
#region 1、基于角色的API授权 

// 1【授权】、这个很简单，其他什么都不用做，
// 无需配置服务，只需要在API层的controller上边，增加特性即可，注意，只能是角色的:
// [Authorize(Roles = "Admin")]

// 2【认证】、然后在下边的configure里，配置中间件即可:app.UseMiddleware<JwtTokenAuth>();但是这个方法，无法验证过期时间，所以如果需要验证过期时间，还是需要下边的第三种方法，官方认证

#endregion

#region 2、基于策略的授权（简单版）

// 1【授权】、这个和上边的异曲同工，好处就是不用在controller中，写多个 roles 。
// 然后这么写 [Authorize(Policy = "Admin")]
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Client", policy => policy.RequireRole("Client").Build());
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin").Build());
    options.AddPolicy("SystemOrAdmin", policy => policy.RequireRole("Admin", "System"));
});


// 2【认证】、然后在下边的configure里，配置中间件即可:app.UseMiddleware<JwtTokenAuth>();但是这个方法，无法验证过期时间，所以如果需要验证过期时间，还是需要下边的第三种方法，官方认证
#endregion
#endregion
#region 【认证】
//读取配置文件
var symmetricKeyAsBase64 = configuration["Audience:Secret"];
var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
var signingKey = new SymmetricSecurityKey(keyByteArray);


//2.1【认证】
//builder.Services.AddAuthentication(x =>
//{
//    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
// .AddJwtBearer(o =>
// {
//     o.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuerSigningKey = true,
//         IssuerSigningKey = signingKey,
//         ValidateIssuer = true,
//         ValidIssuer = configuration["Audience:Issuer"],//发行人
//         ValidateAudience = true,
//         ValidAudience = configuration["Audience:Audience"],//订阅人
//         ValidateLifetime = true,
//         ClockSkew = TimeSpan.Zero,
//         RequireExpirationTime = true,
//     };

// });

#endregion

#region 【第二步：配置认证服务】
// 令牌验证参数
var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = signingKey,
    ValidateIssuer = true,
    ValidIssuer = configuration["Audience:Issuer"],//发行人
    ValidateAudience = true,
    ValidAudience = configuration["Audience:Audience"],//订阅人
    ValidateLifetime = true,
    ClockSkew = TimeSpan.FromSeconds(30),
    RequireExpirationTime = true,
};

//2.1【认证】、core自带官方JWT认证
// 开启Bearer认证
builder.Services.AddAuthentication("Bearer")
 // 添加JwtBearer服务
 .AddJwtBearer(o =>
 {
     o.TokenValidationParameters = tokenValidationParameters;
     o.Events = new JwtBearerEvents
     {
         OnAuthenticationFailed = context =>
         {
             // 如果过期，则把<是否过期>添加到，返回头信息中
             if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
             {
                 context.Response.Headers.Add("Token-Expired", "true");
             }
             return Task.CompletedTask;
         }
     };
 });
#endregion
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
app.UseHttpsRedirection();
//调用中间件：UseAuthentication（认证），必须在所有需要身份认证的中间件前调用，比如 UseAuthorization（授权）。
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
//app.UseResultWrapper();
//app.UseRequestCulture();
app.Run();
