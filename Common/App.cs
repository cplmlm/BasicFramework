using Common.Core;
using Common.Extensions;
using Common.HttpContextUser;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common;

public class App
{
    static App()
    {
        EffectiveTypes = Assemblies.SelectMany(GetTypes);
    }

    private static bool _isRun;

    /// <summary>是否正在运行</summary>
    public static bool IsBuild { get; set; }

    public static bool IsRun
    {
        get => _isRun;
        set => _isRun = IsBuild = value;
    }

    /// <summary>应用有效程序集</summary>
    public static readonly IEnumerable<Assembly> Assemblies = RuntimeExtension.GetAllAssemblies();

    /// <summary>有效程序集类型</summary>
    public static readonly IEnumerable<Type> EffectiveTypes;

    /// <summary>优先使用App.GetService()手动获取服务</summary>
    public static IServiceProvider RootServices => IsRun || IsBuild ? InternalApp.RootServices : null;

    /// <summary>获取Web主机环境，如，是否是开发环境，生产环境等</summary>
    public static IWebHostEnvironment WebHostEnvironment => InternalApp.WebHostEnvironment;

    /// <summary>获取泛型主机环境，如，是否是开发环境，生产环境等</summary>
    public static IHostEnvironment HostEnvironment => InternalApp.HostEnvironment;

    /// <summary>全局配置选项</summary>
    public static IConfiguration Configuration => InternalApp.Configuration;

    /// <summary>
    /// 获取请求上下文
    /// </summary>
    public static HttpContext HttpContext => RootServices?.GetService<IHttpContextAccessor>()?.HttpContext;

    public static IUser User => GetService<IUser>();

    #region Service

    /// <summary>解析服务提供器</summary>
    /// <param name="serviceType"></param>
    /// <param name="mustBuild"></param>
    /// <param name="throwException"></param>
    /// <returns></returns>
    public static IServiceProvider GetServiceProvider(Type serviceType, bool mustBuild = false, bool throwException = true)
    {
        if (App.HostEnvironment == null || App.RootServices != null &&
            InternalApp.InternalServices
                .Where((u =>
                    u.ServiceType ==
                    (serviceType.IsGenericType ? serviceType.GetGenericTypeDefinition() : serviceType)))
                .Any((u => u.Lifetime == ServiceLifetime.Singleton)))
            return App.RootServices;

        //获取请求生存周期的服务
        if (HttpContext?.RequestServices != null)
            return HttpContext.RequestServices;

        if (App.RootServices != null)
        {
            IServiceScope scope = RootServices.CreateScope();
            return scope.ServiceProvider;
        }

        if (mustBuild)
        {
            if (throwException)
            {
                throw new ApplicationException("当前不可用，必须要等到 WebApplication Build后");
            }

            return default;
        }

        ServiceProvider serviceProvider = InternalApp.InternalServices.BuildServiceProvider();
        return serviceProvider;
    }

    public static TService GetService<TService>(bool mustBuild = true) where TService : class =>
        App.GetService(typeof(TService), null, mustBuild) as TService;

    /// <summary>获取请求生存周期的服务</summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="serviceProvider"></param>
    /// <param name="mustBuild"></param>
    /// <returns></returns>
    public static TService GetService<TService>(IServiceProvider serviceProvider, bool mustBuild = true)
        where TService : class => (serviceProvider ?? App.GetServiceProvider(typeof(TService), mustBuild, false))?.GetService<TService>();

    /// <summary>获取请求生存周期的服务</summary>
    /// <param name="type"></param>
    /// <param name="serviceProvider"></param>
    /// <param name="mustBuild"></param>
    /// <returns></returns>
    public static object GetService(Type type, IServiceProvider serviceProvider = null, bool mustBuild = true) =>
        (serviceProvider ?? App.GetServiceProvider(type, mustBuild, false))?.GetService(type);

    #endregion

    #region private

    /// <summary>加载程序集中的所有类型</summary>
    /// <param name="ass"></param>
    /// <returns></returns>
    private static IEnumerable<Type> GetTypes(Assembly ass)
    {
        Type[] source = Array.Empty<Type>();
        try
        {
            source = ass.GetTypes();
        }
        catch
        {
            $@"Error load `{ass.FullName}` assembly.".WriteErrorLine();
        }

        return source.Where(u => u.IsPublic);
    }

    #endregion

   
}