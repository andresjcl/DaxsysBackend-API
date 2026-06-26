using Daxsys.Application.Companies.DTOs;
using Daxsys.Application.Companies.Interfaces;
using Daxsys.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;

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

    public async Task<CompanyDetailDto?> GetCompanyByIdAsync(int EmpCodigo)
    {
        return await _context.Companies
            .AsNoTracking()
            .Where(x => x.EmpCodigo == EmpCodigo)
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

    public async Task<List<BranchDto>> GetBranchesAsync(int EmpCodigo)
    {
        return await _context.Branches
            .AsNoTracking()
            .Where(x => x.EmpCodigo == EmpCodigo)
            .OrderBy(x => x.SucCodigo)
            .Select(x => new BranchDto
            {
                EmpCodigo = x.EmpCodigo,
                Code = x.SucCodigo,
                Name = x.SucNombre,
                Address = x.SucDireccion,
                Ruc = x.SucRuc,
                DefaultWarehouseCode = x.BodCodigo,
                TributaryId = x.SucIdTributario
            })
            .ToListAsync();
    }

    public async Task<List<WarehouseDto>> GetWarehousesAsync(int EmpCodigo, string branchCode)
    {
        return await _context.Warehouses
            .AsNoTracking()
            .Where(x => x.EmpCodigo == EmpCodigo && x.SucCodigo == branchCode)
            .OrderBy(x => x.BodCodigo)
            .Select(x => new WarehouseDto
            {
                EmpCodigo = x.EmpCodigo,
                BranchCode = x.SucCodigo,
                Code = x.BodCodigo,
                Name = x.BodNombre
            })
            .ToListAsync();
    }

    public async Task<List<PointOfSaleDto>> GetPointsOfSaleAsync(int EmpCodigo, string branchCode)
    {
        return await _context.PointsOfSale
            .AsNoTracking()
            .Where(x => x.EmpCodigo == EmpCodigo && x.SucCodigo == branchCode)
            .OrderBy(x => x.PtoCodigo)
            .Select(x => new PointOfSaleDto
            {
                EmpCodigo = x.EmpCodigo,
                BranchCode = x.SucCodigo,
                Code = x.PtoCodigo,
                Name = x.PtoNombre,
                TributaryId = x.PtoIdTributario,
                PointType = x.TipoPunto
            })
            .ToListAsync();
    }
    public async Task<CompanyParameterDto?> GetParametersAsync(int EmpCodigo)
    {
        return await _context.CompanyParameters
            .AsNoTracking()
            .Where(x => x.EmpCodigo == EmpCodigo)
            .Select(x => new CompanyParameterDto
            {
                // ==================== CLAVE PRIMARIA ====================
                EmpCodigo = x.EmpCodigo,

                // ==================== CONFIGURACIÓN CONTABLE ====================
                DefCtaNumNiveles = x.DefCtaNumNiveles,
                DefCtaNumGrupos = x.DefCtaNumGrupos,
                DefCtaNumDigNivel = x.DefCtaNumDigNivel,
                DefCtaNumNiveles1 = x.DefCtaNumNiveles1,
                DefCtaNumGrupos1 = x.DefCtaNumGrupos1,
                DefCtaNumDigNivel1 = x.DefCtaNumDigNivel1,
                DefCtaV = x.DefCtaV,

                // ==================== CIERRE CONTABLE ====================
                ParContiCierre = x.ParContiCierre,
                ParInvTipoCierre = x.ParInvTipoCierre,

                // ==================== IVA ====================
                ParVenIVA = x.ParVenIVA,
                ParComIVA = x.ParComIVA,
                ParClvIVA = x.ParClvIVA,

                // ==================== VENTAS ====================
                ParVensNEm = x.ParVensNEm,
                ParVensNAcuDoc = x.ParVensNAcuDoc,
                ParDocPrincipalVta = x.ParDocPrincipalVta,
                ParPagoCompras = x.ParPagoCompras,

                // ==================== COMPRAS ====================
                ParComSNEmp = x.ParComSNEmp,
                ParComSNAcuDoc = x.ParComSNAcuDoc,

                // ==================== PRESUPUESTOS ====================
                PrsptoNumNiveles = x.PrsptoNumNiveles,
                PrsptoNumGrupos = x.PrsptoNumGrupos,
                PrsptoNumDigNivel = x.PrsptoNumDigNivel,

                // ==================== ACUMULACIÓN HISTÓRICA ====================
                ParAcumHis = x.ParAcumHis,
                ParAcfNumNiv = x.ParAcfNumNiv,

                // ==================== ROLES Y CLAVES ====================
                ParRolCodMay = x.ParRolCodMay,
                ParRolTur = x.ParRolTur,
                ParClvDsc = x.ParClvDsc,

                // ==================== SUCURSAL PRINCIPAL ====================
                MainBranchCode = x.ParSucPri,

                // ==================== DÍGITOS ====================
                ParNumerodigitos = x.ParNumerodigitos,
                CostDigits = x.ParDigitosCostos,
                PriceDigits = x.ParDigitosPrecios,

                // ==================== FECHAS Y LÍMITES ====================
                ParFecDes = x.ParFecDes,
                LimAtrasoEntrada = x.ParLimAtrasoEntrada,
                LimExtraSalida = x.ParLimExtraSalida,
                LimExtraEntrada = x.ParLimExtraEntrada,
                ParDiasMensualesAcf = x.ParDiasMensualesAcf,
                EmpDiasMensualesAcf = x.EmpDiasMensualesAcf,

                // ==================== CHEQUES ====================
                ParCheques = x.ParCheques,

                // ==================== CRUCE DE DOCUMENTOS ====================
                ParCruceDocSucursal = x.ParCruceDocSucursal,

                // ==================== VALIDACIONES SRI ====================
                ParValiDir = x.ParValiDir,
                ParValiSRI = x.ParValiSRI,
                UrlSRI = x.ParUrlSRI,

                // ==================== PATHS Y DIRECTORIOS ====================
                ParPathImagenes = x.ParPathImagenes,
                ImagesPath = x.EmpPathImagenes,
                PathTmpServer = x.PathTmpServer,
                LongCodDirectorio = x.LongCodDirectorio,

                // ==================== CORREO ====================
                CtaLocalEmail = x.CtaLocalEmail
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<CompanyDatabaseDto>> GetDatabasesAsync(int EmpCodigo)
    {
        return await _context.CompanyDatabases
            .AsNoTracking()
            .Where(x => x.EmpCodigo == EmpCodigo)
            .OrderBy(x => x.ArchTipo)
            .Select(x => new CompanyDatabaseDto
            {
                EmpCodigo = x.EmpCodigo,
                DatabaseType = x.ArchTipo,
                DatabaseName = x.ArchNom ?? ""
            })
            .ToListAsync();
    }

    public async Task<List<AvailableDocumentDto>> GetAvailableDocumentsAsync(int EmpCodigo, string archiveType)
    {
        // 1. Obtener el nombre de la base transaccional desde Arch_Datos
        var databaseName = await _context.CompanyDatabases
            .AsNoTracking()
            .Where(x => x.EmpCodigo == EmpCodigo && x.ArchTipo == archiveType)
            .Select(x => x.ArchNom)
            .FirstOrDefaultAsync();

        if (string.IsNullOrWhiteSpace(databaseName))
            throw new InvalidOperationException($"No se encontró la base transaccional configurada para el tipo {archiveType}.");

        if (!IsSafeSqlIdentifier(databaseName))
            throw new InvalidOperationException("El nombre de la base transaccional no es válido.");

        // 2. Obtener la conexión actual y cambiar de base de datos
        var connection = _context.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        await using var command = connection.CreateCommand();

        // Usar la base de datos transaccional
        command.CommandText = $@"
            SELECT
                Opc_documento AS DocumentCode,
                Opc_nombre AS DocumentName,
                Opc_tipo AS DocumentType
            FROM [{databaseName}].dbo.AdcOpc
            ORDER BY Opc_documento;
        ";

        var result = new List<AvailableDocumentDto>();

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new AvailableDocumentDto
            {
                DocumentCode = reader["DocumentCode"]?.ToString()?.Trim() ?? "",
                DocumentName = reader["DocumentName"]?.ToString()?.Trim(),
                DocumentType = reader["DocumentType"]?.ToString()?.Trim()
            });
        }

        return result;
    }

    private static bool IsSafeSqlIdentifier(string value)
    {
        return value.All(c => char.IsLetterOrDigit(c) || c == '_');
    }
}