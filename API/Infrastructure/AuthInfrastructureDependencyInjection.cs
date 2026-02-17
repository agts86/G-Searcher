using Features.Auth.Repositories;
using Infrastructure.Models.DB.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class AuthInfrastructureDependencyInjection
{
    public static IServiceCollection AddAuthInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IAuthRepository, AuthRepository>();
        return services;
    }
}
