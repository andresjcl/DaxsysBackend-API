using System.Security.Claims;
using Daxsys.Application.Auth.DTOs;
using Daxsys.Application.Auth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Daxsys.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserContextService _userContextService;

    public AuthController(
        IAuthService authService,
        IUserContextService userContextService)
    {
        _authService = authService;
        _userContextService = userContextService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);

        if (result is null)
        {
            return Unauthorized(new
            {
                message = "Usuario o contraseña incorrectos, o usuario no vigente."
            });
        }

        return Ok(result);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User.FindFirstValue("sub")
                     ?? User.FindFirstValue("unique_name");

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(new
            {
                message = "No se pudo identificar el usuario autenticado."
            });
        }

        var result = await _authService.GetMeAsync(userId);

        if (result is null)
        {
            return NotFound(new
            {
                message = "Usuario no encontrado."
            });
        }

        return Ok(result);
    }

    [Authorize]
    [HttpGet("context")]
    public async Task<IActionResult> GetContext([FromQuery] int EmpCodigo,[FromQuery] string systemId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User.FindFirstValue("sub")
                     ?? User.FindFirstValue("unique_name");

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(new
            {
                message = "No se pudo identificar el usuario autenticado."
            });
        }

        var result = await _userContextService.GetContextAsync(userId, EmpCodigo, systemId);

        if (result is null)
        {
            return BadRequest(new
            {
                message = "No se pudo obtener el contexto del usuario."
            });
        }

        return Ok(result);
    }


    [Authorize]
    [HttpPost("select-context")]
    public async Task<IActionResult> SelectContext([FromBody] SelectContextRequestDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User.FindFirstValue("sub")
                     ?? User.FindFirstValue("unique_name");

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(new
            {
                message = "No se pudo identificar el usuario autenticado."
            });
        }

        try
        {
            var result = await _userContextService.SelectContextAsync(userId, request);

            if (result is null)
            {
                return BadRequest(new
                {
                    message = "No se pudo seleccionar el contexto operativo."
                });
            }

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }
}