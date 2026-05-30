using Daxsys.Application.System.DTOs;
using Daxsys.Application.System.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Daxsys.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SystemController : ControllerBase
{
    private readonly ISystemQueryService _systemQueryService;

    public SystemController(ISystemQueryService systemQueryService)
    {
        _systemQueryService = systemQueryService;
    }

    [HttpGet("databases")]
    public async Task<IActionResult> GetDatabases()
    {
        var result = await _systemQueryService.GetDatabasesAsync();
        return Ok(result);
    }


    [AllowAnonymous]
    [HttpPost("initialize")]
    public async Task<IActionResult> Initialize([FromBody] InitializeSystemRequestDto request)
    {
        try
        {
            await _systemQueryService.InitializeSystemAsync(request);

            return Ok(new
            {
                message = "Sistema inicializado correctamente."
            });
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