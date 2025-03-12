using LineWebHookAPI.Models.DB.Repositories;
using LineWebHookAPI.Models.Dto.HotPeppers;
using LineWebHookAPI.Models.DB;
using LineWebHookAPI.Models.Dto.Line.API.Messages.Templates;
using LineWebHookAPI.Models.Dto.Line.API.Messages;
using LineWebHookAPI.Http;
using LineWebHookAPI.Models.Dto.Line.API.Requests;

namespace LineWebHookAPI.Models.BL.HotPeppers;

/// <summary>
/// ホットペッパーコントローラーのビジネスロジック
/// </summary>
public class HotPepperBL(IConfiguration configuration, MyContext dbContext)
{
    /// <summary>
    /// リポジトリ
    /// </summary>
    public HotPepperRepository HotPepperRepository { get; protected set; } = new HotPepperRepository(dbContext);

    /// <summary>
    /// 設定情報
    /// </summary>
    private IConfiguration Configuration { get; } = configuration;

    /// <summary>
    /// Http操作クラス
    /// </summary>
    public HttpAdapter Http { get; protected set; } = new HttpAdapter();

    /// <summary>
    /// ラインフックからの位置情報を受け取り、ホットペッパーAPIを実行し返答する
    /// </summary>
    /// <param name="gourmetGettingDto">位置情報</param>
    /// <returns>LineAPIにPostした内容</returns>
    public async Task<Replay> PostGourmetLocationAsync(GourmetGettingDto gourmetGettingDto)
    {
        var message = gourmetGettingDto.Events.First().Message;
        
        await HotPepperRepository.CreateLogAsync(message);
        await HotPepperRepository.SaveChangesAsync();
        
        var hotPepperHttp = new HotPepperHttp(Http, Configuration);
        var gourmet = await hotPepperHttp.GetGourmetAsync(message);
        var lineHttp = new LineHttp(Http, Configuration);
        // クリックリファレンスがでているがその通り対応するとインターフェイス型でシリアライズ時されるのであえてこのままにする
        var replay = new Replay()
        {
            ReplyToken = gourmetGettingDto.Events.First().ReplyToken,
            Messages = new TemplateMessage[]
            {
                new()
                {
                    AltText = "検索結果",
                    Template = new CarouselTemplate()
                    {
                        Columns = gourmet.Results.ToCarouselTemplateColumns()
                    }
                }
            }
        };
        // デバッグ実行時はエラーコード確定のため処理しない
        #if PRODUCTION
        await lineHttp.PostReplayAsync(replay);
        #endif
        return replay;
    }
}
