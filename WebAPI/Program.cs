using Extensions.AutoMapper;
using Extensions;
using Common;
using Common.LifetimeInterfaces;
using Core.AutoInjectService;
using Autofac;
using Extensions.ServiceExtensions;
using Autofac.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
//≈‰÷√host”Î»›∆˜
//builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
//    .ConfigureContainer<ContainerBuilder>(builder =>
//    {
//        builder.RegisterModule(new AutofacModuleRegister());
//    });

// 2°¢≈‰÷√∑˛ŒÒ
//builder.Services.AutoRegistryService(ServiceLifetime.Transient);
builder.Services.AutoRegistryService();
builder.Services.AddSingleton(new AppSettings(builder.Configuration));
builder.Services.AddControllers();
builder.Services.AddCacheSetup();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextSetup();
builder.Services.AddAuthorizationSetup();
builder.Services.AddAuthentication_JWTSetup();
builder.Services.AddAutoMapper(typeof(CustomProfile));
builder.Services.AddSqlsugarSetup();
builder.Services.AddSwaggerSetup();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
