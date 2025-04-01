using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LineWebHookAPI.Models.DB.Tables;

/// <summary>
/// PostGourmetLocationAsyncのリクエストログ
/// </summary>
[Table("GourmetWordLog")]
public class GourmetWordLog : Meta
{
    /// <summary>
    /// 主キー
    /// </summary>
    [Key]
    [Column("Id")]
    public Guid Id { get; set; }

    /// <summary>
    /// 緯度
    /// </summary>
    [Column("Text")]
    public string Text {get; set;}
}
