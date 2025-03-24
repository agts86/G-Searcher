using LineWebHookAPI.Models.DB;
using Microsoft.AspNetCore.Mvc;
using LineWebHookAPI.Models.BL.Managed;

namespace LineWebHookAPI.Controllers;

/// <summary>
/// ホットペッパー関連API
/// </summary>
[ApiController]
[Route("api/managed")]
public class ManagedController
(
    LineWebHookContext dbContext
) : ControllerBase
{
    /// <summary>
    /// ビジネスロジック
    /// </summary>
    public ManagedBL ManagedBL { get; protected set; } = new ManagedBL(dbContext);

    /// <summary>
    /// LineChatBotの実行ログを取得する
    /// </summary>
    /// <returns>LineAPIにPostした内容</returns>
    [HttpGet("gourmet")]
    public async Task<IActionResult> GetGourmetLogAsync()
    {
        var res = await ManagedBL.GetGourmetLogAsync();
        return Ok(res);
    }

    /// <summary>
    /// エラーログを取得する
    /// </summary>
    /// <returns>エラーログ</returns>
    [HttpGet("error-log")]
    public async Task<IActionResult> GetErrorLogAsync()
    {
        var res = await ManagedBL.GetErrorLogAsync();
        return Ok(res);
    }
}
