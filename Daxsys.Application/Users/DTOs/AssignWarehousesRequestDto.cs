namespace Daxsys.Application.Users.DTOs;

public class AssignWarehousesRequestDto
{
    public int CompanyId { get; set; }
    public string BranchId { get; set; } = null!;
    public List<WarehousePermissionItemDto> Warehouses { get; set; } = new();
}

public class WarehousePermissionItemDto
{
    public string WarehouseId { get; set; } = null!;
    public bool Authorized { get; set; }
}