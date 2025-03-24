using LineWebHookAPI.Models.DB.Repositories;
using LineWebHookAPI.Models.Dto.HotPeppers;
using LineWebHookAPI.Models.DB;
using LineWebHookAPI.Models.Dto.Line.API.Messages.Templates;
using LineWebHookAPI.Models.Dto.Line.API.Messages;
using LineWebHookAPI.Models.Http;
using LineWebHookAPI.Models.Dto.Line.API.Requests;
using LineWebHookAPI.Constants.Line.API;
using LineWebHookAPI.Constants.HotPepper;
using LineWebHookAPI.Models.Dto.Line.Hook.Messages;

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
    public async Task<Reply[]> PostGourmetLocationAsync(GourmetGettingDto gourmetGettingDto, GenreCode genreCode)
    {
        var hotPepperHttp = new HotPepperHttp(Http, Configuration);
        var lineHttp = new LineHttp(Http, Configuration);
        var replies = new List<Reply>();

        foreach(var e in gourmetGettingDto.Events ?? [])
        {
            if(e.Message is LocationMessage message) 
            {
                await HotPepperRepository.CreateLogAsync(message);
                await HotPepperRepository.SaveChangesAsync();
            }
            
            
            var gourmet = await hotPepperHttp.GetGourmetAsync(e.Message,genreCode);
            
            var columns = gourmet.Results.ToCarouselTemplateColumns();
            replies.Add
            (
                new Reply()
                {
                    ReplyToken = e.ReplyToken,
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
                    new Dto.Line.API.Messages.TextMessage[]
                    {
                        new()
                        {
                            Text = MessageTexts.NotFound
                        }
                    }
                }
            );
        }   
        
        // デバッグ実行時はエラーコード確定のため処理しない
        if(!Env.IsDevelopment()) replies.ForEach(async x => await lineHttp.PostReplyAsync(x));
        return [.. replies];
    }
}
