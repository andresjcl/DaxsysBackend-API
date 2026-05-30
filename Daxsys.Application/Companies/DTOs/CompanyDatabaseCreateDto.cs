namespace Daxsys.Application.Companies.DTOs;

public class CompanyDatabaseCreateDto
{
    public string DatabaseType { get; set; } = null!;
    public string? DatabaseName { get; set; }
}