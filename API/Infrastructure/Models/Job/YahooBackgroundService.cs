using Application.Models.Dto.Yahoo;
using Application.Models.Job;
using Application.Models.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Models.Job;

/// <summary>
/// Yahoo!ローカルサーチ非同期ジョブサービス
/// </summary>
/// <typeparam name="LocalJobDto"></typeparam>
public class YahooBackgroundService(IServiceProvider sp, IBackgroundJobQueue<LocalJobDto> queue) : BackgroundService
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
            var yahooService = scope.ServiceProvider.GetRequiredService<IYahooService>();
            await yahooService.PostLocalJobAsync(Queue, ct);
        }
        catch { }
    }
}
