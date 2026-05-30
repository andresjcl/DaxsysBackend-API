namespace Daxsys.Application.Menus.DTOs;

public class AssignMenuPermissionsRequestDto
{
    public int CompanyId { get; set; }
    public string SystemId { get; set; } = null!;
    public List<int> MenuIds { get; set; } = new();
}