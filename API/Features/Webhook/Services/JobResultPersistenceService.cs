using Features.Webhook.Dto;
using Shared.Jobs;

namespace Features.Webhook.Services;

public interface IJobResultPersistenceService
{
    /// <summary>
    /// 処理結果キューに溜まっている結果をバッチでDBに永続化する
    /// </summary>
    /// <param name="resultQueue">処理結果キュー</param>
    /// <param name="ct">キャンセルトークン</param>
    Task PersistAsync(IBackgroundJobQueue<LocalJobResultDto> resultQueue, CancellationToken ct);
}

/// <summary>
/// ジョブ処理結果の永続化オーケストレーター
/// </summary>
internal class JobResultPersistenceService
(
    IJobLogService jobLogService,
    IEventLogService eventLogService
) : IJobResultPersistenceService
{
    /// <summary>
    /// ジョブログサービス
    /// </summary>
    private IJobLogService JobLogService { get; } = jobLogService;

    /// <summary>
    /// イベントログサービス
    /// </summary>
    private IEventLogService EventLogService { get; } = eventLogService;

    /// <summary>
    /// 処理結果キューに溜まっている結果をバッチでDBに永続化する
    /// </summary>
    /// <param name="resultQueue">処理結果キュー</param>
    /// <param name="ct">キャンセルトークン</param>
    public async Task PersistAsync(IBackgroundJobQueue<LocalJobResultDto> resultQueue, CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var results = await resultQueue.ReadBatchAsync(ct);
            await EventLogService.CreateEventLogsAsync([.. results.SelectMany(x => x.Results ?? []).Select(x => x.Meta)]);
            await JobLogService.CreatesAsync(results);
        }
    }
}
