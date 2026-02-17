using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using LineDevSdk.Configurations;
using LineDevSdk.DTO.MessagingAPIs;
using LineDevSdk.DTO.WebHooks;
using Features.Webhook.Dto;
using Shared.Jobs;
using Features.Webhook.Services;
using Shared.Validations;
using Microsoft.AspNetCore.Mvc;

namespace Features.Webhook;

/// <summary>
/// Webhook関連API
/// </summary>
[ApiVersion("1")]
[ApiController]
[Route("api/v{version:apiVersion}/webhook")]
public class WebhookController
(
    IWebhookService webhookService
) : ControllerBase
{
    /// <summary>
    /// ビジネスロジック
    /// </summary>
    private IWebhookService WebhookService { get; } = webhookService;

    /// <summary>
    /// ラインフックからの位置情報を受け取り、ジョブキューに登録する
    /// </summary>
    /// <param name="gourmetGettingDto">位置情報</param>
    /// <param name="genreCode">ジャンルコード</param>
    /// <param name="queue">ジョブキュー</param>
    /// <returns></returns>
    [HttpPost("local/accept")]
    [ServiceFilter(typeof(LineSignatureFilter))]
    public async Task<IActionResult> AcceptLocalAsync
    (
        [FromBody] WebHook gourmetGettingDto,
        [FromQuery][MaxLength(7)][HalfNumeric] string genreCode,
        [FromServices] IBackgroundJobQueue<LocalJobDto> queue
    )
    {
        var job = new LocalJobDto
        (
            gourmetGettingDto,
            genreCode
        );
        await queue.EnqueueAsync(job);

        await WebhookService.AcceptLocalAsync(job);
        return Accepted(new { job.Id });
    }

    /// <summary>
    /// ラインフックからの位置情報を受け取り、YahooAPIを実行し返答する
    /// </summary>
    /// <param name="gourmetGettingDto">位置情報</param>
    /// <param name="genreCode">ジャンルコード</param>
    /// <returns>LineAPIにPostした内容</returns>
    [HttpPost("local")]
    [ServiceFilter(typeof(LineSignatureFilter))]
    public async Task<ActionResult<Reply[]>> PostLocalAsync
    (
        [FromBody] WebHook gourmetGettingDto,
        [FromQuery][MaxLength(7)][HalfNumeric] string genreCode)
    {
        var res = await WebhookService.PostLocalAsync(gourmetGettingDto, genreCode);
        return res;
    }
}
