namespace Daxsys.Application.Users.DTOs;

public class AssignableWarehouseDto
{
    public int EmpCodigo { get; set; }
    public string BranchId { get; set; } = null!;
    public string WarehouseId { get; set; } = null!;
    public string? WarehouseName { get; set; }
    public bool HasAccess { get; set; }
}