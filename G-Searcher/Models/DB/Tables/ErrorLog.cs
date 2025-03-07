using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace G_Searcher.Models.DB.Tables;

[Table("ErrorLog")]
public class ErrorLog : Meta
{
    [Key]
    [Column("Id")]
    public Guid Id { get; set; }

    [Column("Contents")]
    [Required]
    public string Contents { get; set; }
}
