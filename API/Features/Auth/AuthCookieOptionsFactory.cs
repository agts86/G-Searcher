using Microsoft.AspNetCore.Http;

namespace Features.Auth;

/// <summary>
/// 認証 Cookie オプション生成
/// </summary>
public static class AuthCookieOptionsFactory
{
    /// <summary>
    /// 認証 Cookie オプションを作成する
    /// </summary>
    /// <param name="expiresAt">有効期限</param>
    /// <returns>Cookie オプション</returns>
    public static CookieOptions Create(DateTimeOffset expiresAt)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/",
            Expires = expiresAt
        };
    }
}
