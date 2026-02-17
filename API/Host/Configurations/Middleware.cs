using System.Diagnostics.CodeAnalysis;
using System.Net;
using Shared.Exceptions;
using Shared.Repositories;
using Microsoft.AspNetCore.Http;

namespace Host.Configurations;

/// <summary>
/// ミドルウェア
/// 例外はここで全て処理する
/// </summary>
[ExcludeFromCodeCoverage]
public class Middleware(IMiddleWareRepository repository) : IMiddleware
{
    /// <summary>
    /// ミドルウェア用リポジトリ
    /// </summary>
    public IMiddleWareRepository Repository { get; } = repository;

    /// <summary>
    /// 例外処理
    /// </summary>
    /// <param name="httpContext">HTTP 要求</param>
    /// <param name="next">HTTP 要求を処理できる関数</param>
    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        try
        {
            // Line署名検証でFromBody以外でも使うので再読み込み可能にしておく
            httpContext.Request.EnableBuffering();
            await next(httpContext);
        }
        catch (StatusCodeException ex)
        {
            await Repository.CreateErrorLogAsync(ex.Error);
            await Repository.SaveChangesAsync();
            await ResponseErrorAsync(httpContext, ex.StatusCode, ex.Error);
        }
        catch (Exception ex)
        {
            await Repository.CreateErrorLogAsync(ex);
            await Repository.SaveChangesAsync();
            var error = new ResponseError("An error occurred while processing your request.");
            await ResponseErrorAsync(httpContext, HttpStatusCode.InternalServerError, error);
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
