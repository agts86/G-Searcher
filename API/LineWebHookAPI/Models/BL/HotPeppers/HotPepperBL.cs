using LineWebHookAPI.Models.DB.Repositories;
using LineWebHookAPI.Models.Dto.HotPeppers;
using LineWebHookAPI.Models.DB;
using LineWebHookAPI.Models.Dto.Line.API.Messages.Templates;
using LineWebHookAPI.Models.Dto.Line.API.Messages;
using LineWebHookAPI.Models.Http;
using LineWebHookAPI.Models.Dto.Line.API.Requests;
using LineWebHookAPI.Constants.Line.API;
using LineWebHookAPI.Constants.HotPepper;

namespace LineWebHookAPI.Models.BL.HotPeppers;

/// <summary>
/// ホットペッパーコントローラーのビジネスロジック
/// </summary>
public class HotPepperBL(IConfiguration configuration, LineWebHookContext dbContext,IHostEnvironment env,HttpAdapter http)
{
    /// <summary>
    /// リポジトリ
    /// </summary>
    protected HotPepperRepository HotPepperRepository { get; set; } = new HotPepperRepository(dbContext);

    /// <summary>
    /// 設定情報
    /// </summary>
    private IConfiguration Configuration { get; } = configuration;

    /// <summary>
    /// 環境情報
    /// </summary>
    private IHostEnvironment Env { get; } = env;

    /// <summary>
    /// Http操作クラス
    /// </summary>
    protected HttpAdapter Http { get; set; } = http;

    /// <summary>
    /// ラインフックからの位置情報を受け取り、ホットペッパーAPIを実行し返答する
    /// </summary>
    /// <param name="gourmetGettingDto">位置情報</param>
    /// <returns>LineAPIにPostした内容</returns>
    public async Task<Reply> PostGourmetLocationAsync(GourmetGettingDto gourmetGettingDto)
    {
        var message = gourmetGettingDto.Events.First().Message;
        
        await HotPepperRepository.CreateLogAsync(message);
        await HotPepperRepository.SaveChangesAsync();
        
        var hotPepperHttp = new HotPepperHttp(Http, Configuration);
        var gourmet = await hotPepperHttp.GetGourmetAsync(message);
        var lineHttp = new LineHttp(Http, Configuration);
        var columns = gourmet.Results.ToCarouselTemplateColumns();
        var Reply = new Reply()
        {
            ReplyToken = gourmetGettingDto.Events.First().ReplyToken,
            Messages = columns.Length > 0 ? 
            new TemplateMessage[]
            {
                new()
                {
                    AltText = "検索結果",
                    Template = new CarouselTemplate()
                    {
                        Columns = columns
                    }
                }
            } :
            new TextMessage[]
            {
                new()
                {
                    Text = MessageTexts.NotFound
                }
            }
        };

        // デバッグ実行時はエラーコード確定のため処理しない
        if(!Env.IsDevelopment()) await lineHttp.PostReplyAsync(Reply);
        return Reply;
    }
}
