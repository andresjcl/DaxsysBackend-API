using System.ComponentModel.DataAnnotations.Schema;

namespace Daxsys.Domain.Entities;

[Table("Emp_bod")]
public class Warehouse
{
    [Column("Emp_Codigo")]
    public int EmpCodigo { get; set; }

    [Column("Suc_Codigo")]
    public string SucCodigo { get; set; } = null!;

    [Column("Bod_codigo")]
    public string BodCodigo { get; set; } = null!;

    [Column("Bod_nombre")]
    public string? BodNombre { get; set; }

    [Column("Bod_IdCta")]
    public string? BodIdCta { get; set; }
}