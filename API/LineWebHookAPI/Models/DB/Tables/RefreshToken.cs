using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LineWebHookAPI.Models.DB.Tables;

/// <summary>
/// リフレッシュトークン
/// </summary>
[Table("RefreshToken")]
public class RefreshToken : Meta
{
    /// <summary>
    /// 主キー
    /// </summary>
    [Key]
    [Column("Id")]
    public Guid Id { get; set; }

    /// <summary>
    /// ユーザー名
    /// </summary>
    [Column("UserName")]
    [Required]
    public string UserName { get; set; }

    /// <summary>
    /// リフレッシュトークンのハッシュ値
    /// </summary>
    [Column("TokenHash")]
    [Required]
    public string TokenHash { get; set; }

    /// <summary>
    /// 有効期限
    /// </summary>
    [Column("ExpiresAt")]
    [Required]
    public DateTime ExpiresAt { get; set; }
}
