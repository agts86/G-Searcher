using Features.Webhook.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Features.Webhook;

public static class WebhookDependencyInjection
{
    public static IServiceCollection AddWebhookFeature(this IServiceCollection services)
    {
        services.AddScoped<IJobLogService, JobLogService>();
        services.AddScoped<IEventLogService, EventLogService>();
        services.AddScoped<ILineReplyService, LineReplyService>();
        services.AddScoped<IWebhookService, WebhookService>();
        services.AddScoped<IJobResultPersistenceService, JobResultPersistenceService>();
        return services;
    }
}
