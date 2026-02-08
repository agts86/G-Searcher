using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LineWebHookAPI.Models.Dto.Auth;
using LineWebHookAPI.Models.Exceptions;
using Microsoft.IdentityModel.Tokens;

namespace LineWebHookAPI.Models.Services;

public interface IAuthService
{
    /// <summary>
    /// 管理者ログインを実行する
    /// </summary>
    /// <param name="request">ログイン情報</param>
    /// <returns>認証成功時のトークン情報</returns>
    Task<AuthLoginResult> LoginAsync(LoginRequestDto request);

    /// <summary>
    /// 認証済みユーザー名を取得する
    /// </summary>
    /// <param name="user">認証情報</param>
    /// <returns>ユーザー名</returns>
    string GetCurrentUserName(ClaimsPrincipal user);
}

/// <summary>
/// ログイン成功時の情報
/// </summary>
/// <param name="UserName">管理者ユーザー名</param>
/// <param name="Token">JWT トークン</param>
/// <param name="ExpiresAt">トークン有効期限</param>
public record AuthLoginResult(string UserName, string Token, DateTimeOffset ExpiresAt);

/// <summary>
/// 認証サービス
/// </summary>
public class AuthService(IConfiguration configuration) : IAuthService
{
    /// <summary>
    /// 設定情報
    /// </summary>
    private IConfiguration Configuration { get; } = configuration;

    /// <summary>
    /// 管理者ログインを実行する
    /// </summary>
    /// <param name="request">ログイン情報</param>
    /// <returns>認証成功時のトークン情報</returns>
    public Task<AuthLoginResult> LoginAsync(LoginRequestDto request)
    {
        var adminUserName = Configuration.GetValue<string>("Auth:AdminUserName");
        var adminPassword = Configuration.GetValue<string>("Auth:AdminPassword");
        var isFailure = new (string Expected, string Actual)[]
        {
            (adminUserName, request.UserName),
            (adminPassword, request.Password)
        }
        .Any(x => !IsMatch(x.Expected, x.Actual));
        if (isFailure)
            throw new UnauthorizedException(new ResponseError("Invalid user name or password."));
        
        var jwtKey = Configuration.GetValue<string>("Auth:JwtKey") ?? string.Empty;
        const int MinimumJwtKeyLength = 32;
        if (jwtKey.Length < MinimumJwtKeyLength)
            throw new InvalidOperationException("Auth:JwtKey must be at least 32 characters.");

        var issuer = Configuration.GetValue<string>("Auth:Issuer");
        var audience = Configuration.GetValue<string>("Auth:Audience");
        var expiresMinutes = Configuration.GetValue<int?>("Auth:ExpiresMinutes") ?? 120;
        if (expiresMinutes <= 0)
            expiresMinutes = 120;

        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(expiresMinutes);
        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, request.UserName),
            new (ClaimTypes.Name, request.UserName),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken
        (
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        return Task.FromResult(new AuthLoginResult
        (
            request.UserName,
            tokenHandler.WriteToken(token),
            expiresAt
        ));
    }

    /// <summary>
    /// 認証済みユーザー名を取得する
    /// </summary>
    /// <param name="user">認証情報</param>
    /// <returns>ユーザー名</returns>
    public string GetCurrentUserName(ClaimsPrincipal user)
    {
        var userName = user.FindFirstValue(ClaimTypes.Name);
        if (string.IsNullOrWhiteSpace(userName))
            throw new UnauthorizedException(new ResponseError("Unauthorized."));
        return userName;
    }

    /// <summary>
    /// 固定長比較で照合する
    /// </summary>
    /// <param name="left">設定値</param>
    /// <param name="right">入力値</param>
    /// <returns>一致可否</returns>
    private static bool IsMatch(string left, string right)
    {
        var leftLight = new string[] {left, right};
        if(leftLight.Any(string.IsNullOrWhiteSpace))
            return false;   

        var leftBytes = Encoding.UTF8.GetBytes(left);
        var rightBytes = Encoding.UTF8.GetBytes(right);
        if (leftBytes.Length != rightBytes.Length)
         return false;

        return CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
    }
}
