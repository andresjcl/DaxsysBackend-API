using Daxsys.Application.Features.Facturacion.DTOs;
using Daxsys.Application.Features.Facturacion.Services;
using Daxsys.Domain.Entities;
using Daxsys.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Security.Claims;

namespace Daxsys.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FacturacionController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly FacturaService _facturaService;
    private readonly IConfiguration _configuration;

    public FacturacionController(AppDbContext dbContext, FacturaService facturaService, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _facturaService = facturaService;
        _configuration = configuration;
    }

    [HttpPost("emitir")]
    public async Task<IActionResult> EmitirFactura([FromBody] FacturaRequestDto request)
    {
        // Obtener empresaId desde el token JWT
        var empresaIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(empresaIdStr))
            return Unauthorized(new { error = "No se pudo identificar la empresa" });

        // Convertir a int (asumiendo que EmpCodigo es int)
        if (!int.TryParse(empresaIdStr, out int empresaId))
            return BadRequest(new { error = "ID de empresa inválido" });

        // Obtener la empresa
        var empresaConfig = await _dbContext.Companies
            .FirstOrDefaultAsync(e => e.EmpCodigo == empresaId);  // Comparar int con int

        if (empresaConfig == null)
            return BadRequest(new { error = "Empresa no encontrada o inactiva" });

        // Validar sucursal permitida para esta empresa
        var sucursalPermitida = await _dbContext.Branches
            .AnyAsync(s => s.EmpCodigo == empresaId           // int con int
                        && s.SucCodigo == request.Sucursal);  // string con string

        if (!sucursalPermitida)
            return BadRequest(new { error = $"Sucursal {request.Sucursal} no permitida" });

        // Obtener el nombre de la base de datos transaccional desde Emp_Arch (tipo "ADC")
        var databaseConfig = await _dbContext.CompanyDatabases
            .FirstOrDefaultAsync(db => db.EmpCodigo == empresaId && db.ArchTipo == "ADC");

        if (databaseConfig == null || string.IsNullOrEmpty(databaseConfig.ArchNom))
            return BadRequest(new { error = "No hay configuración de base de datos para esta empresa" });

        // Construir la cadena de conexión usando DefaultConnection como plantilla
        var connectionString = BuildConnectionString(databaseConfig.ArchNom);

        // Llamar al servicio de facturación con la conexión correcta
        var resultado = await _facturaService.CrearFactura(
            connectionString,
            request,
            empresaId.ToString());  // Convertir a string si el servicio espera string

        if (!resultado.Success)
            return StatusCode(500, resultado);

        return Ok(resultado);
    }

    private string BuildConnectionString(string databaseName)
    {
        // Obtener la cadena de conexión por defecto
        var defaultConnection = _configuration.GetConnectionString("DefaultConnection");

        // Usar SqlConnectionStringBuilder para modificar solo la base de datos
        var builder = new SqlConnectionStringBuilder(defaultConnection);

        // Cambiar la base de datos por la de la empresa
        builder.InitialCatalog = databaseName;

        return builder.ConnectionString;
    }
}