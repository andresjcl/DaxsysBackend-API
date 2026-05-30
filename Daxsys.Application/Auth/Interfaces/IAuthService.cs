using Daxsys.Application.Auth.DTOs;

namespace Daxsys.Application.Auth.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
    Task<MeResponseDto?> GetMeAsync(string userId);
}