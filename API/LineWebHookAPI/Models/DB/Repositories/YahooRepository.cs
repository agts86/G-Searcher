using LineDevSdk.DTOs.MessagingAPIs;
using LineDevSdk.Https;
using LineWebHookAPI.Models.DB.Tables;
using Microsoft.EntityFrameworkCore;
using YahooDeveloperApiClient.YOLP;
using YahooDeveloperApiClient.YOLP.Request;
using YahooDeveloperApiClient.YOLP.Response;

namespace LineWebHookAPI.Models.DB.Repositories;

/// <summary>
/// Yahooコントローラー用リポジトリ
/// </summary>
public interface IYahooRepository
{
    /// <summary>
    /// グルメAPIを実行して結果を取得する
    /// </summary>
    /// <param name="message">位置情報</param>
    /// <param name="genreCode">ジャンルコード</param>
    /// <returns>実行結果</returns>
    Task<LocalSearchResult> GetLocalSearchResultAsync(LocalSearchRequest request);
    
    /// <summary>
    /// LineAPIに返信を送信する
    /// </summary>
    /// <param name="Reply">返答内容</param>
    /// <param name="endPointUrl">エンドポイント</param>
    /// <param name="token">トークン</param>
    Task PostReplyAsync(Reply Reply, string token);

    /// <summary>
    /// データベースの変更を保存する
    /// </summary>
    Task SaveChangesAsync();

    /// <summary>
    /// ジョブログを作成する
    /// </summary>
    /// <param name="jobDto">ジョブログのデータ</param>
    Task CreateAsync<T>(T data) where T : class;

    /// <summary>
    /// コンテキストを更新する
    /// </summary>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    void Update<T>(T data) where T : class;

    /// <summary>
    /// ジョブログを取得する
    /// </summary>
    /// <param name="jobId">ジョブログのID</param>
    /// <returns>ジョブログ</returns>
    Task<JobLog> FetchJobLogAsync(Guid jobId);
}

/// <summary>
/// Yahooコントローラー用リポジトリ
/// </summary>
public class YahooRepository(LineWebHookContext dbContext, IYOLPClient YOLPClient, ILineMessagingClient LineMessagingClient) : IYahooRepository
{
    /// <summary>
    /// EFCoreのコンテキスト
    /// </summary>
    protected LineWebHookContext DbContext { get; } = dbContext;

    /// <summary>
    /// YOLPAPI操作クラス
    /// </summary>
    protected IYOLPClient YOLPClient { get; } = YOLPClient;

    /// <summary>
    /// LineMessagingAPI操作クラス
    /// </summary>
    protected ILineMessagingClient LineMessagingClient { get; } = LineMessagingClient;

    /// <summary>
    /// グルメAPIを実行して結果を取得する
    /// </summary>
    /// <param name="message">位置情報</param>
    /// <param name="genreCode">ジャンルコード</param>
    /// <returns>実行結果</returns>
    public async Task<LocalSearchResult> GetLocalSearchResultAsync(LocalSearchRequest request)
    {
        return await YOLPClient.GetLocalSearchResultAsync(request);
    }

    /// <summary>
    /// LineAPIに返信を送信する
    /// </summary>
    /// <param name="Reply">返答内容</param>
    /// <param name="endPointUrl">エンドポイント</param>
    /// <param name="token">トークン</param>
    public Task PostReplyAsync(Reply Reply, string token)
    {
        return LineMessagingClient.PostReplyAsync(Reply, token);
    }

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

    /// <summary>
    /// データを更新する
    /// </summary>
    /// <typeparam name="T">データの型</typeparam>
    public void Update<T>(T data) where T : class
    {
        DbContext.Update(data);
    }

    /// <summary>
    /// ジョブログを取得する
    /// </summary>
    /// <param name="jobId">ジョブログのID</param>
    /// <returns>ジョブログ</returns>
    public async Task<JobLog> FetchJobLogAsync(Guid jobId)
    {
        return await DbContext.JobLogs.Where(j => j.Id == jobId).FirstOrDefaultAsync();
    }
}
