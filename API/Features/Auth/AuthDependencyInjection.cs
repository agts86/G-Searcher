using Features.Auth.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Features.Auth;

public static class AuthDependencyInjection
{
    public static IServiceCollection AddAuthFeature(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}
