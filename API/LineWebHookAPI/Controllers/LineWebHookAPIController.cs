using LineWebHookAPI.Models.DB.Repositories;
using LineWebHookAPI.Models.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace LineWebHookAPI.Controllers;

/// <summary>
/// APIコントローラーの基底クラス
/// 複雑な処理はしないので特別にRepository直呼びを許可
/// </summary>
public abstract class LineWebHookAPIController(BaseControllerRepository baseControllerRepository) : ControllerBase
{
    /// <summary>
    /// リポジトリ
    /// </summary>
    private BaseControllerRepository BaseControllerRepository { get; } = baseControllerRepository;

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
            Console.WriteLine(ex.Error);
            // await BaseControllerRepository.CreateErrorLogAsync(ex.Error);
            // await BaseControllerRepository.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            // await BaseControllerRepository.CreateErrorLogAsync(e);
            // await BaseControllerRepository.SaveChangesAsync();
        }
    }
}
