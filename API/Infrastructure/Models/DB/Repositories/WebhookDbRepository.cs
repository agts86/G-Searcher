using Application.Models.DB.Tables;
using Features.Webhook.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models.DB.Repositories;

/// <summary>
/// Webhook用DBリポジトリ
/// </summary>
internal class WebhookDbRepository(LineWebHookContext dbContext) : IWebhookDbRepository
{
    /// <summary>
    /// EFCoreのコンテキスト
    /// </summary>
    private LineWebHookContext DbContext { get; } = dbContext;

    /// <summary>
    /// データベースの変更を保存する
    /// </summary>
    public async Task SaveChangesAsync()
    {
        await DbContext.SaveChangesAsync();
    }

    /// <summary>
    /// データを登録する
    /// </summary>
    /// <typeparam name="T">データの型</typeparam>
    public async Task CreateAsync<T>(T data) where T : class
    {
        await DbContext.AddAsync(data);
    }
}
