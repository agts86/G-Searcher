using Features.Webhook.Dto;
using Shared.Jobs;
using Features.Webhook.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Models.Job;

/// <summary>
/// Webhook非同期ジョブサービス
/// </summary>
/// <typeparam name="LocalJobDto"></typeparam>
internal class WebhookBackgroundService(IServiceProvider sp, IBackgroundJobQueue<LocalJobDto> queue) : BackgroundService
{
    /// <summary>
    /// サービスプロバイダー
    /// </summary>
    private IServiceProvider ServiceProvider { get; } = sp;

    /// <summary>
    /// バックグラウンドジョブキュー
    /// </summary>
    private IBackgroundJobQueue<LocalJobDto> Queue { get; } = queue;

    /// <summary>
    /// 実行処理
    /// </summary>
    /// <param name="ct">キャンセルトークン</param>
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        try
        {
            using var scope = ServiceProvider.CreateScope();
            var webhookService = scope.ServiceProvider.GetRequiredService<IWebhookService>();
            await webhookService.PostLocalJobAsync(Queue, ct);
        }
        catch { }
    }
}
