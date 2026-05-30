namespace Daxsys.Application.Companies.DTOs;

public class BranchDto
{
    public int CompanyId { get; set; }
    public string Code { get; set; } = null!;
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? Ruc { get; set; }
    public string? DefaultWarehouseCode { get; set; }
    public string? TributaryId { get; set; }
}