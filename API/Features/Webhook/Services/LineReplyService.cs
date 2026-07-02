using LineDevSdk.DTO.Commons.Messages;
using LineDevSdk.DTO.Commons.Messages.Templates;
using LineDevSdk.DTO.MessagingAPIs;
using LineDevSdk.DTO.WebHooks;
using LineDevSdk.DTO.WebHooks.Events;
using LineDevSdk.Http;
using Features.Webhook.Dto;
using Features.Webhook.Constants;
using Features.Webhook.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using YahooDeveloperApiClient.YOLP;
using YahooDeveloperApiClient.YOLP.Request;

namespace Features.Webhook.Services;

public interface ILineReplyService
{
    /// <summary>
    /// ラインフックからのリクエストを受け取り、YahooAPIを実行し返答する
    /// </summary>
    /// <param name="gourmetGettingDto">リクエスト</param>
    /// <param name="genreCode">ジャンルコード</param>
    /// <returns>LineAPIにPostした内容</returns>
    Task<LocalEventResultDto[]> PostLocalAsync(WebHook gourmetGettingDto, string genreCode);
}

/// <summary>
/// Yahoo検索とLINE返信のビジネスロジック
/// </summary>
internal class LineReplyService
(
    IYOLPClient yolpClient,
    ILineMessagingClient lineMessagingClient,
    IWebHostEnvironment env,
    IConfiguration configuration
) : ILineReplyService
{
    /// <summary>
    /// YOLPAPI操作クラス
    /// </summary>
    private IYOLPClient YOLPClient { get; } = yolpClient;

    /// <summary>
    /// LineMessagingAPI操作クラス
    /// </summary>
    private ILineMessagingClient LineMessagingClient { get; } = lineMessagingClient;

    /// <summary>
    /// 環境情報
    /// </summary>
    private IWebHostEnvironment Env { get; } = env;

    /// <summary>
    /// 設定情報
    /// </summary>
    private IConfiguration Configuration { get; } = configuration;

    /// <summary>
    /// ラインフックからのリクエストを受け取り、YahooAPIを実行し返答する
    /// </summary>
    /// <param name="gourmetGettingDto">リクエスト</param>
    /// <returns>LineAPIにPostした内容</returns>
    public async Task<LocalEventResultDto[]> PostLocalAsync(WebHook gourmetGettingDto, string genreCode)
    {
        var results = new List<LocalEventResultDto>();

        foreach (var e in gourmetGettingDto.Events ?? [])
        {
            if (e is not MessageEvent messageEvent) continue;

            var log = messageEvent.Message.ConvertGourmetLog();

            var localSearchRequest = new LocalSearchRequest()
            {
                Dist = 1,
                Results = 20,
                GenreCode = genreCode,
                Detail = YdfDetailLevel.Full
            };
            localSearchRequest.MergeMessageInfo(messageEvent.Message);
            var gourmet = await YOLPClient.GetLocalSearchResultAsync(localSearchRequest);
            var columns = gourmet.ToCarouselTemplateColumns();

            var reply = new Reply()
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
            };

            results.Add(new LocalEventResultDto(reply, log));
        }

        // デバッグ実行時はエラーコード確定のため処理しない
        if (!Env.IsDevelopment())
            await Task.WhenAll
            (
                results.Select
                (
                    async x => x.IsReplySucceeded = await PostReplyAsync(x.Reply)
                )
            );

        return [.. results];
    }

    /// <summary>
    /// LineAPIに返信を送信する
    /// </summary>
    /// <param name="reply">返答内容</param>
    /// <returns>送信に成功したか</returns>
    private async Task<bool> PostReplyAsync(Reply reply)
    {
        try
        {
            await LineMessagingClient.PostReplyAsync(reply, Configuration.GetValue<string>("Line:Token"));
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
