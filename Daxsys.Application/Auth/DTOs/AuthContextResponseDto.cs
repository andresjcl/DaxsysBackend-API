namespace Daxsys.Application.Auth.DTOs;

public class AuthContextResponseDto
{
    public string UserId { get; set; } = null!;
    public bool IsAdmin { get; set; }
    public int EmpCodigo { get; set; }
    public string? CompanyName { get; set; }

    public string? DefaultBranchId { get; set; }
    public string? DefaultWarehouseId { get; set; }
    public string? DefaultPointOfSaleId { get; set; }

    public List<AuthContextBranchDto> Branches { get; set; } = new();
    public List<AuthContextWarehouseDto> Warehouses { get; set; } = new();
    public List<AuthContextPointOfSaleDto> PointsOfSale { get; set; } = new();
    public List<AuthContextMenuDto> Menu { get; set; } = new();
    public List<AuthContextDocumentDto> Documents { get; set; } = new();
}