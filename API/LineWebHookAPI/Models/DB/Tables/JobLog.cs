using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LineWebHookAPI.Models.DB.Tables;

/// <summary>
/// ジョブログ
/// </summary>
[Table("JobLog")]
public class JobLog : Meta
{
    /// <summary>
    /// 主キー
    /// </summary>
    [Key]
    [Column("Id")]
    public Guid Id { get; set; }

    /// <summary>
    /// 結果
    /// </summary>
    [Column("IsSuccess")]
    [Required]
    public bool IsSuccess { get; set; }

    /// <summary>
    /// リクエスト内容
    /// </summary>
    [Column("Contents")]
    [Required]
    public string Contents { get; set; }

    /// <summary>
    /// 備考
    /// </summary>
    [Column("Info")]
    [Required]
    public string Info { get; set; }
}
