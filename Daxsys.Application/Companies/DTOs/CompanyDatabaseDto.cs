namespace Daxsys.Application.Companies.DTOs;

public class CompanyDatabaseDto
{
    public int CompanyId { get; set; }
    public string DatabaseType { get; set; } = null!;
    public string? DatabaseName { get; set; }
}