using Daxsys.Application.Menus.DTOs;
using Daxsys.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Daxsys.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MenusController : ControllerBase
{
    private readonly MenuService _service;

    public MenusController(MenuService service)
    {
        _service = service;
    }

    [HttpGet("tree")]
    public async Task<IActionResult> GetTree(
        [FromQuery] string userId,
        [FromQuery] int companyId,
        [FromQuery] string systemId)
    {
        var result = await _service.GetMenuTreeAsync(userId, companyId, systemId);
        return Ok(result);
    }

    [HttpPut("{userId}/permissions")]
    public async Task<IActionResult> AssignPermissions(
        string userId,
        [FromBody] AssignMenuPermissionsRequestDto request)
    {
        await _service.AssignMenuPermissionsAsync(userId, request);
        return Ok(new { message = "Permisos de menú guardados" });
    }
}