namespace Daxsys.Domain.Entities;

public class UserPointOfSale
{
    public string IdUsuario { get; set; } = null!;
    public byte IdEmpresa { get; set; }
    public string CodSucursal { get; set; } = null!;
    public string CodPtoVta { get; set; } = null!;
    public string? AutorizaPtoVta { get; set; }
}