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
    /// 日本のタイムゾーン
    /// </summary>
    private const int JapanKind = 9;

    /// <summary>
    /// 登録日時
    /// </summary>
    private DateTime _createdAt;
    
    [Column("CreatedAt")]
    public DateTime CreatedAt 
    {
        get => _createdAt.AddHours(JapanKind);
        set => _createdAt = value.ToUniversalTime();
    }
    /// <summary>
    /// 更新日時
    /// </summary>
    private DateTime _updatedAt;
    
    [Column("UpdatedAt")]
    public DateTime UpdatedAt 
    {
        get => _updatedAt.AddHours(JapanKind);
        set => _updatedAt = value.ToUniversalTime();
    }
}
