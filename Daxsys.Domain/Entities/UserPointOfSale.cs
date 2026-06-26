using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Daxsys.Domain.Entities;

[Table("sys_ptoVta")]
[PrimaryKey(nameof(IdUsuario), nameof(IdEmpresa), nameof(CodSucursal), nameof(CodPtoVta))]

public class UserPointOfSale
{
    public string IdUsuario { get; set; } = null!;
    public byte IdEmpresa { get; set; }
    public string CodSucursal { get; set; } = null!;
    public string CodPtoVta { get; set; } = null!;
    public string? AutorizaPtoVta { get; set; }
}