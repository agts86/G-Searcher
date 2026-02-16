using Application.Models.Exceptions;

namespace Application.Models.DB.Repositories;

public interface IMiddleWareRepository
{
    /// <summary>
    /// エラーログを作成する
    /// </summary>
    /// <param name="ex">例外</param>
    Task CreateErrorLogAsync(Exception ex);

    /// <summary>
    /// エラーログを作成する
    /// </summary>
    /// <param name="ex">例外</param>
    Task CreateErrorLogAsync(ResponseError error);

    /// <summary>
    /// データベースの変更を保存する
    /// </summary>
    Task SaveChangesAsync();
}
