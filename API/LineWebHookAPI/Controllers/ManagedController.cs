using Microsoft.AspNetCore.Mvc;
using LineWebHookAPI.Models.Services.Managed;
using LineWebHookAPI.Constants;
using LineWebHookAPI.Models.DB.Repositories;
using LineWebHookAPI.Models.Http;

namespace LineWebHookAPI.Controllers;

/// <summary>
/// ホットペッパー関連API
/// </summary>
[ApiController]
[Route("api/managed")]
public class ManagedController(ManagedRepositoryBase managedRepository) : ControllerBase
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
    public async Task<IActionResult> GetErrorLogAsync()
    {
        var res = await ManagedService.GetErrorLogAsync();
        return Ok(res);
    }
}
