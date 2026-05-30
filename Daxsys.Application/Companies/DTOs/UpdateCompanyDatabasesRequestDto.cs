namespace Daxsys.Application.Companies.DTOs;

public class UpdateCompanyDatabasesRequestDto
{
    public List<UpdateCompanyDatabaseItemDto> Databases { get; set; } = new();
}

public class UpdateCompanyDatabaseItemDto
{
    public string ArchiveType { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
}