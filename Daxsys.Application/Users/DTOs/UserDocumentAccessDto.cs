namespace Daxsys.Application.Users.DTOs;

public class UserDocumentAccessDto
{
    public int CompanyId { get; set; }
    public string UserId { get; set; } = null!;
    public string DocumentCode { get; set; } = null!;
    public string Option { get; set; } = null!;
    public bool? Opened { get; set; }
    public int? Quantity { get; set; }
    public decimal? Minimo { get; set; }
    public decimal? Maximo { get; set; }
    public string? FixedValue { get; set; }
    public decimal? AuxVal1 { get; set; }
    public decimal? AuxVal2 { get; set; }
    public string? AuxStr1 { get; set; }
    public string? AuxStr2 { get; set; }
}