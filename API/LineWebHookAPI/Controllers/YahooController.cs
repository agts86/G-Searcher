using Microsoft.AspNetCore.Mvc;
using LineWebHookAPI.Configurations;
using LineWebHookAPI.Models.Services.Yahoo;
using System.ComponentModel.DataAnnotations;
using LineWebHookAPI.Validations;
using LineWebHookAPI.Models.Dto.Line.Hook;

namespace LineWebHookAPI.Controllers;

/// <summary>
/// Yahoo関連API
/// </summary>
[ApiController]
[Route("api/yahoo")]
public class YahooController(IYahooService yahooService) : ControllerBase
{
    /// <summary>
    /// ビジネスロジック
    /// </summary>
    public IYahooService YahooService { get; protected set; } = yahooService;

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
        var res = await YahooService.PostLocalAsync(gourmetGettingDto,genreCode);
        return Ok(res);
    }
}
