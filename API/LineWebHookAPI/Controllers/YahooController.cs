using Microsoft.AspNetCore.Mvc;
using LineWebHookAPI.Configurations;
using LineWebHookAPI.Models.Services.Yahoo;
using System.ComponentModel.DataAnnotations;
using LineWebHookAPI.Validations;
using LineWebHookAPI.Models.Dto.Line.Hook;
using LineWebHookAPI.Models.DB.Repositories;

namespace LineWebHookAPI.Controllers;

/// <summary>
/// ホットペッパー関連API
/// </summary>
[ApiController]
[Route("api/yahoo")]
public class YahooController
(
    IYahooService yahooService,
    IHostEnvironment env,
    BaseControllerRepository baseControllerRepository
) : LineWebHookAPIController(baseControllerRepository)
{
    /// <summary>
    /// ビジネスロジック
    /// </summary>
    public IYahooService YahooService { get; protected set; } = yahooService;

    /// <summary>
    /// 環境情報
    /// </summary>
    /// <value></value>
    public IHostEnvironment Env { get; protected set; } = env;

    /// <summary>
    /// ラインフックからの位置情報を受け取り、YahooAPIを実行し返答する
    /// </summary>
    /// <param name="gourmetGettingDto">位置情報</param>
    /// <param name="genreCode">ジャンルコード</param>
    /// <returns>LineAPIにPostした内容</returns>
    [HttpPost("local")]
    [ServiceFilter(typeof(LineSignatureFilter))]
    public async Task<IActionResult> PostLocalAsync
    (
        [FromBody] GourmetGettingDto gourmetGettingDto,
        [FromQuery] [MaxLength(7)] [HalfNumeric] string genreCode)
    {
        if (Env.IsDevelopment())
        {
            var res = await YahooService.PostLocalAsync(gourmetGettingDto,genreCode);
            return Ok(res);
        }
        else
        {
            _ = RunTaskAsync(YahooService.PostLocalAsync(gourmetGettingDto,genreCode));
            return Ok();
        }
    }
}
