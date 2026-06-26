namespace Daxsys.Application.Companies.DTOs;

public class CompanyDatabaseDto
{
    public int EmpCodigo { get; set; }
    public string DatabaseType { get; set; } = null!;
    public string? DatabaseName { get; set; }
}