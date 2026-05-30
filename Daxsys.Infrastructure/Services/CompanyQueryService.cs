using Daxsys.Application.Companies.DTOs;
using Daxsys.Application.Companies.Interfaces;
using Daxsys.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Daxsys.Infrastructure.Services;

public class CompanyQueryService : ICompanyQueryService
{
    private readonly AppDbContext _context;

    public CompanyQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CompanyListItemDto>> GetCompaniesAsync()
    {
        return await _context.Companies
            .AsNoTracking()
            .OrderBy(x => x.EmpCodigo)
            .Select(x => new CompanyListItemDto
            {
                Id = x.EmpCodigo,
                Name = x.EmpNombre,
                Ruc = x.EmpRuc,
                IsDefault = x.EmpDefecto,
                TipoBase = x.EmpTipoBase
            })
            .ToListAsync();
    }

    public async Task<CompanyDetailDto?> GetCompanyByIdAsync(int companyId)
    {
        return await _context.Companies
            .AsNoTracking()
            .Where(x => x.EmpCodigo == companyId)
            .Select(x => new CompanyDetailDto
            {
                Id = x.EmpCodigo,
                Name = x.EmpNombre,
                Pais = x.EmpPais,
                Provincia = x.EmpProvincia,
                Ciudad = x.EmpCiudad,
                Canton = x.EmpCanton,
                Direccion = x.EmpDireccion,
                Telefono1 = x.EmpTelefono1,
                Telefono2 = x.EmpTelefono2,
                Email = x.EmpEmail,
                Ruc = x.EmpRuc,
                IsDefault = x.EmpDefecto,
                TipoBase = x.EmpTipoBase
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<BranchDto>> GetBranchesAsync(int companyId)
    {
        return await _context.Branches
            .AsNoTracking()
            .Where(x => x.EmpCodigo == companyId)
            .OrderBy(x => x.SucCodigo)
            .Select(x => new BranchDto
            {
                CompanyId = x.EmpCodigo,
                Code = x.SucCodigo,
                Name = x.SucNombre,
                Address = x.SucDireccion,
                Ruc = x.SucRuc,
                DefaultWarehouseCode = x.BodCodigo,
                TributaryId = x.SucIdTributario
            })
            .ToListAsync();
    }

    public async Task<List<WarehouseDto>> GetWarehousesAsync(int companyId, string branchCode)
    {
        return await _context.Warehouses
            .AsNoTracking()
            .Where(x => x.EmpCodigo == companyId && x.SucCodigo == branchCode)
            .OrderBy(x => x.BodCodigo)
            .Select(x => new WarehouseDto
            {
                CompanyId = x.EmpCodigo,
                BranchCode = x.SucCodigo,
                Code = x.BodCodigo,
                Name = x.BodNombre
            })
            .ToListAsync();
    }

    public async Task<List<PointOfSaleDto>> GetPointsOfSaleAsync(int companyId, string branchCode)
    {
        return await _context.PointsOfSale
            .AsNoTracking()
            .Where(x => x.EmpCodigo == companyId && x.SucCodigo == branchCode)
            .OrderBy(x => x.PtoCodigo)
            .Select(x => new PointOfSaleDto
            {
                CompanyId = x.EmpCodigo,
                BranchCode = x.SucCodigo,
                Code = x.PtoCodigo,
                Name = x.PtoNombre,
                TributaryId = x.PtoIdTributario,
                PointType = x.TipoPunto
            })
            .ToListAsync();
    }

    public async Task<CompanyParameterDto?> GetParametersAsync(int companyId)
    {
        return await _context.CompanyParameters
            .AsNoTracking()
            .Where(x => x.EmpCodigo == companyId)
            .Select(x => new CompanyParameterDto
            {
                CompanyId = x.EmpCodigo,
                MainBranchCode = x.ParSucPri,
                MainSaleDocument = x.ParDocPrincipalVta,
                SaleIvaCode = x.ParVenIVA,
                PurchaseIvaCode = x.ParComIVA,
                CostDigits = x.ParDigitosCostos,
                PriceDigits = x.ParDigitosPrecios,
                ImagesPath = x.EmpPathImagenes
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<CompanyDatabaseDto>> GetDatabasesAsync(int companyId)
    {
        return await _context.CompanyDatabases
            .AsNoTracking()
            .Where(x => x.EmpCodigo == companyId)
            .OrderBy(x => x.ArchTipo)
            .Select(x => new CompanyDatabaseDto
            {
                CompanyId = x.EmpCodigo,
                DatabaseType = x.ArchTipo,
                DatabaseName = x.ArchNom
            })
            .ToListAsync();
    }

    public async Task<List<AvailableDocumentDto>> GetAvailableDocumentsAsync(int companyId, string archiveType)
    {
        var databaseName = await _context.CompanyDatabases
            .AsNoTracking()
            .Where(x => x.EmpCodigo == companyId && x.ArchTipo == archiveType)
            .Select(x => x.ArchNom)
            .FirstOrDefaultAsync();

        if (string.IsNullOrWhiteSpace(databaseName))
            throw new InvalidOperationException("No se encontró la base transaccional configurada en Emp_Arch.");

        if (!IsSafeSqlIdentifier(databaseName))
            throw new InvalidOperationException("El nombre de la base transaccional no es válido.");

        var connection = _context.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        await using var command = connection.CreateCommand();

        command.CommandText = $@"
        SELECT
            Opc_documento,
            Opc_nombre,
            Opc_tipo
        FROM [{databaseName}].dbo.AdcOpc
        ORDER BY Opc_documento;
    ";

        var result = new List<AvailableDocumentDto>();

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new AvailableDocumentDto
            {
                DocumentCode = reader["Opc_documento"]?.ToString()?.Trim() ?? "",
                DocumentName = reader["Opc_nombre"]?.ToString()?.Trim(),
                DocumentType = reader["Opc_tipo"]?.ToString()?.Trim()
            });
        }

        return result;
    }

    private static bool IsSafeSqlIdentifier(string value)
    {
        return value.All(c => char.IsLetterOrDigit(c) || c == '_');
    }


}