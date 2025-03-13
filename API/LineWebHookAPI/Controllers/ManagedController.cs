using LineWebHookAPI.Models.DB;
using LineWebHookAPI.Models.BL.HotPeppers;
using LineWebHookAPI.Models.Dto.HotPeppers;
using Microsoft.AspNetCore.Mvc;
using LineWebHookAPI.Configurations;
using LineWebHookAPI.Models.BL.Managed;

namespace LineWebHookAPI.Controllers;

/// <summary>
/// ホットペッパー関連API
/// </summary>
[ApiController]
[Route("api/managed")]
public class ManagedController
(
    MyContext dbContext
) : ControllerBase
{
    /// <summary>
    /// ビジネスロジック
    /// </summary>
    public ManagedBL ManagedBL { get; protected set; } = new ManagedBL(dbContext);

    /// <summary>
    /// LineChatBotの実行ログを取得する
    /// </summary>
    /// <param name="gourmetGettingDto">位置情報</param>
    /// <returns>LineAPIにPostした内容</returns>
    [HttpGet("gourmet")]
    public async Task<IActionResult> GetGourmetLogAsync()
    {
        var res = await ManagedBL.GetLogAsync();
        return Ok(res);
    }
}
