namespace Features.Auth.Dto;

/// <summary>
/// ログイン状態確認レスポンス
/// </summary>
/// <param name="UserName">管理者ユーザー名</param>
public record MeResponseDto(string UserName);
