using LineWebHookAPI.Models.DB.Tables;
using Microsoft.EntityFrameworkCore;

namespace LineWebHookAPI.Models.DB.SaveChanges;

/// <summary>
/// 登録用メタデータ操作クラス
/// </summary>
public class SaveChangesAdd : ISaveChanges
{
    /// <summary>
    /// エンティティの状態
    /// </summary>
    public EntityState EntityState { get; set; } = EntityState.Added;

    /// <summary>
    /// メタデータ操作
    /// </summary>
    /// <param name="meta">メタデータ</param>
    public void ChangeMeta(Meta meta)
    {
        meta.CreatedAt = DateTime.Now;
        meta.UpdatedAt = DateTime.Now;
    }
}
