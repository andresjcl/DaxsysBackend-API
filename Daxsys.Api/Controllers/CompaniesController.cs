using Daxsys.Application.Companies.DTOs;
using Daxsys.Application.Companies.Interfaces;
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

    public CompaniesController(
        ICompanyQueryService companyQueryService,
        ICompanyCommandService companyCommandService)
    {
        _companyQueryService = companyQueryService;
        _companyCommandService = companyCommandService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCompanies()
    {
        var result = await _companyQueryService.GetCompaniesAsync();
        return Ok(result);
    }

    [HttpGet("{companyId:int}")]
    public async Task<IActionResult> GetCompanyById(int companyId)
    {
        var result = await _companyQueryService.GetCompanyByIdAsync(companyId);

        if (result is null)
            return NotFound(new { message = "Empresa no encontrada." });

        return Ok(result);
    }

    [HttpGet("{companyId:int}/branches")]
    public async Task<IActionResult> GetBranches(int companyId)
    {
        var result = await _companyQueryService.GetBranchesAsync(companyId);
        return Ok(result);
    }

    [HttpGet("{companyId:int}/branches/{branchCode}/warehouses")]
    public async Task<IActionResult> GetWarehouses(int companyId, string branchCode)
    {
        var result = await _companyQueryService.GetWarehousesAsync(companyId, branchCode);
        return Ok(result);
    }

    [HttpGet("{companyId:int}/branches/{branchCode}/points-of-sale")]
    public async Task<IActionResult> GetPointsOfSale(int companyId, string branchCode)
    {
        var result = await _companyQueryService.GetPointsOfSaleAsync(companyId, branchCode);
        return Ok(result);
    }

    [HttpGet("{companyId:int}/parameters")]
    public async Task<IActionResult> GetParameters(int companyId)
    {
        var result = await _companyQueryService.GetParametersAsync(companyId);

        if (result is null)
            return NotFound(new { message = "Parámetros no encontrados." });

        return Ok(result);
    }

    [HttpGet("{companyId:int}/databases")]
    public async Task<IActionResult> GetDatabases(int companyId)
    {
        var result = await _companyQueryService.GetDatabasesAsync(companyId);
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


    [HttpPut("{companyId:int}")]
    public async Task<IActionResult> UpdateCompany(int companyId, [FromBody] UpdateCompanyRequestDto request)
    {
        try
        {
            var result = await _companyCommandService.UpdateCompanyAsync(companyId, request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{companyId:int}/branches")]
    public async Task<IActionResult> CreateBranch(int companyId, [FromBody] CreateBranchRequestDto request)
    {
        try
        {
            var result = await _companyCommandService.CreateBranchAsync(companyId, request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{companyId:int}/branches/{branchCode}/warehouses")]
    public async Task<IActionResult> CreateWarehouse(int companyId, string branchCode, [FromBody] CreateWarehouseRequestDto request)
    {
        try
        {
            var result = await _companyCommandService.CreateWarehouseAsync(companyId, branchCode, request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{companyId:int}/branches/{branchCode}/points-of-sale")]
    public async Task<IActionResult> CreatePointOfSale(int companyId, string branchCode, [FromBody] CreatePointOfSaleRequestDto request)
    {
        try
        {
            var result = await _companyCommandService.CreatePointOfSaleAsync(companyId, branchCode, request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{companyId:int}/available-documents")]
    public async Task<IActionResult> GetAvailableDocuments(int companyId,[FromQuery] string archiveType = "ADC")
    {
        try
        {
            var result = await _companyQueryService.GetAvailableDocumentsAsync(companyId, archiveType);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{companyId:int}/databases")]
    public async Task<IActionResult> UpdateDatabases(int companyId,[FromBody] UpdateCompanyDatabasesRequestDto request)
    {
        try
        {
            var result = await _companyCommandService.UpdateCompanyDatabasesAsync(companyId, request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{companyId:int}/parameters")]
    public async Task<IActionResult> UpdateParameters(int companyId,[FromBody] UpdateCompanyParametersRequestDto request)
    {
        try
        {
            var result = await _companyCommandService.UpdateCompanyParametersAsync(companyId, request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{companyId:int}")]
    public async Task<IActionResult> DeleteCompany(int companyId)
    {
        try
        {
            await _companyCommandService.DeleteCompanyAsync(companyId);
            return Ok(new { message = "Empresa eliminada correctamente." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


}