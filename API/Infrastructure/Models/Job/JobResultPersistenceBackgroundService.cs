using Features.Webhook.Dto;
using Shared.Jobs;
using Features.Webhook.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Models.Job;

/// <summary>
/// ジョブ処理結果の永続化バックグラウンドサービス
/// </summary>
internal class JobResultPersistenceBackgroundService(IServiceProvider sp, IBackgroundJobQueue<LocalJobResultDto> resultQueue) : BackgroundService
{
    /// <summary>
    /// サービスプロバイダー
    /// </summary>
    private IServiceProvider ServiceProvider { get; } = sp;

    /// <summary>
    /// 処理結果キュー
    /// </summary>
    private IBackgroundJobQueue<LocalJobResultDto> ResultQueue { get; } = resultQueue;

    /// <summary>
    /// 実行処理
    /// </summary>
    /// <param name="ct">キャンセルトークン</param>
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        try
        {
            using var scope = ServiceProvider.CreateScope();
            var jobResultPersistenceService = scope.ServiceProvider.GetRequiredService<IJobResultPersistenceService>();
            await jobResultPersistenceService.PersistAsync(ResultQueue, ct);
        }
        catch { }
    }
}
