using Microsoft.Extensions.DependencyInjection;

namespace Common;

[AttributeUsage(AttributeTargets.Class)]
public class ServiceAttribute : System.Attribute
{
    /// <summary>
    /// 服务生命周期
    /// </summary>
    public ServiceLifetime Lifetime {get; set;}=ServiceLifetime.Transient;
    public ServiceAttribute(ServiceLifetime lifetime)
    {
        Lifetime = lifetime;
    }
}