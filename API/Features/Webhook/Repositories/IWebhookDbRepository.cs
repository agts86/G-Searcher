using Application.Models.DB.Tables;

namespace Features.Webhook.Repositories;

/// <summary>
/// Webhook用DBリポジトリ
/// </summary>
internal interface IWebhookDbRepository
{
    /// <summary>
    /// データベースの変更を保存する
    /// </summary>
    Task SaveChangesAsync();

    /// <summary>
    /// データを登録する
    /// </summary>
    /// <typeparam name="T">データの型</typeparam>
    Task CreateAsync<T>(T data) where T : class;
}
