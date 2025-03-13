using LineWebHookAPI.Models.DB;
using LineWebHookAPI.Models.BL.HotPeppers;
using LineWebHookAPI.Models.Dto.HotPeppers;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using LineWebHookAPI.Configurations;

namespace LineWebHookAPI.Controllers;

/// <summary>
/// ホットペッパー関連API
/// </summary>
[ApiController]
[Route("api/hot-pepper")]
public class HotPepperController
(
    IConfiguration configuration,
    MyContext dbContext,
    IHostEnvironment env
) : ControllerBase
{
    /// <summary>
    /// ビジネスロジック
    /// </summary>
    public HotPepperBL HotPepperBL { get; protected set; } = new HotPepperBL(configuration,dbContext,env);

    /// <summary>
    /// 環境情報
    /// </summary>
    /// <value></value>
    public IHostEnvironment Env { get; protected set; } = env;

    /// <summary>
    /// ラインフックからの位置情報を受け取り、ホットペッパーAPIを実行し返答する
    /// </summary>
    /// <param name="gourmetGettingDto">位置情報</param>
    /// <returns>LineAPIにPostした内容</returns>
    [HttpPost("gourmet/location")]
    [ServiceFilter(typeof(LineSignatureFilter))]
    public async Task<IActionResult> PostGourmetLocationAsync([FromBody] GourmetGettingDto gourmetGettingDto)
    {
        Console.WriteLine(JsonSerializer.Serialize(gourmetGettingDto));
        if (Env.IsDevelopment())
        {
            var res = await HotPepperBL.PostGourmetLocationAsync(gourmetGettingDto);
            return Ok(res);
        }
        else
        {
            _ = HotPepperBL.PostGourmetLocationAsync(gourmetGettingDto);
            return Ok();
        }
        
    }
}
