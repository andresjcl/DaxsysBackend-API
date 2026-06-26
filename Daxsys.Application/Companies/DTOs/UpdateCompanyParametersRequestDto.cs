namespace Daxsys.Application.Companies.DTOs;

public class UpdateCompanyParametersRequestDto
{
    // ==================== CAMPOS PRINCIPALES ====================
    public string? MainBranchCode { get; set; }
    public string? MainSaleDocument { get; set; }
    public string? SaleIvaCode { get; set; }
    public string? PurchaseIvaCode { get; set; }
    public int? CostDigits { get; set; }
    public int? PriceDigits { get; set; }
    public string? ImagesPath { get; set; }

    // ==================== CAMPOS ADICIONALES ====================
    public string? UrlSRI { get; set; }
    public bool? ValidateSRI { get; set; }
    public bool? ValidateDirectory { get; set; }
    public int? LimAtrasoEntrada { get; set; }
    public int? LimExtraSalida { get; set; }
    public int? LimExtraEntrada { get; set; }
    public byte? Cheques { get; set; }
    public string? PagoCompras { get; set; }
    public string? ClvDsc { get; set; }
    public string? ClvIVA { get; set; }
}