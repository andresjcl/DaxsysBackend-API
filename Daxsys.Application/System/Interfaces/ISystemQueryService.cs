using Daxsys.Application.System.DTOs;

namespace Daxsys.Application.System.Interfaces;

public interface ISystemQueryService
{
    Task<List<DatabaseInfoDto>> GetDatabasesAsync();
    Task InitializeSystemAsync(InitializeSystemRequestDto request);
}