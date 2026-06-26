namespace Daxsys.Application.Users.DTOs;

public class AssignBranchesRequestDto
{
    public int EmpCodigo { get; set; }
    public List<BranchPermissionItemDto> Branches { get; set; } = new();
}

public class BranchPermissionItemDto
{
    public string BranchId { get; set; } = null!;
    public bool Authorized { get; set; }
}