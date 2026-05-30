namespace Daxsys.Domain.Entities;

public class UserWarehouse
{
    public string IdUsuario { get; set; } = null!;
    public byte? IdEmpresa { get; set; }
    public string? CodSucursal { get; set; }
    public string CodBodega { get; set; } = null!;
    public string? AutorizaBod { get; set; }
}