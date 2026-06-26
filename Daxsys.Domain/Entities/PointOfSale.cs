using System.ComponentModel.DataAnnotations.Schema;

namespace Daxsys.Domain.Entities;

[Table("Emp_PtoVta")]
public class PointOfSale
{
    [Column("Emp_Codigo")]
    public int EmpCodigo { get; set; }

    [Column("Suc_Codigo")]
    public string SucCodigo { get; set; } = null!;

    [Column("Pto_codigo")]
    public string PtoCodigo { get; set; } = null!;

    [Column("Pto_nombre")]
    public string? PtoNombre { get; set; }

    [Column("Pto_IDTributario")]
    public string? PtoIdTributario { get; set; }

    [Column("Pto_IdCta")]
    public string? PtoIdCta { get; set; }

    [Column("TipoPunto")]
    public string? TipoPunto { get; set; }
}