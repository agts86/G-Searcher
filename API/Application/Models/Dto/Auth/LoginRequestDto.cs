using System.ComponentModel.DataAnnotations;

namespace Application.Models.Dto.Auth;

/// <summary>
/// ログインリクエスト
/// </summary>
public class LoginRequestDto
{
    /// <summary>
    /// 管理者ユーザー名
    /// </summary>
    [Required]
    [MinLength(1)]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// パスワード
    /// </summary>
    [Required]
    [MinLength(1)]
    public string Password { get; set; } = string.Empty;
}
