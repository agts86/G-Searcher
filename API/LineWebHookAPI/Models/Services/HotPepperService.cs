using LineWebHookAPI.Models.DB.Repositories;
using LineWebHookAPI.Models.Http;
using LineWebHookAPI.Constants.HotPepper;
using LineDevSdk.Https;
using LineDevSdk.DTOs.MessagingAPIs;
using LineDevSdk.DTOs.Commons.Messages.Templates;
using LineDevSdk.Dtos.Commons.Messages;
using LineDevSdk.DTOs.Commons.Messages;
using LineDevSdk.DTOs.WebHooks;
using LineDevSdk.DTOs.WebHooks.Events;
using LineWebHookAPI.Constants.Line.API;

namespace LineWebHookAPI.Models.Services;

/// <summary>
/// ホットペッパーコントローラーのビジネスロジック
/// </summary>
public class HotPepperService
(
    IHotPepperRepository hotPepperRepository,
    IWebHostEnvironment env,
    IHotPepperHttp hotPepperHttp,
    ILineHttp lineHttp,
    IConfiguration configuration
)
{
    /// <summary>
    /// リポジトリ
    /// </summary>
    protected IHotPepperRepository HotPepperRepository { get; set; } = hotPepperRepository;

    /// <summary>
    /// 環境情報
    /// </summary>
    private IWebHostEnvironment Env { get; } = env;

    /// <summary>
    /// HotPepperAPI操作クラス
    /// </summary>
    protected IHotPepperHttp HotPepperHttp { get; set; } = hotPepperHttp;

    /// <summary>
    /// LineMessagingAPI操作クラス
    /// </summary>
    protected ILineHttp LineHttp { get; set; } = lineHttp;

    protected IConfiguration Configuration { get; } = configuration;

    /// <summary>
    /// ラインフックからの位置情報を受け取り、ホットペッパーAPIを実行し返答する
    /// </summary>
    /// <param name="gourmetGettingDto">位置情報</param>
    /// <returns>LineAPIにPostした内容</returns>
    public async Task<Reply[]> PostGourmetLocationAsync(WebHook gourmetGettingDto, GenreCode genreCode)
    {
        var replies = new List<Reply>();

        foreach (var e in gourmetGettingDto.Events ?? [])
        {
            if (e is not MessageEvent messageEvent) continue;
            await HotPepperRepository.CreateGourmetLogAsync(messageEvent.Message);
            await HotPepperRepository.SaveChangesAsync();

            var gourmet = await HotPepperHttp.GetGourmetAsync(messageEvent.Message, genreCode);
            var columns = gourmet.Results.ToCarouselTemplateColumns();

            replies.Add
            (
                new Reply()
                {
                    ReplyToken = messageEvent.ReplyToken,
                    Messages =
                    [
                        columns.Length > 0 ?
                        new TemplateMessage()
                        {
                            AltText = "検索結果",
                            Template = new CarouselTemplate()
                            {
                                Columns = columns
                            }
                        }:
                        new TextV2Message()
                        {
                            Text = MessageTexts.NotFound
                        }
                    ]
                }
            );
        }

        // デバッグ実行時はエラーコード確定のため処理しない
        if (!Env.IsDevelopment())
            replies.ForEach
            (
                async x => await LineHttp.PostReplyAsync
                (
                    x,
                    string.Format(Configuration.GetValue<string>("Line:Url"), "reply"),
                    Configuration.GetValue<string>("Line:Token")
                )
            );
        return [.. replies];
    }
}
