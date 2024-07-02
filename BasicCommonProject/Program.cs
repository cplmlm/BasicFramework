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
// 1������host������
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
               .SetMinimumLevel(LogLevel.Information)  // ������־����
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

    #region Token�󶨵�ConfigureServices
    // ����С��
    c.OperationFilter<AddResponseHeadersFilter>();
    c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
    // ��header�����token�����ݵ���̨
    c.OperationFilter<SecurityRequirementsOperationFilter>();

    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "JWT��Ȩ(���ݽ�������ͷ�н��д���) ֱ�����¿�������Bearer {token}��ע������֮����һ���ո�\"",
        Name = "Authorization",//jwtĬ�ϵĲ�������
        In = ParameterLocation.Header,//jwtĬ�ϴ��Authorization��Ϣ��λ��(����ͷ��)
        Type = SecuritySchemeType.ApiKey
    });
    #endregion
});

var configuration = builder.Configuration;
#region ������Ȩ��
#region 1�����ڽ�ɫ��API��Ȩ 

// 1����Ȩ��������ܼ򵥣�����ʲô����������
// �������÷���ֻ��Ҫ��API���controller�ϱߣ��������Լ��ɣ�ע�⣬ֻ���ǽ�ɫ��:
// [Authorize(Roles = "Admin")]

// 2����֤����Ȼ�����±ߵ�configure������м������:app.UseMiddleware<JwtTokenAuth>();��������������޷���֤����ʱ�䣬���������Ҫ��֤����ʱ�䣬������Ҫ�±ߵĵ����ַ������ٷ���֤

#endregion

#region 2�����ڲ��Ե���Ȩ���򵥰棩

// 1����Ȩ����������ϱߵ�����ͬ�����ô����ǲ�����controller�У�д��� roles ��
// Ȼ����ôд [Authorize(Policy = "Admin")]
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Client", policy => policy.RequireRole("Client").Build());
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin").Build());
    options.AddPolicy("SystemOrAdmin", policy => policy.RequireRole("Admin", "System"));
});


// 2����֤����Ȼ�����±ߵ�configure������м������:app.UseMiddleware<JwtTokenAuth>();��������������޷���֤����ʱ�䣬���������Ҫ��֤����ʱ�䣬������Ҫ�±ߵĵ����ַ������ٷ���֤
#endregion
#endregion
#region ����֤��
//��ȡ�����ļ�
var symmetricKeyAsBase64 = configuration["Audience:Secret"];
var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
var signingKey = new SymmetricSecurityKey(keyByteArray);


//2.1����֤��
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
//         ValidIssuer = configuration["Audience:Issuer"],//������
//         ValidateAudience = true,
//         ValidAudience = configuration["Audience:Audience"],//������
//         ValidateLifetime = true,
//         ClockSkew = TimeSpan.Zero,
//         RequireExpirationTime = true,
//     };

// });

#endregion

#region ���ڶ�����������֤����
// ������֤����
var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = signingKey,
    ValidateIssuer = true,
    ValidIssuer = configuration["Audience:Issuer"],//������
    ValidateAudience = true,
    ValidAudience = configuration["Audience:Audience"],//������
    ValidateLifetime = true,
    ClockSkew = TimeSpan.FromSeconds(30),
    RequireExpirationTime = true,
};

//2.1����֤����core�Դ��ٷ�JWT��֤
// ����Bearer��֤
builder.Services.AddAuthentication("Bearer")
 // ���JwtBearer����
 .AddJwtBearer(o =>
 {
     o.TokenValidationParameters = tokenValidationParameters;
     o.Events = new JwtBearerEvents
     {
         OnAuthenticationFailed = context =>
         {
             // ������ڣ����<�Ƿ����>��ӵ�������ͷ��Ϣ��
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
//�����м����UseAuthentication����֤����������������Ҫ�����֤���м��ǰ���ã����� UseAuthorization����Ȩ����
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
//app.UseResultWrapper();
//app.UseRequestCulture();
app.Run();
