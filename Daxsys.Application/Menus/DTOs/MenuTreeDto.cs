namespace Daxsys.Application.Menus.DTOs;

public class MenuTreeDto
{
    public int IdMenu { get; set; }
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public bool HasAccess { get; set; }
    public List<MenuTreeDto> Children { get; set; } = new();
}