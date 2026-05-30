namespace Daxsys.Application.Auth.DTOs;

public class SelectContextResponseDto
{
    public string UserId { get; set; } = null!;
    public bool IsAdmin { get; set; }

    public int CompanyId { get; set; }
    public string? CompanyName { get; set; }

    public string BranchId { get; set; } = null!;
    public string? BranchName { get; set; }

    public string? WarehouseId { get; set; }
    public string? WarehouseName { get; set; }

    public string? PointOfSaleId { get; set; }
    public string? PointOfSaleName { get; set; }
    public string ContextToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}