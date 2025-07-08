using LineWebHookAPI.Models.DB.Repositories;
using LineWebHookAPI.Models.Http;
using LineWebHookAPI.Constants.Line.API;
using LineDevSdk.Https;
using LineDevSdk.DTOs.MessagingAPIs;
using LineDevSdk.DTOs.Commons.Messages.Templates;
using LineDevSdk.Dtos.Commons.Messages;
using LineDevSdk.DTOs.Commons.Messages;
using LineDevSdk.DTOs.WebHooks;
using LineDevSdk.DTOs.WebHooks.Events;

namespace LineWebHookAPI.Models.Services;

/// <summary>
/// YahooBコントローラーのビジネスロジック
/// </summary>
public class YahooService
(
    IYahooRepository yahooPepperRepository,
    IWebHostEnvironment env,
    IYahooHttp yahooHttp,
    ILineHttp lineHttp,
    IConfiguration configuration
)
{
    /// <summary>
    /// リポジトリ
    /// </summary>
    protected IYahooRepository YahooPepperRepository { get; set; } = yahooPepperRepository;

    /// <summary>
    /// 環境情報
    /// </summary>
    private IWebHostEnvironment Env { get; } = env;

    /// <summary>
    /// HotPepperAPI操作クラス
    /// </summary>
    protected IYahooHttp YahooHttp { get; set; } = yahooHttp;

    /// <summary>
    /// LineMessagingAPI操作クラス
    /// </summary>
    protected ILineHttp LineHttp { get; set; } = lineHttp;

    protected IConfiguration Configuration { get; } = configuration;

    /// <summary>
    /// ラインフックからの位置情報を受け取り、YahooAPIを実行し返答する
    /// </summary>
    /// <param name="gourmetGettingDto">位置情報</param>
    /// <returns>LineAPIにPostした内容</returns>
    public async Task<Reply[]> PostLocalAsync(WebHook gourmetGettingDto, string genreCode)
    {
        var replies = new List<Reply>();

        foreach (var e in gourmetGettingDto.Events ?? [])
        {
            if (e is not MessageEvent messageEvent) continue;
            await YahooPepperRepository.CreateGourmetLogAsync(messageEvent.Message);
            await YahooPepperRepository.SaveChangesAsync();

            var gourmet = await YahooHttp.GetLocateAsync(messageEvent.Message, genreCode);
            var columns = gourmet.ToCarouselTemplateColumns();

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

