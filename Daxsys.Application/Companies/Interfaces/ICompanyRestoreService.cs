using System.Threading.Tasks;
using Daxsys.Application.Companies.DTOs;

namespace Daxsys.Application.Companies.Interfaces;

public interface ICompanyRestoreService
{
    Task<RestoreCompanyResponseDto> RestoreCompanyFromBackupAsync(RestoreCompanyRequestDto request);
}

public class RestoreCompanyResponseDto
{
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = null!;
    public string Ruc { get; set; } = null!;
    public string Initials { get; set; } = null!;
    public List<RestoredDatabaseDto> Databases { get; set; } = new();
    public bool Success { get; set; }
    public string? Message { get; set; }
}

public class RestoredDatabaseDto
{
    public string DatabaseType { get; set; } = null!;
    public string SourceDatabaseName { get; set; } = null!;
    public string NewDatabaseName { get; set; } = null!;
    public bool Restored { get; set; }
}