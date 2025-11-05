using LineDevSdk.DTOs.Commons.Messages;
using LineDevSdk.DTOs.MessagingAPIs;
using LineDevSdk.Https;
using LineWebHookAPI.Models.DB.Tables;
using LineWebHookAPI.Models.Dto.Yahoo;
using LineWebHookAPI.Models.Http;

namespace LineWebHookAPI.Models.DB.Repositories;

/// <summary>
/// Yahooコントローラー用リポジトリ
/// </summary>
public interface IYahooRepository
{
    /// <summary>
    /// ログを作成する
    /// </summary>
    /// <param name="dto">位置情報メッセージ</param>
    Task CreateGourmetLogAsync(IMessage message);

    /// <summary>
    /// グルメAPIを実行して結果を取得する
    /// </summary>
    /// <param name="message">位置情報</param>
    /// <param name="genreCode">ジャンルコード</param>
    /// <returns>実行結果</returns>
    Task<LocalDto> GetLocateAsync(IMessage message, string genreCode);
    
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
}

/// <summary>
/// Yahooコントローラー用リポジトリ
/// </summary>
public class YahooRepository(LineWebHookContext dbContext, IYahooClient YahooClient, ILineMessagingClient LineMessagingClient) : IYahooRepository
{
    /// <summary>
    /// EFCoreのコンテキスト
    /// </summary>
    protected LineWebHookContext DbContext { get; } = dbContext;

    /// <summary>
    /// YOLPAPI操作クラス
    /// </summary>
    protected IYahooClient YahooClient { get; } = YahooClient;

    /// <summary>
    /// LineMessagingAPI操作クラス
    /// </summary>
    protected ILineMessagingClient LineMessagingClient { get; } = LineMessagingClient;

    /// <summary>
    /// ログを作成する
    /// </summary>
    /// <param name="dto">位置情報メッセージ</param>
    public async Task CreateGourmetLogAsync(IMessage message)
    {
        if (message is LocationMessage locationMessage)
        {
            await CreateGourmetLogAsync(locationMessage);
        }
        else if (message is TextMessage textMessage)
        {
            await CreateGourmetLogAsync(textMessage);
        }
    }

    /// <summary>
    /// 位置情報ログを作成する
    /// </summary>
    /// <param name="dto">位置情報メッセージ</param>
    private async Task CreateGourmetLogAsync(LocationMessage message)
    {
        var log = new GourmetLocationLog
        {
            Id = Guid.NewGuid(),
            Lat = message.Latitude,
            Lng = message.Longitude
        };
        await DbContext.GourmetLocationLogs.AddAsync(log);
    }

    /// <summary>
    /// テキスト情報ログを作成する
    /// </summary>
    /// <param name="dto">位置情報メッセージ</param>
    private async Task CreateGourmetLogAsync(TextMessage message)
    {
        var log = new GourmetWordLog
        {
            Id = Guid.NewGuid(),
            Text = message.Text
        };
        await DbContext.GourmetWordLogs.AddAsync(log);
    }

    /// <summary>
    /// グルメAPIを実行して結果を取得する
    /// </summary>
    /// <param name="message">位置情報</param>
    /// <param name="genreCode">ジャンルコード</param>
    /// <returns>実行結果</returns>
    public Task<LocalDto> GetLocateAsync(IMessage message, string genreCode)
    {
        return YahooClient.GetLocateAsync(message, genreCode);
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
}
