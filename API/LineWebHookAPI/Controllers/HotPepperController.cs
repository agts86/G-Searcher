using LineWebHookAPI.Models.DB;
using LineWebHookAPI.Models.Services.HotPeppers;
using Microsoft.AspNetCore.Mvc;
using LineWebHookAPI.Configurations;
using LineWebHookAPI.Models.Http;
using LineWebHookAPI.Constants.HotPepper;
using LineWebHookAPI.Models.Dto.Line.Hook;

namespace LineWebHookAPI.Controllers;

/// <summary>
/// ホットペッパー関連API
/// </summary>
[ApiController]
[Route("api/hot-pepper")]
public class HotPepperController
(
    IConfiguration configuration,
    LineWebHookContext dbContext,
    IHostEnvironment env,
    IHttpAdapter http
) : LineWebHookAPIController(dbContext)
{
    /// <summary>
    /// ビジネスロジック
    /// </summary>
    public HotPepperService HotPepperService { get; protected set; } = new HotPepperService(configuration,dbContext,env,http);

    /// <summary>
    /// 環境情報
    /// </summary>
    /// <value></value>
    public IHostEnvironment Env { get; protected set; } = env;

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
        if (Env.IsDevelopment())
        {
            var res = await HotPepperService.PostGourmetLocationAsync(gourmetGettingDto,genreCode);
            return Ok(res);
        }
        else
        {
            _ = RunTaskAsync(HotPepperService.PostGourmetLocationAsync(gourmetGettingDto,genreCode));
            return Ok();
        }
    }
}
