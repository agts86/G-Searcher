using Application.Models.DB.Tables;
using Features.Webhook.Repositories;

namespace Features.Webhook.Services;

public interface IEventLogService
{
    /// <summary>
    /// イベントログを登録する
    /// </summary>
    /// <param name="logs">登録するログ一覧</param>
    Task CreateEventLogsAsync(Meta[] logs);
}

/// <summary>
/// イベントログのビジネスロジック
/// </summary>
internal class EventLogService(IWebhookDbRepository webhookDbRepository) : IEventLogService
{
    /// <summary>
    /// DBリポジトリ
    /// </summary>
    private IWebhookDbRepository WebhookDbRepository { get; } = webhookDbRepository;

    /// <summary>
    /// イベントログを登録する
    /// </summary>
    /// <param name="logs">登録するログ一覧</param>
    public async Task CreateEventLogsAsync(Meta[] logs)
    {
        foreach (var log in logs)
        {
            await WebhookDbRepository.CreateAsync(log);
        }
        await WebhookDbRepository.SaveChangesAsync();
    }
}
