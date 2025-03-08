using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LineWebHookAPI.Models.DB.Tables;

/// <summary>
/// PostGourmetLocationAsyncのリクエストログ
/// </summary>
[Table("GourmetLog")]
public class GourmetLog : Meta
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
    [Column("Lat")]
    public double Lat {get; set;}

    /// <summary>
    /// 軽度
    /// </summary>
    [Column("Lng")]
    public double Lng {get; set;}
}
