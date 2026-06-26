using Daxsys.Application.Users.DTOs;
using Daxsys.Application.Users.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Daxsys.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserManagementService _userManagementService;

    public UsersController(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var result = await _userManagementService.GetUsersAsync();
        return Ok(result);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(string userId)
    {
        var result = await _userManagementService.GetUserByIdAsync(userId);

        if (result is null)
            return NotFound(new { message = "Usuario no encontrado." });

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDto request)
    {
        try
        {
            var result = await _userManagementService.CreateUserAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserRequestDto request)
    {
        try
        {
            var result = await _userManagementService.UpdateUserAsync(userId, request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{userId}/password")]
    public async Task<IActionResult> ChangePassword(string userId, [FromBody] ChangePasswordRequestDto request)
    {
        try
        {
            var result = await _userManagementService.ChangePasswordAsync(userId, request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{userId}/branches")]
    public async Task<IActionResult> AssignBranches(string userId, [FromBody] AssignBranchesRequestDto request)
    {
        try
        {
            await _userManagementService.AssignBranchesAsync(userId, request);
            return Ok(new { message = "Sucursales asignadas correctamente." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{userId}/warehouses")]
    public async Task<IActionResult> AssignWarehouses(string userId, [FromBody] AssignWarehousesRequestDto request)
    {
        try
        {
            await _userManagementService.AssignWarehousesAsync(userId, request);
            return Ok(new { message = "Bodegas asignadas correctamente." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{userId}/points-of-sale")]
    public async Task<IActionResult> AssignPointsOfSale(string userId, [FromBody] AssignPointsOfSaleRequestDto request)
    {
        try
        {
            await _userManagementService.AssignPointsOfSaleAsync(userId, request);
            return Ok(new { message = "Puntos de venta asignados correctamente." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{userId}/permissions/context")]
    public async Task<IActionResult> GetPermissionContext(string userId, [FromQuery] int EmpCodigo)
    {
        var result = await _userManagementService.GetPermissionContextAsync(userId, EmpCodigo);

        if (result is null)
            return NotFound(new { message = "Usuario no encontrado." });

        return Ok(result);
    }

    [HttpPut("{userId}/accesses")]
    public async Task<IActionResult> AssignAccesses(string userId, [FromBody] AssignAccessesRequestDto request)
    {
        try
        {
            await _userManagementService.AssignAccessesAsync(userId, request);
            return Ok(new { message = "Accesos asignados correctamente." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{userId}/accesses")]
    public async Task<IActionResult> GetAccesses(string userId, [FromQuery] int EmpCodigo)
    {
        var result = await _userManagementService.GetAccessesAsync(userId, EmpCodigo);
        return Ok(result);
    }

    [HttpPut("{userId}/documents")]
    public async Task<IActionResult> AssignDocuments(string userId, [FromBody] AssignDocumentsRequestDto request)
    {
        try
        {
            await _userManagementService.AssignDocumentsAsync(userId, request);
            return Ok(new { message = "Documentos asignados correctamente." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{userId}/documents")]
    public async Task<IActionResult> GetDocuments(string userId, [FromQuery] int EmpCodigo)
    {
        var result = await _userManagementService.GetDocumentsAsync(userId, EmpCodigo);
        return Ok(result);
    }

    [HttpGet("{userId}/document-accesses")]
    public async Task<IActionResult> GetDocumentAccesses(
    string userId,
    [FromQuery] int EmpCodigo,
    [FromQuery] string documentCode)
    {
        var result = await _userManagementService.GetDocumentAccessesAsync(userId, EmpCodigo, documentCode);
        return Ok(result);
    }


    [HttpPut("{userId}/document-accesses")]
    public async Task<IActionResult> AssignDocumentAccesses(string userId,[FromBody] AssignDocumentAccessesRequestDto request)
    {
        try
        {
            await _userManagementService.AssignDocumentAccessesAsync(userId, request);
            return Ok(new { message = "Accesos detallados del documento guardados correctamente." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    [HttpGet("{userId}/assignable-branches")]
    public async Task<IActionResult> GetAssignableBranches(string userId,[FromQuery] int EmpCodigo)
    {
        try
        {
            var result = await _userManagementService.GetAssignableBranchesAsync(userId, EmpCodigo);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{userId}/assignable-warehouses")]
    public async Task<IActionResult> GetAssignableWarehouses(string userId,[FromQuery] int EmpCodigo,[FromQuery] string branchId)
    {
        try
        {
            var result = await _userManagementService.GetAssignableWarehousesAsync(userId, EmpCodigo, branchId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{userId}/assignable-points-of-sale")]
    public async Task<IActionResult> GetAssignablePointsOfSale(string userId,[FromQuery] int EmpCodigo,[FromQuery] string branchId)
    {
        try
        {
            var result = await _userManagementService.GetAssignablePointsOfSaleAsync(userId, EmpCodigo, branchId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{userId}/assignable-documents")]
    public async Task<IActionResult> GetAssignableDocuments(string userId,[FromQuery] int EmpCodigo,[FromQuery] string archiveType = "ADC")
    {
        try
        {
            var result = await _userManagementService.GetAssignableDocumentsAsync(userId, EmpCodigo, archiveType);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

}