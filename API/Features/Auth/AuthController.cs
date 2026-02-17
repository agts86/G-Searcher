using Asp.Versioning;
using Features.Auth.Constants;
using Features.Auth.Dtos;
using Features.Auth.Services;
using Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Features.Auth;

/// <summary>
/// 認証関連 API
/// </summary>
[ApiVersion("1")]
[ApiController]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// ビジネスロジック
    /// </summary>
    private IAuthService AuthService { get; } = authService;

    /// <summary>
    /// 管理者ログインを実行する
    /// </summary>
    /// <param name="request">ログイン情報</param>
    /// <returns>ログイン結果</returns>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> LoginAsync([FromBody] LoginRequestDto request)
    {
        var result = await AuthService.LoginAsync(request);

        Response.Cookies.Append
        (
            AuthCookie.Name,
            result.AccessToken,
            AuthCookieOptionsFactory.Create(result.AccessTokenExpiresAt)
        );
        Response.Cookies.Append
        (
            AuthCookie.RefreshName,
            result.RefreshToken,
            AuthCookieOptionsFactory.Create(result.RefreshTokenExpiresAt)
        );

        return new LoginResponseDto(result.UserName, result.AccessTokenExpiresAt);
    }

    /// <summary>
    /// リフレッシュトークンで再認証を実行する
    /// </summary>
    /// <returns>ログイン結果</returns>
    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponseDto>> RefreshAsync()
    {
        var hasRefreshToken = Request.Cookies.TryGetValue(AuthCookie.RefreshName, out var refreshToken);
        if (!hasRefreshToken)
            throw new UnauthorizedException(new ResponseError("Unauthorized."));

        var result = await AuthService.RefreshAsync(refreshToken);

        Response.Cookies.Append
        (
            AuthCookie.Name,
            result.AccessToken,
            AuthCookieOptionsFactory.Create(result.AccessTokenExpiresAt)
        );
        Response.Cookies.Append
        (
            AuthCookie.RefreshName,
            result.RefreshToken,
            AuthCookieOptionsFactory.Create(result.RefreshTokenExpiresAt)
        );

        return new LoginResponseDto(result.UserName, result.AccessTokenExpiresAt);
    }

    /// <summary>
    /// 管理者ログアウトを実行する
    /// </summary>
    /// <returns>No Content</returns>
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        Request.Cookies.TryGetValue(AuthCookie.RefreshName, out var refreshToken);
        await AuthService.LogoutAsync(refreshToken);

        Response.Cookies.Delete(AuthCookie.Name, AuthCookieOptionsFactory.Create(DateTimeOffset.UtcNow.AddDays(-1)));
        Response.Cookies.Delete(AuthCookie.RefreshName, AuthCookieOptionsFactory.Create(DateTimeOffset.UtcNow.AddDays(-1)));
        return NoContent();
    }

    /// <summary>
    /// ログイン状態を確認する
    /// </summary>
    /// <returns>ログインユーザー情報</returns>
    [Authorize]
    [HttpGet("me")]
    public ActionResult<MeResponseDto> GetMe()
    {
        var userName = AuthService.GetCurrentUserName(User);
        return new MeResponseDto(userName);
    }
}
