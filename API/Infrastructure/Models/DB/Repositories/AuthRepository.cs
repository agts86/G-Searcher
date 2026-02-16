using Application.Models.DB.Tables;
using Application.Models.DB.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models.DB.Repositories;

/// <summary>
/// 認証用リポジトリ
/// </summary>
public class AuthRepository(LineWebHookContext dbContext) : IAuthRepository
{
    /// <summary>
    /// EFCoreのコンテキスト
    /// </summary>
    private LineWebHookContext DbContext { get; } = dbContext;

    /// <summary>
    /// リフレッシュトークンをハッシュ値で取得する
    /// </summary>
    /// <param name="tokenHash">トークンハッシュ値</param>
    /// <returns>リフレッシュトークン</returns>
    public async Task<RefreshToken> FetchRefreshTokenByHashAsync(string tokenHash)
    {
        return await DbContext.RefreshTokens.SingleOrDefaultAsync(x => x.TokenHash == tokenHash);
    }

    /// <summary>
    /// ユーザーの全リフレッシュトークンを無効化する
    /// </summary>
    /// <param name="userName">ユーザー名</param>
    public async Task RevokeAllRefreshTokensByUserNameAsync(string userName)
    {
        var tokens = await DbContext.RefreshTokens.Where(x => x.UserName == userName).ToListAsync();
        if (tokens.Count == 0)
            return;

        DbContext.RefreshTokens.RemoveRange(tokens);
    }

    /// <summary>
    /// リフレッシュトークンを無効化する
    /// </summary>
    /// <param name="token">リフレッシュトークン</param>
    public void RevokeRefreshToken(RefreshToken token)
    {
        DbContext.RefreshTokens.Remove(token);
    }

    /// <summary>
    /// リフレッシュトークンを登録する
    /// </summary>
    /// <param name="token">リフレッシュトークン</param>
    public async Task AddRefreshTokenAsync(RefreshToken token)
    {
        await DbContext.RefreshTokens.AddAsync(token);
    }

    /// <summary>
    /// 変更を保存する
    /// </summary>
    public async Task SaveChangesAsync()
    {
        await DbContext.SaveChangesAsync();
    }
}
