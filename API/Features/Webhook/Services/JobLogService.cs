using Application.Models.DB.Tables;
using Features.Webhook.Dto;
using Features.Webhook.Repositories;

namespace Features.Webhook.Services;

public interface IJobLogService
{
    /// <summary>
    /// ジョブログをまとめて登録する
    /// </summary>
    /// <param name="results">ジョブの処理結果一覧</param>
    Task CreatesAsync(IEnumerable<LocalJobResultDto> results);
}

/// <summary>
/// ジョブログのビジネスロジック
/// </summary>
internal class JobLogService(IWebhookDbRepository webhookDbRepository) : IJobLogService
{
    /// <summary>
    /// DBリポジトリ
    /// </summary>
    private IWebhookDbRepository WebhookDbRepository { get; } = webhookDbRepository;

    /// <summary>
    /// ジョブログをまとめて登録する
    /// </summary>
    /// <param name="results">ジョブの処理結果一覧</param>
    public async Task CreatesAsync(IEnumerable<LocalJobResultDto> results)
    {
        foreach (var result in results)
        {
            var log = new JobLog
            {
                Id = result.Job.Id,
                Contents = result.Job.GetBody(),
                IsSuccess = result.IsSuccess,
                Info = result.ErrorMessage
            };
            await WebhookDbRepository.CreateAsync(log);
        }
        await WebhookDbRepository.SaveChangesAsync();
    }
}
