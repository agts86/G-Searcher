using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LineWebHookAPI.Models.DB;
using LineWebHookAPI.Models.DB.Tables;
using LineWebHookAPI.Models.Dto.Auth;
using LineWebHookAPI.Models.Exceptions;
using Microsoft.EntityFrameworkCore;
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
    /// リフレッシュトークンで再認証を実行する
    /// </summary>
    /// <param name="refreshToken">リフレッシュトークン</param>
    /// <returns>認証成功時のトークン情報</returns>
    Task<AuthLoginResult> RefreshAsync(string refreshToken);

    /// <summary>
    /// ログアウト時にリフレッシュトークンを無効化する
    /// </summary>
    /// <param name="refreshToken">リフレッシュトークン</param>
    Task LogoutAsync(string refreshToken);

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
/// <param name="AccessToken">アクセストークン</param>
/// <param name="AccessTokenExpiresAt">アクセストークン有効期限</param>
/// <param name="RefreshToken">リフレッシュトークン</param>
/// <param name="RefreshTokenExpiresAt">リフレッシュトークン有効期限</param>
public record AuthLoginResult
(
    string UserName,
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAt
);

/// <summary>
/// 認証サービス
/// </summary>
public class AuthService(IConfiguration configuration, LineWebHookContext dbContext) : IAuthService
{
    /// <summary>
    /// 設定情報
    /// </summary>
    private IConfiguration Configuration { get; } = configuration;

    /// <summary>
    /// DBコンテキスト
    /// </summary>
    private LineWebHookContext DbContext { get; } = dbContext;

    /// <summary>
    /// 管理者ログインを実行する
    /// </summary>
    /// <param name="request">ログイン情報</param>
    /// <returns>認証成功時のトークン情報</returns>
    public async Task<AuthLoginResult> LoginAsync(LoginRequestDto request)
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

        var (accessToken, accessTokenExpiresAt) = CreateAccessToken(request.UserName);
        var (refreshToken, refreshTokenHash, refreshTokenExpiresAt) = CreateRefreshToken();

        await RevokeAllRefreshTokensByUserNameAsync(request.UserName);

        DbContext.RefreshTokens.Add
        (
            new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserName = request.UserName,
                TokenHash = refreshTokenHash,
                ExpiresAt = refreshTokenExpiresAt.UtcDateTime
            }
        );

        await DbContext.SaveChangesAsync();

        return new AuthLoginResult
        (
            request.UserName,
            accessToken,
            accessTokenExpiresAt,
            refreshToken,
            refreshTokenExpiresAt
        );
    }

    /// <summary>
    /// リフレッシュトークンで再認証を実行する
    /// </summary>
    /// <param name="refreshToken">リフレッシュトークン</param>
    /// <returns>認証成功時のトークン情報</returns>
    public async Task<AuthLoginResult> RefreshAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new UnauthorizedException(new ResponseError("Unauthorized."));

        var refreshTokenHash = GetTokenHash(refreshToken);
        var storedToken = await DbContext.RefreshTokens.SingleOrDefaultAsync(x => x.TokenHash == refreshTokenHash);
        if (storedToken is null)
            throw new UnauthorizedException(new ResponseError("Unauthorized."));

        if (storedToken.ExpiresAt <= DateTime.UtcNow)
        {
            DbContext.RefreshTokens.Remove(storedToken);
            await DbContext.SaveChangesAsync();
            throw new UnauthorizedException(new ResponseError("Unauthorized."));
        }

        var (accessToken, accessTokenExpiresAt) = CreateAccessToken(storedToken.UserName);
        var (newRefreshToken, newRefreshTokenHash, refreshTokenExpiresAt) = CreateRefreshToken();

        DbContext.RefreshTokens.Remove(storedToken);
        DbContext.RefreshTokens.Add
        (
            new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserName = storedToken.UserName,
                TokenHash = newRefreshTokenHash,
                ExpiresAt = refreshTokenExpiresAt.UtcDateTime
            }
        );
        await DbContext.SaveChangesAsync();

        return new AuthLoginResult
        (
            storedToken.UserName,
            accessToken,
            accessTokenExpiresAt,
            newRefreshToken,
            refreshTokenExpiresAt
        );
    }

    /// <summary>
    /// ログアウト時にリフレッシュトークンを無効化する
    /// </summary>
    /// <param name="refreshToken">リフレッシュトークン</param>
    public async Task LogoutAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return;

        var refreshTokenHash = GetTokenHash(refreshToken);
        var storedToken = await DbContext.RefreshTokens.SingleOrDefaultAsync(x => x.TokenHash == refreshTokenHash);
        if (storedToken is null)
            return;

        DbContext.RefreshTokens.Remove(storedToken);
        await DbContext.SaveChangesAsync();
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

    /// <summary>
    /// アクセストークンを作成する
    /// </summary>
    /// <param name="userName">ユーザー名</param>
    /// <returns>アクセストークン情報</returns>
    private (string AccessToken, DateTimeOffset ExpiresAt) CreateAccessToken(string userName)
    {
        var jwtKey = Configuration.GetValue<string>("Auth:JwtKey") ?? string.Empty;
        const int MinimumJwtKeyLength = 32;
        if (jwtKey.Length < MinimumJwtKeyLength)
            throw new InvalidOperationException("Auth:JwtKey must be at least 32 characters.");

        var issuer = Configuration.GetValue<string>("Auth:Issuer");
        var audience = Configuration.GetValue<string>("Auth:Audience");
        var expiresMinutes = Configuration.GetValue<int?>("Auth:ExpiresMinutes") ?? 15;
        if (expiresMinutes <= 0)
            expiresMinutes = 15;

        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(expiresMinutes);
        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, userName),
            new (ClaimTypes.Name, userName),
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
        return (tokenHandler.WriteToken(token), expiresAt);
    }

    /// <summary>
    /// リフレッシュトークンを作成する
    /// </summary>
    /// <returns>リフレッシュトークン情報</returns>
    private (string RefreshToken, string TokenHash, DateTimeOffset ExpiresAt) CreateRefreshToken()
    {
        var refreshExpiresDays = Configuration.GetValue<int?>("Auth:RefreshExpiresDays") ?? 7;
        if (refreshExpiresDays <= 0)
            refreshExpiresDays = 7;

        var refreshToken = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(64));
        var tokenHash = GetTokenHash(refreshToken);
        var expiresAt = DateTimeOffset.UtcNow.AddDays(refreshExpiresDays);

        return (refreshToken, tokenHash, expiresAt);
    }

    /// <summary>
    /// トークンハッシュ値を取得する
    /// </summary>
    /// <param name="token">トークン</param>
    /// <returns>ハッシュ値</returns>
    private static string GetTokenHash(string token)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hash);
    }

    /// <summary>
    /// ユーザーの既存リフレッシュトークンを無効化する
    /// </summary>
    /// <param name="userName">ユーザー名</param>
    private async Task RevokeAllRefreshTokensByUserNameAsync(string userName)
    {
        var tokens = await DbContext.RefreshTokens.Where(x => x.UserName == userName).ToListAsync();
        if (tokens.Count == 0)
            return;

        DbContext.RefreshTokens.RemoveRange(tokens);
    }
}
