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
    /// <param name="context">HTTP 要求</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await Next(context);
        }
        catch(StatusCodeException ex)
        {
            await ResponseErrorAsync(context,ex.StatusCode,ex.Error);
        }
        catch(Exception)
        {
            var error = new ResponseError("An error occurred while processing your request.");
            await ResponseErrorAsync(context,HttpStatusCode.InternalServerError,error);
        }
    }

    /// <summary>
    /// レスポンスを返す
    /// </summary>
    /// <param name="context"></param>
    /// <param name="statusCode"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    private static async Task ResponseErrorAsync(HttpContext context, HttpStatusCode statusCode, ResponseError error)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsJsonAsync(error);
    }
}
