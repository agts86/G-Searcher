namespace LineWebHookAPI.Models.Dto.Auth;

/// <summary>
/// ログインレスポンス
/// </summary>
/// <param name="UserName">管理者ユーザー名</param>
/// <param name="ExpiresAt">トークン有効期限</param>
public record LoginResponseDto(string UserName, DateTimeOffset ExpiresAt);
