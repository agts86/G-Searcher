using System.Net;
using LineWebHookAPI.Models.DB;
using LineWebHookAPI.Models.DB.Repositories;
using LineWebHookAPI.Models.Exceptions;

namespace LineWebHookAPI.Configurations;

/// <summary>
/// ミドルウェア
/// 例外はここで全て処理する
/// </summary>
public class Middleware(MyContext dbContext) : IMiddleware
{
    /// <summary>
    /// ミドルウェア用リポジトリ
    /// </summary>
    public MiddleWareRepository Repository { get; } = new MiddleWareRepository(dbContext);

    /// <summary>
    /// 例外処理
    /// </summary>
    /// <param name="httpContext">HTTP 要求</param>
    /// <param name="next">HTTP 要求を処理できる関数</param>
    public async Task InvokeAsync(HttpContext httpContext,RequestDelegate next)
    {
        try
        {
            await next(httpContext);
        }
        catch(StatusCodeException ex)
        {
            await Repository.ErrorLogDao.CreateLogAsync(ex.Error);
            await Repository.SaveChangesAsync();
            await ResponseErrorAsync(httpContext,ex.StatusCode,ex.Error);
        }
        catch(Exception ex)
        {
            var realError = new ResponseError(ex.Message);
            await Repository.ErrorLogDao.CreateLogAsync(realError);
            await Repository.SaveChangesAsync();
            var error = new ResponseError("An error occurred while processing your request.");
            await ResponseErrorAsync(httpContext,HttpStatusCode.InternalServerError,error);
        }
    }

    /// <summary>
    /// レスポンスを返す
    /// </summary>
    /// <param name="httpContext">HTTP 要求</param>
    /// <param name="statusCode">ステータスコード</param>
    /// <param name="error">エラー情報</param>
    private static async Task ResponseErrorAsync(HttpContext httpContext, HttpStatusCode statusCode, ResponseError error)
    {
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = (int)statusCode;
        await httpContext.Response.WriteAsJsonAsync(error);
    }
}
