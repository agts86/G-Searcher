using LineWebHookAPI.Models.DB.Tables;
using LineWebHookAPI.Models.Dto.Line.Hook.Messages;

namespace LineWebHookAPI.Models.DB.Repositories;

/// <summary>
/// HotPepperコントローラー用リポジトリ
/// </summary>
public interface IHotPepperRepository
{
    /// <summary>
    /// ログを作成する
    /// </summary>
    /// <param name="dto">位置情報メッセージ</param>
    Task CreateGourmetLogAsync(Message message);

    /// <summary>
    /// データベースの変更を保存する
    /// </summary>
    Task SaveChangesAsync();
}

/// <summary>
/// HotPepperコントローラー用リポジトリ
/// </summary>
public class HotPepperRepository(LineWebHookContext dbContext) : IHotPepperRepository
{
    /// <summary>
    /// EFCoreのコンテキスト
    /// </summary>
    private LineWebHookContext DbContext { get; } = dbContext;

    /// <summary>
    /// ログを作成する
    /// </summary>
    /// <param name="dto">位置情報メッセージ</param>
    public async Task CreateGourmetLogAsync(Message message)
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
    /// 保存する
    /// </summary>
    public async Task SaveChangesAsync()
    {
        await DbContext.SaveChangesAsync();
    }
}
