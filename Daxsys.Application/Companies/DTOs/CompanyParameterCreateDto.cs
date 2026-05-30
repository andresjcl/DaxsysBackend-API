namespace Daxsys.Application.Companies.DTOs;

public class CompanyParameterCreateDto
{
    public string? MainBranchCode { get; set; }
    public string? MainSaleDocument { get; set; }
    public string? SaleIvaCode { get; set; }
    public string? PurchaseIvaCode { get; set; }
    public int? CostDigits { get; set; }
    public int? PriceDigits { get; set; }
    public string? ImagesPath { get; set; }
}