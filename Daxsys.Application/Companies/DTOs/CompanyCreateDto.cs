namespace Daxsys.Application.Companies.DTOs;

public class CompanyCreateDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Pais { get; set; }
    public string? Provincia { get; set; }
    public string? Ciudad { get; set; }
    public string? Canton { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono1 { get; set; }
    public string? Telefono2 { get; set; }
    public string? Email { get; set; }
    public string? Ruc { get; set; }
    public bool IsDefault { get; set; }
    public string? TipoBase { get; set; }
}