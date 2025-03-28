using LineWebHookAPI.Models.DB;
using LineWebHookAPI.Models.DB.Repositories;
using LineWebHookAPI.Models.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace LineWebHookAPI.Controllers;

/// <summary>
/// APIコントローラーの基底クラス
/// 複雑な処理はしないので特別にRepository直呼びを許可
/// </summary>
public abstract class LineWebHookAPIController(LineWebHookContext dbContext) : ControllerBase
{
    protected BaseControllerRepository BaseControllerRepository { get; } = new BaseControllerRepository(dbContext);

    /// <summary>
    /// エラーログを作成する
    /// </summary>
    /// <param name="ex">例外</param>
    protected async Task CreateErrorLogAsync(Exception ex)
    {
        var responseError = new ResponseError(ex.Message);
        await BaseControllerRepository.ErrorLogDao.CreateLogAsync(responseError);
        await BaseControllerRepository.SaveChangesAsync();
    }
}
