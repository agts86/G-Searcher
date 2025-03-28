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
    /// <summary>
    /// リポジトリ
    /// </summary>
    private BaseControllerRepository BaseControllerRepository { get; } = new BaseControllerRepository(dbContext);

    /// <summary>
    /// awaitせずにエラーハンドリングしながらタスクを実行する
    /// </summary>
    /// <param name="task">非同期メソッド</param>
    /// <typeparam name="T">中身</typeparam>
    protected async Task RunTaskAsync(Task task)
    {
        try
        {
            await task;
        }
        catch(StatusCodeException ex)
        {
            await BaseControllerRepository.ErrorLogDao.CreateLogAsync(ex.Error);
            await BaseControllerRepository.SaveChangesAsync();
        }
        catch (Exception e)
        {
            await BaseControllerRepository.CreateErrorLogAsync(e);
            await BaseControllerRepository.SaveChangesAsync();
        }
    }
}
