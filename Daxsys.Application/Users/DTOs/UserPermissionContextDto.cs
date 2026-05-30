namespace Daxsys.Application.Users.DTOs;

public class UserPermissionContextDto
{
    public string UserId { get; set; } = null!;
    public int CompanyId { get; set; }
    public List<string> Branches { get; set; } = new();
    public List<UserWarehouseContextDto> Warehouses { get; set; } = new();
    public List<UserPointOfSaleContextDto> PointsOfSale { get; set; } = new();
}

public class UserWarehouseContextDto
{
    public string BranchId { get; set; } = null!;
    public string WarehouseId { get; set; } = null!;
}

public class UserPointOfSaleContextDto
{
    public string BranchId { get; set; } = null!;
    public string PointOfSaleId { get; set; } = null!;
}