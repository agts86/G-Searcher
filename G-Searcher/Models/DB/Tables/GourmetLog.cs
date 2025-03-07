using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace G_Searcher.Models.DB.Tables;

/// <summary>
/// GetGourmetAsyncのリクエストログ
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
