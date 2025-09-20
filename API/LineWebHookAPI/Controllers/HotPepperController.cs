using LineWebHookAPI.Models.Services;
using Microsoft.AspNetCore.Mvc;
using LineWebHookAPI.Constants.HotPepper;
using LineWebHookAPI.Models.DB.Repositories;
using LineWebHookAPI.Models.Http;
using LineDevSdk.Https;
using LineDevSdk.Configurations;
using LineDevSdk.DTOs.WebHooks;
using LineDevSdk.DTOs.MessagingAPIs;

namespace LineWebHookAPI.Controllers;

/// <summary>
/// ホットペッパー関連API
/// </summary>
[ApiController]
[Route("api/hot-pepper")]
public class HotPepperController
(
    IHotPepperRepository hotPepperRepository,
    IWebHostEnvironment env,
    IHotPepperHttp hotPepperHttp,
    ILineHttp lineHttp,
    IConfiguration configuration
) : ControllerBase
{
    /// <summary>
    /// ビジネスロジック
    /// </summary>
    public HotPepperService HotPepperService { get; protected set; } = new HotPepperService(hotPepperRepository, env, hotPepperHttp, lineHttp, configuration);

    /// <summary>
    /// ラインフックからの位置情報を受け取り、ホットペッパーAPIを実行し返答する
    /// </summary>
    /// <param name="gourmetGettingDto">位置情報</param>
    /// <param name="genreCode">ジャンルコード</param>
    /// <returns>LineAPIにPostした内容</returns>
    [HttpPost("gourmet/location")]
    [ServiceFilter(typeof(LineSignatureFilter))]
    public async Task<ActionResult<Reply[]>> PostGourmetLocationAsync([FromBody] WebHook gourmetGettingDto, [FromQuery] GenreCode genreCode)
    {
        var res = await HotPepperService.PostGourmetLocationAsync(gourmetGettingDto, genreCode);
        return res;
    }
}
