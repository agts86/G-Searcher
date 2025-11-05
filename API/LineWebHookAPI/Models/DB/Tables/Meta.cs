using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using LineDevSdk.Jsons;

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
    private DateTime _createdAt;

    [Column("CreatedAt")]
    public DateTime CreatedAt
    {
        get => GetLocalDateTime(_createdAt);
        set => _createdAt = GetUtcDateTime(value);
    }

    /// <summary>
    /// 更新日時
    /// </summary>
    private DateTime _updatedAt;

    [Column("UpdatedAt")]
    public DateTime UpdatedAt
    {
        get => GetLocalDateTime(_updatedAt);
        set => _updatedAt = GetUtcDateTime(value);
    }

    /// <summary>
    /// ローカル日時に変換する
    /// </summary>
    /// <param name="value">日時</param>
    /// <returns>ローカル日時</returns>
    private static DateTime GetLocalDateTime(DateTime value) => DateTime.SpecifyKind(value, DateTimeKind.Utc).ToLocalTime();

    /// <summary>
    /// UTC日時に変換する
    /// </summary>
    /// <param name="value">日時</param>
    /// <returns>UTC日時</returns>
    private static DateTime GetUtcDateTime(DateTime value)
    {
        return value.Kind == DateTimeKind.Unspecified
        ? DateTime.SpecifyKind(value, DateTimeKind.Utc)
        : value.ToUniversalTime();
    }
}
