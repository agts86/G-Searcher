using Features.Managed.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Features.Managed;

public static class ManagedDependencyInjection
{
    public static IServiceCollection AddManagedFeature(this IServiceCollection services)
    {
        services.AddScoped<IManagedService, ManagedService>();
        return services;
    }
}
