using Daxsys.Application.Auth.DTOs;

namespace Daxsys.Application.Auth.Interfaces;

public interface IUserContextService
{
    Task<AuthContextResponseDto?> GetContextAsync(string userId, int companyId, string systemId);
    Task<SelectContextResponseDto?> SelectContextAsync(string userId, SelectContextRequestDto request);
}