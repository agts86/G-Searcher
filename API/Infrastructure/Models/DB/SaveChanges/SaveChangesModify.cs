using Application.Models.DB.Tables;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models.DB.SaveChanges;

/// <summary>
/// 更新用メタデータ操作クラス
/// </summary>
public class SaveChangesModify: ISaveChanges
{
    /// <summary>
    /// エンティティの状態
    /// </summary>
    public EntityState EntityState { get; set; } = EntityState.Modified;

    /// <summary>
    /// メタデータ操作
    /// </summary>
    /// <param name="meta">メタデータ</param>
    public void ChangeMeta(Meta meta)
    {
        meta.UpdatedAt = DateTime.UtcNow;
    }
}
