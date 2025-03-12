using LineWebHookAPI.Models.DB;
using LineWebHookAPI.Models.BL.HotPeppers;
using LineWebHookAPI.Models.Dto.HotPeppers;
using Microsoft.AspNetCore.Mvc;
using LineWebHookAPI.Configurations;

namespace LineWebHookAPI.Controllers;

/// <summary>
/// ホットペッパー関連API
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HotPepperController
(
    IConfiguration configuration,
    MyContext dbContext
) : ControllerBase
{
    /// <summary>
    /// ビジネスロジック
    /// </summary>
    public HotPepperBL HotPepperBL { get; protected set; } = new HotPepperBL(configuration,dbContext);

    /// <summary>
    /// ラインフックからの位置情報を受け取り、ホットペッパーAPIを実行し返答する
    /// </summary>
    /// <param name="gourmetGettingDto">位置情報</param>
    /// <returns>LineAPIにPostした内容</returns>
    [HttpPost("Gourmet/location")]
    [ServiceFilter(typeof(LineSignatureFilter))]
    public async Task<IActionResult> PostGourmetLocationAsync([FromBody] GourmetGettingDto gourmetGettingDto)
    {
        var res = await HotPepperBL.PostGourmetLocationAsync(gourmetGettingDto);
        return Ok(res);
    }
}
