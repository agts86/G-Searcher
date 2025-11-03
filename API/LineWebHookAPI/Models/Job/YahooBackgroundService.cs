using LineWebHookAPI.Models.Dto.Yahoo;
using LineWebHookAPI.Models.Services;

namespace LineWebHookAPI.Models.Job;

/// <summary>
/// Yahoo!ローカルサーチ非同期ジョブサービス
/// </summary>
/// <typeparam name="YahooLocalJob"></typeparam>
public class YahooBackgroundService(IServiceProvider sp, IBackgroundJobQueue<YahooLocalJob> queue) : BackgroundService
{
    /// <summary>
    /// サービスプロバイダー
    /// </summary>
    private IServiceProvider ServiceProvider { get; } = sp;

    /// <summary>
    /// バックグラウンドジョブキュー
    /// </summary>
    private IBackgroundJobQueue<YahooLocalJob> Queue { get; } = queue;

    /// <summary>
    /// 実行処理
    /// </summary>
    /// <param name="stoppingToken">キャンセルトークン</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var job in Queue.ReadAllAsync(stoppingToken))
        {
            using var scope = ServiceProvider.CreateScope();
            var yahooService = scope.ServiceProvider.GetRequiredService<IYahooService>();
            await yahooService.PostLocalAsync(job.WebHook, job.GenreCode);
        }
    }
}
