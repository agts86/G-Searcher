using Microsoft.AspNetCore.Mvc;
using LineWebHookAPI.Models.Services.Managed;
using LineWebHookAPI.Models.DB.Repositories;

namespace LineWebHookAPI.Controllers;

/// <summary>
/// ホットペッパー関連API
/// </summary>
[ApiController]
[Route("api/managed")]
public class ManagedController
(
    IManagedService managedService,
    BaseControllerRepository baseControllerRepository
) : LineWebHookAPIController(baseControllerRepository)
{
    /// <summary>
    /// ビジネスロジック
    /// </summary>
    public IManagedService ManagedService { get; protected set; } = managedService;

    /// <summary>
    /// LineChatBotの実行ログを取得する
    /// </summary>
    /// <returns>LineAPIにPostした内容</returns>
    [HttpGet("gourmet")]
    public async Task<IActionResult> GetGourmetLogAsync()
    {
        var res = await ManagedService.GetGourmetLogAsync();
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
