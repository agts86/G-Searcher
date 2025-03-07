using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace G_Searcher.Models.DB.Tables;

[Table("GourmetLog")]
public class GourmetLog : Meta
{
    [Key]
    [Column("Id")]
    public Guid Id { get; set; }

    [Column("Lat")]
    public double Lat {get; set;}

    [Column("Lng")]
    public double Lng {get; set;}
}
