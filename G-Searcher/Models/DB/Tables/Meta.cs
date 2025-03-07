using System.ComponentModel.DataAnnotations.Schema;

namespace G_Searcher.Models.DB.Tables;

/// <summary>
/// メタデータ
/// </summary>
public abstract class Meta
{
    /// <summary>
    /// 登録日時
    /// </summary>
    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新日時
    /// </summary>
    [Column("UpdatedAt")]
    public DateTime UpdatedAt { get; set; }
}
