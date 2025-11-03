using Microsoft.AspNetCore.Mvc;
using LineWebHookAPI.Models.Services;
using LineWebHookAPI.Constants;
using LineWebHookAPI.Models.DB.Repositories;
using LineWebHookAPI.Models.DB.Tables;


namespace LineWebHookAPI.Controllers;

/// <summary>
/// ログ取得用関連API
/// </summary>
[ApiController]
[Route("api/managed")]
public class ManagedController(IManagedRepository managedRepository) : ControllerBase
{
    /// <summary>
    /// ビジネスロジック
    /// </summary>
    public ManagedService ManagedService { get; protected set; } = new ManagedService(managedRepository);

    /// <summary>
    /// LineChatBotの位置情報検索実行ログを取得する
    /// </summary>
    /// <returns>LineAPIにPostした内容</returns>
    [HttpGet("gourmet/{messageType}")]
    public async Task<IActionResult> GetGourmetLogsAsync([FromRoute] MessageTypes messageType)
    {
        var res = await ManagedService.GetGourmetLogsAsync(messageType);
        return Ok(res);
    }

    /// <summary>
    /// エラーログを取得する
    /// </summary>
    /// <returns>エラーログ</returns>
    [HttpGet("error-log")]
    public async Task<ActionResult<ErrorLog[]>> GetErrorLogAsync()
    {
        var res = await ManagedService.GetErrorLogAsync();
        return res;
    }
}
