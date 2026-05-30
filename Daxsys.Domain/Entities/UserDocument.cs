namespace Daxsys.Domain.Entities;

public class UserDocument
{
    public string IdUsuario { get; set; } = null!;
    public byte IdEmpresa { get; set; }
    public string CodDocumento { get; set; } = null!;
    public string? Cambios { get; set; }
}