using System.ComponentModel.DataAnnotations.Schema;

namespace G_Searcher.DB.Tables;

public abstract class Meta
{
    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; }

    [Column("UpdatedAt")]
    public DateTime UpdatedAt { get; set; }
}
