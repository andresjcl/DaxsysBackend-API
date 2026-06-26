using Daxsys.Application.Companies.DTOs;
using Daxsys.Application.Companies.Interfaces;
using Daxsys.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Daxsys.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyQueryService _companyQueryService;
    private readonly ICompanyCommandService _companyCommandService;
    private readonly ICompanyRestoreService _companyRestoreService;

    public CompaniesController(
        ICompanyQueryService companyQueryService,
        ICompanyCommandService companyCommandService,
         ICompanyRestoreService companyRestoreService)
    {
        _companyQueryService = companyQueryService;
        _companyCommandService = companyCommandService;
        _companyRestoreService = companyRestoreService; 
    }

    [HttpGet]
    public async Task<IActionResult> GetCompanies()
    {
        var result = await _companyQueryService.GetCompaniesAsync();
        return Ok(result);
    }

    [HttpGet("{EmpCodigo:int}")]
    public async Task<IActionResult> GetCompanyById(int EmpCodigo)
    {
        var result = await _companyQueryService.GetCompanyByIdAsync(EmpCodigo);

        if (result is null)
            return NotFound(new { message = "Empresa no encontrada." });

        return Ok(result);
    }

    [HttpGet("{EmpCodigo:int}/branches")]
    public async Task<IActionResult> GetBranches(int EmpCodigo)
    {
        var result = await _companyQueryService.GetBranchesAsync(EmpCodigo);
        return Ok(result);
    }

    [HttpGet("{EmpCodigo:int}/branches/{branchCode}/warehouses")]
    public async Task<IActionResult> GetWarehouses(int EmpCodigo, string branchCode)
    {
        var result = await _companyQueryService.GetWarehousesAsync(EmpCodigo, branchCode);
        return Ok(result);
    }

    [HttpGet("{EmpCodigo:int}/branches/{branchCode}/points-of-sale")]
    public async Task<IActionResult> GetPointsOfSale(int EmpCodigo, string branchCode)
    {
        var result = await _companyQueryService.GetPointsOfSaleAsync(EmpCodigo, branchCode);
        return Ok(result);
    }

    [HttpGet("{EmpCodigo:int}/parameters")]
    public async Task<IActionResult> GetParameters(int EmpCodigo)
    {
        var result = await _companyQueryService.GetParametersAsync(EmpCodigo);

        if (result is null)
            return NotFound(new { message = "Parámetros no encontrados." });

        return Ok(result);
    }

    [HttpGet("{EmpCodigo:int}/databases")]
    public async Task<IActionResult> GetDatabases(int EmpCodigo)
    {
        var result = await _companyQueryService.GetDatabasesAsync(EmpCodigo);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyRequestDto request)
    {
        try
        {
            var result = await _companyCommandService.CreateCompanyAsync(request);
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

    //[HttpPost("restore-and-create")]
    //public async Task<IActionResult> RestoreAndCreateCompany([FromBody] RestoreCompanyRequestDto request)
    //{
    //    try
    //    {
    //        var result = await _companyRestoreService.RestoreCompanyFromBackupAsync(request);
    //        return Ok(result);
    //    }
    //    catch (InvalidOperationException ex)
    //    {
    //        return BadRequest(new { message = ex.Message });
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, new { message = "Error al restaurar y crear empresa", detail = ex.Message });
    //    }
    //}

    [HttpPut("{EmpCodigo:int}")]
    public async Task<IActionResult> UpdateCompany(int EmpCodigo, [FromBody] UpdateCompanyRequestDto request)
    {
        try
        {
            var result = await _companyCommandService.UpdateCompanyAsync(EmpCodigo, request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{EmpCodigo:int}/branches")]
    public async Task<IActionResult> CreateBranch(int EmpCodigo, [FromBody] CreateBranchRequestDto request)
    {
        try
        {
            var result = await _companyCommandService.CreateBranchAsync(EmpCodigo, request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{EmpCodigo:int}/branches/{branchCode}/warehouses")]
    public async Task<IActionResult> CreateWarehouse(int EmpCodigo, string branchCode, [FromBody] CreateWarehouseRequestDto request)
    {
        try
        {
            var result = await _companyCommandService.CreateWarehouseAsync(EmpCodigo, branchCode, request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{EmpCodigo:int}/branches/{branchCode}/points-of-sale")]
    public async Task<IActionResult> CreatePointOfSale(int EmpCodigo, string branchCode, [FromBody] CreatePointOfSaleRequestDto request)
    {
        try
        {
            var result = await _companyCommandService.CreatePointOfSaleAsync(EmpCodigo, branchCode, request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{EmpCodigo:int}/available-documents")]
    public async Task<IActionResult> GetAvailableDocuments(int EmpCodigo,[FromQuery] string archiveType = "ADC")
    {
        try
        {
            var result = await _companyQueryService.GetAvailableDocumentsAsync(EmpCodigo, archiveType);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{EmpCodigo:int}/databases")]
    public async Task<IActionResult> UpdateDatabases(int EmpCodigo,[FromBody] UpdateCompanyDatabasesRequestDto request)
    {
        try
        {
            var result = await _companyCommandService.UpdateCompanyDatabasesAsync(EmpCodigo, request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{EmpCodigo:int}/parameters")]
    public async Task<IActionResult> UpdateParameters(int EmpCodigo,[FromBody] UpdateCompanyParametersRequestDto request)
    {
        try
        {
            var result = await _companyCommandService.UpdateCompanyParametersAsync(EmpCodigo, request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{EmpCodigo:int}")]
    public async Task<IActionResult> DeleteCompany(int EmpCodigo)
    {
        try
        {
            await _companyCommandService.DeleteCompanyAsync(EmpCodigo);
            return Ok(new { message = "Empresa eliminada correctamente." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


}