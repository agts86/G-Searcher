using LineWebHookAPI.Models.DB.Repositories;
using LineWebHookAPI.Constants.Line.API;
using LineDevSdk.DTOs.MessagingAPIs;
using LineDevSdk.DTOs.Commons.Messages.Templates;
using LineDevSdk.Dtos.Commons.Messages;
using LineDevSdk.DTOs.Commons.Messages;
using LineDevSdk.DTOs.WebHooks;
using LineDevSdk.DTOs.WebHooks.Events;
using YahooDeveloperApiClient.YOLP.Request;
using LineWebHookAPI.Extensions;
using LineWebHookAPI.Models.Dto.Yahoo;
using LineWebHookAPI.Models.DB.Tables;
using LineWebHookAPI.Models.Job;

namespace LineWebHookAPI.Models.Services;

public interface IYahooService
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
    /// ラインフックからのリクエストを受け取り、YahooAPIを実行し返答する
    /// </summary>
    /// <param name="gourmetGettingDto">リクエスト</param>
    /// <param name="genreCode">ジャンルコード</param>
    /// <returns>LineAPIにPostした内容</returns>
    Task<Reply[]> PostLocalAsync(WebHook gourmetGettingDto, string genreCode);
}

/// <summary>
/// YahooBコントローラーのビジネスロジック
/// </summary>
public class YahooService 
(
    IYahooRepository yahooRepository,
    IWebHostEnvironment env,
    IConfiguration configuration
): IYahooService
{
    /// <summary>
    /// リポジトリ
    /// </summary>
    private IYahooRepository YahooRepository { get; } = yahooRepository;

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
        await YahooRepository.CreateAsync(log);
        await YahooRepository.SaveChangesAsync();
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
                await PostLocalAsync(job.WebHook, job.GenreCode);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                errorMessage = ex.Message;
            }
            var log = await YahooRepository.FetchJobLogAsync(job.Id);
            if(log is null) continue;
            log.IsSuccess = isSuccess;
            log.Info = errorMessage;
            YahooRepository.Update(log);
            await YahooRepository.SaveChangesAsync();
            
        }
    }

    /// <summary>
    /// ラインフックからのリクエストを受け取り、YahooAPIを実行し返答する
    /// </summary>
    /// <param name="gourmetGettingDto">リクエスト</param>
    /// <returns>LineAPIにPostした内容</returns>
    public async Task<Reply[]> PostLocalAsync(WebHook gourmetGettingDto, string genreCode)
    {
        var replies = new List<Reply>();

        foreach (var e in gourmetGettingDto.Events ?? [])
        {
            if (e is not MessageEvent messageEvent) continue;

            var log = messageEvent.Message.ConvertGourmetLog();
            await YahooRepository.CreateAsync(log);
            await YahooRepository.SaveChangesAsync();

            var localSearchRequest = new LocalSearchRequest()
            {
                Dist = 1,
                Results = 20,
                GenreCode = genreCode,
                Detail = LocalSearchDetail.Full
            };
            localSearchRequest.MergeMessageInfo(messageEvent.Message);
            var gourmet = await YahooRepository.GetLocalSearchResultAsync(localSearchRequest);
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
                async x => await YahooRepository.PostReplyAsync(x, Configuration.GetValue<string>("Line:Token"))
            );
        return [.. replies];
    }
}

