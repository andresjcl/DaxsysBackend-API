namespace Daxsys.Application.Auth.DTOs;

public class SelectContextRequestDto
{
    public int EmpCodigo { get; set; }
    public string BranchId { get; set; } = null!;
    public string? WarehouseId { get; set; }
    public string? PointOfSaleId { get; set; }
}