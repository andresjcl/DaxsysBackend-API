namespace Daxsys.Application.Companies.DTOs;

public class CompanyListItemDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Ruc { get; set; }
    public bool IsDefault { get; set; }
    public string? TipoBase { get; set; }
}