using Features.Yahoo.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Features.Yahoo;

public static class YahooDependencyInjection
{
    public static IServiceCollection AddYahooFeature(this IServiceCollection services)
    {
        services.AddScoped<IYahooService, YahooService>();
        return services;
    }
}
