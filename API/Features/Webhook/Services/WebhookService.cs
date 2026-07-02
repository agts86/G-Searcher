using Features.Webhook.Dto;
using Shared.Jobs;

namespace Features.Webhook.Services;

public interface IWebhookService
{
    /// <summary>
    /// キューに溜まっているジョブを処理し、結果を永続化キューへ渡す
    /// </summary>
    /// <param name="queue">ジョブキュー</param>
    /// <param name="resultQueue">処理結果キュー</param>
    /// <param name="ct">キャンセルトークン</param>
    Task PostLocalJobAsync(IBackgroundJobQueue<LocalJobDto> queue, IBackgroundJobQueue<LocalJobResultDto> resultQueue, CancellationToken ct);
}

/// <summary>
/// Webhookジョブの返信処理オーケストレーター
/// </summary>
internal class WebhookService(ILineReplyService lineReplyService) : IWebhookService
{
    /// <summary>
    /// LINE返信サービス
    /// </summary>
    private ILineReplyService LineReplyService { get; } = lineReplyService;

    /// <summary>
    /// キューに溜まっているジョブを処理し、結果を永続化キューへ渡す
    /// </summary>
    /// <param name="queue">ジョブキュー</param>
    /// <param name="resultQueue">処理結果キュー</param>
    /// <param name="ct">キャンセルトークン</param>
    public async Task PostLocalJobAsync(IBackgroundJobQueue<LocalJobDto> queue, IBackgroundJobQueue<LocalJobResultDto> resultQueue, CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var jobs = await queue.ReadBatchAsync(ct);
            foreach (var job in jobs)
            {
                LocalEventResultDto[] results = null;
                string errorMessage = null;
                try
                {
                    results = await LineReplyService.PostLocalAsync(job.WebHook, job.GenreCode);
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                }
                await resultQueue.EnqueueAsync(new LocalJobResultDto(job, results, errorMessage), ct);
            }
        }
    }
}
