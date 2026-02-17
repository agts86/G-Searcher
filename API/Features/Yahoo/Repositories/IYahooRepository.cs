using LineDevSdk.DTO.MessagingAPIs;
using Application.Models.DB.Tables;
using YahooDeveloperApiClient.YOLP.Request;
using YahooDeveloperApiClient.YOLP.Response;

namespace Features.Yahoo.Repositories;

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
