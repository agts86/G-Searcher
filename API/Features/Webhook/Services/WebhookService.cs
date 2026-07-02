using LineDevSdk.DTO.Commons.Messages;
using LineDevSdk.DTO.Commons.Messages.Templates;
using LineDevSdk.DTO.MessagingAPIs;
using LineDevSdk.DTO.WebHooks;
using LineDevSdk.DTO.WebHooks.Events;
using Features.Webhook.Dto;
using Features.Webhook.Repositories;
using Features.Webhook.Constants;
using Features.Webhook.Extensions;
using Application.Models.DB.Tables;
using Shared.Jobs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using YahooDeveloperApiClient.YOLP.Request;

namespace Features.Webhook.Services;

public interface IWebhookService
{
    /// <summary>
    /// ジョブログを登録する
    /// </summary>
    /// <param name="job">ジョブ情報</param>
    Task AcceptLocalAsync(LocalJobDto job);

    /// <summary>
    /// キューに溜まっているジョブを処理する
    /// </summary>
    /// <param name="queue">キュー</param>
    /// <param name="ct">キャンセルトークン</param>
    Task PostLocalJobAsync(IBackgroundJobQueue<LocalJobDto> queue, CancellationToken ct);

    /// <summary>
    /// イベントログを登録する
    /// </summary>
    /// <param name="logs">登録するログ一覧</param>
    Task CreateEventLogsAsync(Meta[] logs);

    /// <summary>
    /// ラインフックからのリクエストを受け取り、YahooAPIを実行し返答する
    /// </summary>
    /// <param name="gourmetGettingDto">リクエスト</param>
    /// <param name="genreCode">ジャンルコード</param>
    /// <returns>LineAPIにPostした内容</returns>
    Task<LocalEventResultDto[]> PostLocalAsync(WebHook gourmetGettingDto, string genreCode);
}

/// <summary>
/// Webhookコントローラーのビジネスロジック
/// </summary>
internal class WebhookService
(
    IWebhookRepository webhookRepository,
    IWebHostEnvironment env,
    IConfiguration configuration
) : IWebhookService
{
    /// <summary>
    /// リポジトリ
    /// </summary>
    private IWebhookRepository WebhookRepository { get; } = webhookRepository;

    /// <summary>
    /// 環境情報
    /// </summary>
    private IWebHostEnvironment Env { get; } = env;

    /// <summary>
    /// 設定情報
    /// </summary>
    private IConfiguration Configuration { get; } = configuration;

    /// <summary>
    /// ジョブログを登録する
    /// </summary>
    /// <param name="job">ジョブ情報</param>
    public async Task AcceptLocalAsync(LocalJobDto job)
    {
        var log = new JobLog
        {
            Id = job.Id,
            Contents = job.GetBody(),
        };
        await WebhookRepository.CreateAsync(log);
        await WebhookRepository.SaveChangesAsync();
    }

    /// <summary>
    /// キューに溜まっているジョブを処理する
    /// </summary>
    /// <param name="queue">キュー</param>
    /// <param name="ct">キャンセルトークン</param>
    public async Task PostLocalJobAsync(IBackgroundJobQueue<LocalJobDto> queue, CancellationToken ct)
    {
        await foreach (var job in queue.ReadAllAsync(ct))
        {
            bool isSuccess = true;
            string errorMessage = null;
            try
            {
                var results = await PostLocalAsync(job.WebHook, job.GenreCode);
                await CreateEventLogsAsync([.. results.Select(x => x.Meta)]);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                errorMessage = ex.Message;
            }
            var log = await WebhookRepository.FetchJobLogAsync(job.Id);
            if (log is null) continue;
            log.IsSuccess = isSuccess;
            log.Info = errorMessage;
            WebhookRepository.Update(log);
            await WebhookRepository.SaveChangesAsync();

        }
    }

    /// <summary>
    /// イベントログを登録する
    /// </summary>
    /// <param name="logs">登録するログ一覧</param>
    public async Task CreateEventLogsAsync(Meta[] logs)
    {
        foreach (var log in logs)
        {
            await WebhookRepository.CreateAsync(log);
        }
        await WebhookRepository.SaveChangesAsync();
    }

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
            var gourmet = await WebhookRepository.GetLocalSearchResultAsync(localSearchRequest);
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
            results.ForEach
            (
                async x => await WebhookRepository.PostReplyAsync(x.Reply, Configuration.GetValue<string>("Line:Token"))
            );
        return [.. results];
    }
}
