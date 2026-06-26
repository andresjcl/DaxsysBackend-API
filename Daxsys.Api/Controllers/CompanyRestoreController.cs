using Daxsys.Application.Companies.DTOs;
using Daxsys.Application.Companies.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Daxsys.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CompanyRestoreController : ControllerBase
{
    private readonly ICompanyRestoreService _restoreService;

    public CompanyRestoreController(ICompanyRestoreService restoreService)
    {
        _restoreService = restoreService;
    }

    [HttpPost("restore")]
    public async Task<IActionResult> RestoreCompany([FromBody] RestoreCompanyRequestDto request)
    {
        try
        {
            var result = await _restoreService.RestoreCompanyFromBackupAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al restaurar empresa", detail = ex.Message });
        }
    }
}