namespace Daxsys.Application.Menus.DTOs;

public class AssignMenuPermissionsRequestDto
{
    public int EmpCodigo { get; set; }
    public string SystemId { get; set; } = null!;
    public List<int> MenuIds { get; set; } = new();
}