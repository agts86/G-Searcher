using Features.Webhook.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Features.Webhook;

public static class WebhookDependencyInjection
{
    public static IServiceCollection AddWebhookFeature(this IServiceCollection services)
    {
        services.AddScoped<IWebhookService, WebhookService>();
        return services;
    }
}
