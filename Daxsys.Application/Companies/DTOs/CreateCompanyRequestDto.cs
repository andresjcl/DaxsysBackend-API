namespace Daxsys.Application.Companies.DTOs;

public class CreateCompanyRequestDto
{
    public CompanyCreateDto Company { get; set; } = null!;
    public CompanyParameterCreateDto Parameters { get; set; } = null!;
    public BranchCreateDto MainBranch { get; set; } = null!;
    public WarehouseCreateDto MainWarehouse { get; set; } = null!;
    public List<PointOfSaleCreateDto> PointsOfSale { get; set; } = new();
    public List<CompanyDatabaseCreateDto> Databases { get; set; } = new();
}