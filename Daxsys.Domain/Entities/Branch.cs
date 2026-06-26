using System.ComponentModel.DataAnnotations.Schema;

namespace Daxsys.Domain.Entities;

[Table("Emp_suc")]
public class Branch
{
    [Column("Emp_Codigo")]
    public int EmpCodigo { get; set; }

    [Column("Suc_Codigo")]
    public string SucCodigo { get; set; } = null!;

    [Column("Suc_Nombre")]
    public string? SucNombre { get; set; }

    [Column("Suc_Direccion")]
    public string? SucDireccion { get; set; }

    [Column("Suc_RUC")]
    public string? SucRuc { get; set; }

    [Column("Suc_SegSocial")]
    public string? SucSegSocial { get; set; }

    [Column("Suc_IdCta")]
    public string? SucIdCta { get; set; }

    [Column("Bod_Codigo")]
    public string? BodCodigo { get; set; }

    [Column("Suc_IdTributario")]
    public string? SucIdTributario { get; set; }

    [Column("precioVta")]
    public decimal? PrecioVta { get; set; }
}