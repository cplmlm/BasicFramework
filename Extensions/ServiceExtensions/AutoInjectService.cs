﻿using Common;
using Microsoft.Extensions.DependencyInjection;
using Common.Extensions;
using Repository.Base;
using IServices.Base;
using Services.Base;
using Autofac.Core;
using System.Reflection;
using Repository.UnitOfWorks;

namespace Core.AutoInjectService;
/// <summary>
/// 自动注入服务类
/// </summary>
public static class AutoInjectService
{
    /// <summary>
    /// 自动注入服务
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    public static IServiceCollection AutoRegistryService(this IServiceCollection services)
    {
        var basePath = AppContext.BaseDirectory;
        var servicesDllFile = Path.Combine(basePath, "Services.dll");
        var repositoryDllFile = Path.Combine(basePath, "Repository.dll");
        if (!(File.Exists(servicesDllFile) && File.Exists(repositoryDllFile)))
        {
            var msg = "Repository.dll和service.dll 丢失，因为项目解耦了，所以需要先F6编译，再F5运行，请检查 bin 文件夹，并拷贝。";
            //Log.Error(msg);
            throw new Exception(msg);
        }
        // 获取 Service.dll 程序集服务，并注册
        var assemblysServices = Assembly.LoadFrom(servicesDllFile);
        var types = assemblysServices.GetTypes().Where(x=>x.Name.Contains("Services") && !x.Name.Contains("Base") && x.IsClass);
        // List<Type> types = AssemblysExtensions.GetAllAssemblies().SelectMany(t => t.GetTypes()).Where(t => t.GetCustomAttributes(typeof(ServiceAttribute), false).Length > 0 && t.GetCustomAttribute<ServiceAttribute>()?.Lifetime == serviceLifetime && t.IsClass && !t.IsAbstract).ToList();
        foreach (var serviceType in types)
        {
            var type = serviceType.GetInterfaces().LastOrDefault();
            services.AddTransient(type, serviceType);
        }
        // 获取 Repository.dll 程序集服务，并注册
        var assemblysRepository = Assembly.LoadFrom(repositoryDllFile);
        types = assemblysRepository.GetTypes().Where(x => x.Name.Contains("Repository") && !x.Name.Contains("Base") && x.IsClass);
        foreach (var serviceType in types)
        {
            var type = serviceType.GetInterfaces().LastOrDefault();
            services.AddTransient(type, serviceType);
        }
        //注册UnitOfWorkManage
        services.AddScoped<IUnitOfWorkManage, UnitOfWorkManage>();
        //注册泛型接口和实现
        services.AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddTransient(typeof(IBaseServices<>), typeof(BaseServices<>));
        return services;
    }
}