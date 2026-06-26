namespace Daxsys.Application.Companies.DTOs;

public class PointOfSaleDto
{
    public int EmpCodigo { get; set; }
    public string BranchCode { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string? Name { get; set; }
    public string? TributaryId { get; set; }
    public string? PointType { get; set; }
}