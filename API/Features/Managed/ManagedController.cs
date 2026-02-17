using Microsoft.AspNetCore.Mvc;
using Features.Managed.Constants;
using Features.Managed.Services;
using Application.Models.DB.Tables;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;


namespace Features.Managed;

/// <summary>
/// ログ取得用関連API
/// </summary>
[ApiVersion("1")]
[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/managed")]
public class ManagedController(IManagedService managedService) : ControllerBase
{
    /// <summary>
    /// ビジネスロジック
    /// </summary>
    private IManagedService ManagedService { get; } = managedService;

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

    /// <summary>
    /// ジョブログを取得する
    /// </summary>
    /// <returns>ジョブログ</returns>
    [HttpGet("job-log")]
    public async Task<ActionResult<JobLog[]>> GetJobLogAsync()
    {
        var res = await ManagedService.GetJobLogAsync();
        return res;
    }
}
