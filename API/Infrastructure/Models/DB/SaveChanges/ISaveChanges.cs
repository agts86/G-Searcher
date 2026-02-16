using Application.Models.DB.Tables;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models.DB.SaveChanges;

/// <summary>
/// メタデータ操作インターフェース
/// </summary>
public interface ISaveChanges
{
    /// <summary>
    /// エンティティの状態
    /// </summary>
    public EntityState EntityState { get; set; }

    /// <summary>
    /// メタデータ操作
    /// </summary>
    /// <param name="meta">メタデータ</param>
    public void ChangeMeta(Meta meta);
}
