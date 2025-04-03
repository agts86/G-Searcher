using LineWebHookAPI.Models.Services.HotPeppers;
using Microsoft.AspNetCore.Mvc;
using LineWebHookAPI.Configurations;
using LineWebHookAPI.Constants.HotPepper;
using LineWebHookAPI.Models.Dto.Line.Hook;

namespace LineWebHookAPI.Controllers;

/// <summary>
/// ホットペッパー関連API
/// </summary>
[ApiController]
[Route("api/hot-pepper")]
public class HotPepperController(IHotPepperService hotPepperService) : ControllerBase
{
    /// <summary>
    /// ビジネスロジック
    /// </summary>
    public IHotPepperService HotPepperService { get; protected set; } = hotPepperService;

    /// <summary>
    /// ラインフックからの位置情報を受け取り、ホットペッパーAPIを実行し返答する
    /// </summary>
    /// <param name="gourmetGettingDto">位置情報</param>
    /// <param name="genreCode">ジャンルコード</param>
    /// <returns>LineAPIにPostした内容</returns>
    [HttpPost("gourmet/location")]
    [ServiceFilter(typeof(LineSignatureFilter))]
    public async Task<IActionResult> PostGourmetLocationAsync([FromBody] GourmetGettingDto gourmetGettingDto,[FromQuery] GenreCode genreCode)
    {
        var res = await HotPepperService.PostGourmetLocationAsync(gourmetGettingDto,genreCode);
        return Ok(res);
    }
}
