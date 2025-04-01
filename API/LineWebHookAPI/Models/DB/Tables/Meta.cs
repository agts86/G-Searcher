using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using LineWebHookAPI.Jsons;

namespace LineWebHookAPI.Models.DB.Tables;

/// <summary>
/// メタデータ
/// </summary>
[JsonConverter(typeof(RealMoldConverter<Meta>))]
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
