using System.Net;
using G_Searcher.Models.Exceptions;

namespace G_Searcher.Configurations;

/// <summary>
/// ミドルウェア
/// 例外はここで全て処理する
/// </summary>
public class Middleware(RequestDelegate next)
{
    /// <summary>
    ///  HTTP 要求を処理できる関数
    /// </summary>
    public RequestDelegate Next { get; } = next;

    /// <summary>
    /// 例外処理
    /// </summary>
    /// <param name="httpContext">HTTP 要求</param>
    public async Task InvokeAsync(HttphttpContext httpContext)
    {
        try
        {
            await Next(httpContext);
        }
        catch(StatusCodeException ex)
        {
            await ResponseErrorAsync(httpContext,ex.StatusCode,ex.Error);
        }
        catch(Exception)
        {
            var error = new ResponseError("An error occurred while processing your request.");
            await ResponseErrorAsync(httpContext,HttpStatusCode.InternalServerError,error);
        }
    }

    /// <summary>
    /// レスポンスを返す
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="statusCode"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    private static async Task ResponseErrorAsync(HttphttpContext httpContext, HttpStatusCode statusCode, ResponseError error)
    {
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = (int)statusCode;
        await httpContext.Response.WriteAsJsonAsync(error);
    }
}
