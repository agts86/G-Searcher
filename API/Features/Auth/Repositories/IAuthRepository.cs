using Application.Models.DB.Tables;

namespace Features.Auth.Repositories;

public interface IAuthRepository
{
    /// <summary>
    /// リフレッシュトークンをハッシュ値で取得する
    /// </summary>
    /// <param name="tokenHash">トークンハッシュ値</param>
    /// <returns>リフレッシュトークン</returns>
    Task<RefreshToken> FetchRefreshTokenByHashAsync(string tokenHash);

    /// <summary>
    /// ユーザーの全リフレッシュトークンを無効化する
    /// </summary>
    /// <param name="userName">ユーザー名</param>
    Task RevokeAllRefreshTokensByUserNameAsync(string userName);

    /// <summary>
    /// リフレッシュトークンを無効化する
    /// </summary>
    /// <param name="token">リフレッシュトークン</param>
    void RevokeRefreshToken(RefreshToken token);

    /// <summary>
    /// リフレッシュトークンを登録する
    /// </summary>
    /// <param name="token">リフレッシュトークン</param>
    Task AddRefreshTokenAsync(RefreshToken token);

    /// <summary>
    /// 変更を保存する
    /// </summary>
    Task SaveChangesAsync();
}
