using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace G_Searcher.Models.DB.Tables;

/// <summary>
/// エラーログ
/// </summary>
[Table("ErrorLog")]
public class ErrorLog : Meta
{
    /// <summary>
    /// 主キー
    /// </summary>
    [Key]
    [Column("Id")]
    public Guid Id { get; set; }

    /// <summary>
    /// エラーメッセージ
    /// </summary>
    [Column("Contents")]
    [Required]
    public string Contents { get; set; }
}
