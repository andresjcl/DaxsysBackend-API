using Daxsys.Application.Auth.DTOs;
using Daxsys.Application.Auth.Interfaces;
using Daxsys.Application.Common.Interfaces;
using Daxsys.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Daxsys.Application.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUserRepository userRepository,
        ITokenService tokenService,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.GetByIdAsync(request.UserName);

        if (user is null)
            return null;

        var now = DateTime.Now;

        if (user.FechaInicio.HasValue && user.FechaInicio.Value.Date > now.Date)
            return null;

        if (user.FechaCaduca.HasValue && user.FechaCaduca.Value.Date < now.Date)
            return null;

        if (!string.Equals(user.Contrasena, request.Password, StringComparison.Ordinal))
            return null;

        var isAdmin = string.Equals(user.IdUsuario, "administrador", StringComparison.OrdinalIgnoreCase);

        var expirationMinutesText = _configuration["Jwt:ExpirationMinutes"];
        var expirationMinutes = int.TryParse(expirationMinutesText, out var parsed)
            ? parsed
            : 480;

        var expiresAt = now.AddMinutes(expirationMinutes);
        var token = _tokenService.GenerateToken(user, isAdmin, expiresAt);

        return new LoginResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = new UserLoginDto
            {
                Id = user.IdUsuario,
                IsAdmin = isAdmin
            }
        };
    }

    public async Task<MeResponseDto?> GetMeAsync(string userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user is null)
            return null;

        var isAdmin = string.Equals(user.IdUsuario, "administrador", StringComparison.OrdinalIgnoreCase);

        return new MeResponseDto
        {
            Id = user.IdUsuario,
            IsAdmin = isAdmin,
            FechaInicio = user.FechaInicio,
            FechaCaduca = user.FechaCaduca
        };
    }
}