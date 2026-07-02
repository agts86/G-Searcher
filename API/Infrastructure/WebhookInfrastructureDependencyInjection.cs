using Features.Webhook.Dto;
using Features.Webhook.Repositories;
using Infrastructure.Models.DB.Repositories;
using Infrastructure.Models.Job;
using Microsoft.Extensions.DependencyInjection;
using Shared.Jobs;

namespace Infrastructure;

public static class WebhookInfrastructureDependencyInjection
{
    public static IServiceCollection AddWebhookInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IWebhookDbRepository, WebhookDbRepository>();
        services.AddSingleton<IBackgroundJobQueue<LocalJobDto>, BackgroundJobQueue<LocalJobDto>>();
        services.AddSingleton<IBackgroundJobQueue<LocalJobResultDto>, BackgroundJobQueue<LocalJobResultDto>>();
        services.AddHostedService<WebhookBackgroundService>();
        services.AddHostedService<JobResultPersistenceBackgroundService>();
        return services;
    }
}
