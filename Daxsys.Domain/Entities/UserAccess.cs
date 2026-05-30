namespace Daxsys.Domain.Entities;

public class UserAccess
{
    public string IdUsuario { get; set; } = null!;
    public float IdEmpresa { get; set; }
    public string? IdSistema { get; set; }
    public string? IdOpcion { get; set; }
    public string? IdNomOpcion { get; set; }
    public string? Accesos { get; set; }
    public decimal IdClaveDoc { get; set; }
}