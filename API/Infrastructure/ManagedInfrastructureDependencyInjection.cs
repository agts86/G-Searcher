using Features.Managed.Repositories;
using Infrastructure.Models.DB.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ManagedInfrastructureDependencyInjection
{
    public static IServiceCollection AddManagedInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IManagedRepository, ManagedRepository>();
        return services;
    }
}
