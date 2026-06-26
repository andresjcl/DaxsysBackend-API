using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Daxsys.Domain.Entities;

public class AdcStk
{
    [Key]
    [Column("Bod_codigo")]
    public string Bod_codigo { get; set; } = null!;

    [Column("Art_codigo")]
    public string Art_codigo { get; set; } = null!;

    [Column("Stk_cantidad")]
    public decimal Stk_cantidad { get; set; }

    [Column("Suc_codigo")]
    public string Suc_codigo { get; set; } = null!;
}