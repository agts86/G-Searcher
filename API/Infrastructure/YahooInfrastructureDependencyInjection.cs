using Features.Yahoo.Dto;
using Features.Yahoo.Repositories;
using Infrastructure.Models.DB.Repositories;
using Infrastructure.Models.Job;
using Microsoft.Extensions.DependencyInjection;
using Shared.Jobs;

namespace Infrastructure;

public static class YahooInfrastructureDependencyInjection
{
    public static IServiceCollection AddYahooInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IYahooRepository, YahooRepository>();
        services.AddSingleton<IBackgroundJobQueue<LocalJobDto>, BackgroundJobQueue<LocalJobDto>>();
        services.AddHostedService<YahooBackgroundService>();
        return services;
    }
}
