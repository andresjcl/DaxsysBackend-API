namespace Daxsys.Domain.Entities;

public class UserBranch
{
    public string IdUsuario { get; set; } = null!;
    public byte? IdEmpresa { get; set; }
    public string CodSucursal { get; set; } = null!;
    public string? AutorizaSuc { get; set; }
}