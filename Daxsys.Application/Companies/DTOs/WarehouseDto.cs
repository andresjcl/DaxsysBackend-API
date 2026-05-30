namespace Daxsys.Application.Companies.DTOs;

public class WarehouseDto
{
    public int CompanyId { get; set; }
    public string BranchCode { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string? Name { get; set; }
}