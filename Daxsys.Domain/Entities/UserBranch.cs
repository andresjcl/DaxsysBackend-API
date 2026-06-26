using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Daxsys.Domain.Entities;

[Table("sys_Sucursales")]
[PrimaryKey(nameof(IdUsuario), nameof(IdEmpresa), nameof(CodSucursal))]
public class UserBranch
{
    public string IdUsuario { get; set; } = null!;
    public byte? IdEmpresa { get; set; }
    public string CodSucursal { get; set; } = null!;
    public string? AutorizaSuc { get; set; }
}