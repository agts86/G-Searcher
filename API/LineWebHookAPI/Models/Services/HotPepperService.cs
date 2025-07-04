using LineWebHookAPI.Models.DB.Repositories;
using LineWebHookAPI.Models.Dto.Line.API.Messages.Templates;
using LineWebHookAPI.Models.Dto.Line.API.Messages;
using LineWebHookAPI.Models.Http;
using LineWebHookAPI.Models.Dto.Line.API.Requests;
using LineWebHookAPI.Constants.Line.API;
using LineWebHookAPI.Constants.HotPepper;
using LineWebHookAPI.Models.Dto.Line.Hook;

namespace LineWebHookAPI.Models.Services;

/// <summary>
/// ホットペッパーコントローラーのビジネスロジック
/// </summary>
public class HotPepperService
(
    IHotPepperRepository hotPepperRepository,
    IWebHostEnvironment env,
    IHotPepperHttp hotPepperHttp,
    ILineHttp lineHttp
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

    /// <summary>
    /// ラインフックからの位置情報を受け取り、ホットペッパーAPIを実行し返答する
    /// </summary>
    /// <param name="gourmetGettingDto">位置情報</param>
    /// <returns>LineAPIにPostした内容</returns>
    public async Task<Reply[]> PostGourmetLocationAsync(GourmetGettingDto gourmetGettingDto, GenreCode genreCode)
    {
        var replies = new List<Reply>();

        foreach (var e in gourmetGettingDto.Events ?? [])
        {
            await HotPepperRepository.CreateGourmetLogAsync(e.Message);
            await HotPepperRepository.SaveChangesAsync();

            var gourmet = await HotPepperHttp.GetGourmetAsync(e.Message, genreCode);
            var columns = gourmet.Results.ToCarouselTemplateColumns();

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
