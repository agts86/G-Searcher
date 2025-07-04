using LineWebHookAPI.Models.DB.Repositories;
using LineWebHookAPI.Models.Dto.Line.API.Messages.Templates;
using LineWebHookAPI.Models.Dto.Line.API.Messages;
using LineWebHookAPI.Models.Http;
using LineWebHookAPI.Models.Dto.Line.API.Requests;
using LineWebHookAPI.Constants.Line.API;
using LineWebHookAPI.Models.Dto.Line.Hook;

namespace LineWebHookAPI.Models.Services.Yahoo;

/// <summary>
/// YahooBコントローラーのビジネスロジック
/// </summary>
public class YahooService
(
    IConfiguration configuration,
    YahooRepositoryBase yahooPepperRepository,
    IHostEnvironment env,
    IYahooHttp yahooHttp,
    ILineHttp lineHttp
)
{
    /// <summary>
    /// リポジトリ
    /// </summary>
    protected YahooRepositoryBase YahooPepperRepository { get; set; } = yahooPepperRepository;

    /// <summary>
    /// 設定情報
    /// </summary>
    private IConfiguration Configuration { get; } = configuration;

    /// <summary>
    /// 環境情報
    /// </summary>
    private IHostEnvironment Env { get; } = env;

    /// <summary>
    /// HotPepperAPI操作クラス
    /// </summary>
    protected IYahooHttp YahooHttp { get; set; } = yahooHttp;

    /// <summary>
    /// LineMessagingAPI操作クラス
    /// </summary>
    protected ILineHttp LineHttp { get; set; } = lineHttp;

    /// <summary>
    /// ラインフックからの位置情報を受け取り、YahooAPIを実行し返答する
    /// </summary>
    /// <param name="gourmetGettingDto">位置情報</param>
    /// <returns>LineAPIにPostした内容</returns>
    public async Task<Reply[]> PostLocalAsync(GourmetGettingDto gourmetGettingDto, string genreCode)
    {
        var replies = new List<Reply>();

        foreach (var e in gourmetGettingDto.Events ?? [])
        {
            await YahooPepperRepository.CreateGourmetLogAsync(e.Message);
            await YahooPepperRepository.SaveChangesAsync();

            var gourmet = await YahooHttp.GetLocateAsync(e.Message, genreCode);
            var columns = gourmet.ToCarouselTemplateColumns();

            replies.Add
            (
                new Reply()
                {
                    ReplyToken = e.ReplyToken,
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
        if (!Env.IsDevelopment()) replies.ForEach(async x => await LineHttp.PostReplyAsync(x));
        return [.. replies];
    }
}

