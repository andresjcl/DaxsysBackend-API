using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Daxsys.Domain.Entities;

[Table("sys_Bodegas")]
[PrimaryKey(nameof(IdUsuario), nameof(IdEmpresa), nameof(CodSucursal), nameof(CodBodega))]

public class UserWarehouse
{
    public string IdUsuario { get; set; } = null!;
    public byte? IdEmpresa { get; set; }
    public string? CodSucursal { get; set; }
    public string CodBodega { get; set; } = null!;
    public string? AutorizaBod { get; set; }
}