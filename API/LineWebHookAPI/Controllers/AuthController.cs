using Asp.Versioning;
using LineWebHookAPI.Configurations;
using LineWebHookAPI.Constants.Auth;
using LineWebHookAPI.Models.Dto.Auth;
using LineWebHookAPI.Models.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LineWebHookAPI.Controllers;

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
            result.Token,
            AuthCookieOptionsFactory.Create(result.ExpiresAt)
        );

        return new LoginResponseDto(result.UserName, result.ExpiresAt);
    }

    /// <summary>
    /// 管理者ログアウトを実行する
    /// </summary>
    /// <returns>No Content</returns>
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(AuthCookie.Name, AuthCookieOptionsFactory.Create(DateTimeOffset.UtcNow.AddDays(-1)));
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
