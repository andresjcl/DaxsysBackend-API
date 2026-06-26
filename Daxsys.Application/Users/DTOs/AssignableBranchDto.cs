namespace Daxsys.Application.Users.DTOs;

public class AssignableBranchDto
{
    public int EmpCodigo { get; set; }
    public string BranchId { get; set; } = null!;
    public string? BranchName { get; set; }
    public bool HasAccess { get; set; }
}