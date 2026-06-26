namespace Daxsys.Application.Companies.DTOs;

public class CreateCompanyResponseDto
{
    public int EmpCodigo { get; set; }
    public string? CompanyName { get; set; }
    public string? MainBranchCode { get; set; }
    public string? MainWarehouseCode { get; set; }
    public int PointsOfSaleCount { get; set; }
    public int DatabasesCount { get; set; }
}