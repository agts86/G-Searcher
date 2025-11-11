using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using LineDevSdk.Configurations;
using LineDevSdk.DTOs.MessagingAPIs;
using LineDevSdk.DTOs.WebHooks;
using LineWebHookAPI.Models.Dto.Yahoo;
using LineWebHookAPI.Models.Job;
using LineWebHookAPI.Models.Services;
using LineWebHookAPI.Validations;
using Microsoft.AspNetCore.Mvc;

namespace LineWebHookAPI.Controllers;

/// <summary>
/// Yahoo関連API
/// </summary>
[ApiVersion("1")]
[ApiController]
[Route("api/v{version:apiVersion}/yahoo")]
public class YahooController
(
    IYahooService yahooService
) : ControllerBase
{
    /// <summary>
    /// ビジネスロジック
    /// </summary>
    private IYahooService YahooService { get; } = yahooService;

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
        [FromServices] IBackgroundJobQueue<LocalJobDto> queue,
        CancellationToken ct
    )
    {
        var job = new LocalJobDto
        (
            gourmetGettingDto,
            genreCode
        );
        await queue.EnqueueAsync(job, ct);

        await YahooService.AcceptLocalAsync(job);
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
        var res = await YahooService.PostLocalAsync(gourmetGettingDto, genreCode);
        return res;
    }
}
