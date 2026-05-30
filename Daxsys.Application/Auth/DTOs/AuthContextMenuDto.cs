namespace Daxsys.Application.Auth.DTOs;

public class AuthContextMenuDto
{
    public int IdMenu { get; set; }
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Ruta { get; set; }
    public string? Icono { get; set; }
    public bool HasAccess { get; set; }
    public List<AuthContextMenuDto> Children { get; set; } = new();
}