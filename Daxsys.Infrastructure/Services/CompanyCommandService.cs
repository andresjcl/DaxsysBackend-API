using Daxsys.Application.Companies.DTOs;
using Daxsys.Application.Companies.Interfaces;
using Daxsys.Domain.Entities;
using Daxsys.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.SqlClient;

namespace Daxsys.Infrastructure.Services;

public class CompanyCommandService : ICompanyCommandService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CompanyCommandService> _logger;

    public CompanyCommandService(
        AppDbContext context,
    IConfiguration configuration,
    ILogger<CompanyCommandService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<CreateCompanyResponseDto> CreateCompanyAsync(CreateCompanyRequestDto request)
    {
        ValidateRequest(request);

        // 1. Validar que no exista empresa con el mismo código
        var companyExists = await _context.Companies
            .AsNoTracking()
            .AnyAsync(x => x.EmpCodigo == request.Company.Id);

        if (companyExists)
            throw new InvalidOperationException("Ya existe una empresa con ese código.");

        // 2. Validar RUC único
        if (!string.IsNullOrWhiteSpace(request.Company.Ruc))
        {
            var rucExists = await _context.Companies
                .AsNoTracking()
                .AnyAsync(x => x.EmpRuc == request.Company.Ruc);

            if (rucExists)
                throw new InvalidOperationException("Ya existe una empresa con ese RUC.");
        }

        // 3. Validar que las bases de datos existan
        foreach (var database in request.Databases)
        {
            if (string.IsNullOrWhiteSpace(database.DatabaseType))
                throw new InvalidOperationException("El tipo de base es obligatorio.");

            if (string.IsNullOrWhiteSpace(database.DatabaseName))
                throw new InvalidOperationException("El nombre de la base es obligatorio.");

            var exists = await DatabaseExistsAsync(database.DatabaseName);

            if (!exists)
                throw new InvalidOperationException($"La base de datos '{database.DatabaseName}' no existe o no está ONLINE.");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // ✅ 1. CREAR EMPRESA (Emp_Datos)
            var company = new Company
            {
                EmpCodigo = request.Company.Id,
                EmpNombre = request.Company.Name,
                EmpPais = request.Company.Pais,
                EmpProvincia = request.Company.Provincia,
                EmpCiudad = request.Company.Ciudad,
                EmpCanton = request.Company.Canton,
                EmpDireccion = request.Company.Direccion,
                EmpTelefono1 = request.Company.Telefono1,
                EmpTelefono2 = request.Company.Telefono2,
                EmpEmail = request.Company.Email,
                EmpRuc = request.Company.Ruc,
                EmpDefecto = request.Company.IsDefault,
                EmpTipoBase = request.Company.TipoBase
            };

            _context.Companies.Add(company);

            // ✅ 2. CREAR PARÁMETROS (Emp_Par)
            var parameters = new CompanyParameter
            {
                EmpCodigo = request.Company.Id,
                ParContiCierre = "A",
                ParInvTipoCierre = "X",
                ParVensNEm = false,
                ParVensNAcuDoc = false,
                ParComSNEmp = false,
                ParComSNAcuDoc = false,
                ParAcumHis = false,
                ParNumerodigitos = 2,
                ParSucPri = request.MainBranch.Code,
                ParDocPrincipalVta = request.Parameters?.MainSaleDocument ?? "FAC",
                ParVenIVA = request.Parameters?.SaleIvaCode ?? "IVA12",
                ParComIVA = request.Parameters?.PurchaseIvaCode ?? "IVA12",
                ParDigitosCostos = request.Parameters?.CostDigits ?? 2,
                ParDigitosPrecios = request.Parameters?.PriceDigits ?? 2,
                EmpPathImagenes = request.Parameters?.ImagesPath
            };

            _context.CompanyParameters.Add(parameters);

            // ✅ 3. CREAR SUCURSAL (Emp_suc)
            var branch = new Branch
            {
                EmpCodigo = request.Company.Id,
                SucCodigo = request.MainBranch.Code,
                SucNombre = request.MainBranch.Name,
                SucDireccion = request.MainBranch.Address,
                SucRuc = request.MainBranch.Ruc ?? request.Company.Ruc,
                SucIdTributario = request.MainBranch.TributaryId,
                BodCodigo = request.MainWarehouse.Code
            };

            _context.Branches.Add(branch);

            // ✅ 4. CREAR BODEGA (Emp_bod)
            var warehouse = new Warehouse
            {
                EmpCodigo = request.Company.Id,
                SucCodigo = request.MainBranch.Code,
                BodCodigo = request.MainWarehouse.Code,
                BodNombre = request.MainWarehouse.Name
            };

            _context.Warehouses.Add(warehouse);

            // ✅ 5. CREAR PUNTOS DE VENTA (Emp_PtoVta)
            foreach (var point in request.PointsOfSale)
            {
                var pos = new PointOfSale
                {
                    EmpCodigo = request.Company.Id,
                    SucCodigo = request.MainBranch.Code,
                    PtoCodigo = point.Code,
                    PtoNombre = point.Name,
                    PtoIdTributario = point.TributaryId,
                    TipoPunto = point.PointType
                };

                _context.PointsOfSale.Add(pos);
            }

            // ✅ 6. CREAR BASES DE DATOS (Emp_Arch)
            foreach (var database in request.Databases)
            {
                var db = new CompanyDatabase
                {
                    EmpCodigo = request.Company.Id,
                    ArchTipo = database.DatabaseType,
                    ArchNom = database.DatabaseName
                };

                _context.CompanyDatabases.Add(db);
            }

            // ✅ UN SOLO SaveChangesAsync para TODOS los INSERT
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new CreateCompanyResponseDto
            {
                EmpCodigo = company.EmpCodigo,
                CompanyName = company.EmpNombre,
                MainBranchCode = branch.SucCodigo,
                MainWarehouseCode = warehouse.BodCodigo,
                PointsOfSaleCount = request.PointsOfSale.Count,
                DatabasesCount = request.Databases.Count
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static void ValidateRequest(CreateCompanyRequestDto request)
    {
        if (request.Company is null)
            throw new InvalidOperationException("Los datos de la empresa son obligatorios.");

        if (request.Parameters is null)
            throw new InvalidOperationException("Los parámetros de la empresa son obligatorios.");

        if (request.MainBranch is null)
            throw new InvalidOperationException("La sucursal principal es obligatoria.");

        if (request.MainWarehouse is null)
            throw new InvalidOperationException("La bodega principal es obligatoria.");

        if (request.Company.Id <= 0)
            throw new InvalidOperationException("El código de empresa debe ser mayor que cero.");

        if (string.IsNullOrWhiteSpace(request.Company.Name))
            throw new InvalidOperationException("El nombre de la empresa es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.MainBranch.Code))
            throw new InvalidOperationException("El código de la sucursal principal es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.MainWarehouse.Code))
            throw new InvalidOperationException("El código de la bodega principal es obligatorio.");

        var duplicatedPointCodes = request.PointsOfSale
            .Where(x => !string.IsNullOrWhiteSpace(x.Code))
            .GroupBy(x => x.Code)
            .Any(g => g.Count() > 1);

        if (duplicatedPointCodes)
            throw new InvalidOperationException("Hay puntos de venta repetidos en la solicitud.");

        var duplicatedDatabaseTypes = request.Databases
            .Where(x => !string.IsNullOrWhiteSpace(x.DatabaseType))
            .GroupBy(x => x.DatabaseType)
            .Any(g => g.Count() > 1);

        if (duplicatedDatabaseTypes)
            throw new InvalidOperationException("Hay tipos de base repetidos en la solicitud.");
    }

    public async Task<CompanyDetailDto> UpdateCompanyAsync(int EmpCodigo, UpdateCompanyRequestDto request)
    {
        var company = await _context.Companies
            .FirstOrDefaultAsync(x => x.EmpCodigo == EmpCodigo);

        if (company is null)
            throw new InvalidOperationException("La empresa no existe.");

        if (!string.IsNullOrWhiteSpace(request.Ruc) &&
            !string.Equals(company.EmpRuc, request.Ruc, StringComparison.OrdinalIgnoreCase))
        {
            var rucExists = await _context.Companies
                .AsNoTracking()
                .AnyAsync(x => x.EmpCodigo != EmpCodigo && x.EmpRuc == request.Ruc);

            if (rucExists)
                throw new InvalidOperationException("Ya existe otra empresa con ese RUC.");
        }

        company.EmpNombre = request.Name;
        company.EmpPais = request.Pais;
        company.EmpProvincia = request.Provincia;
        company.EmpCiudad = request.Ciudad;
        company.EmpCanton = request.Canton;
        company.EmpDireccion = request.Direccion;
        company.EmpTelefono1 = request.Telefono1;
        company.EmpTelefono2 = request.Telefono2;
        company.EmpEmail = request.Email;
        company.EmpRuc = request.Ruc;
        company.EmpDefecto = request.IsDefault;
        company.EmpTipoBase = request.TipoBase;

        await _context.SaveChangesAsync();

        return new CompanyDetailDto
        {
            Id = company.EmpCodigo,
            Name = company.EmpNombre,
            Pais = company.EmpPais,
            Provincia = company.EmpProvincia,
            Ciudad = company.EmpCiudad,
            Canton = company.EmpCanton,
            Direccion = company.EmpDireccion,
            Telefono1 = company.EmpTelefono1,
            Telefono2 = company.EmpTelefono2,
            Email = company.EmpEmail,
            Ruc = company.EmpRuc,
            IsDefault = company.EmpDefecto,
            TipoBase = company.EmpTipoBase
        };
    }

    public async Task<BranchDto> CreateBranchAsync(int EmpCodigo, CreateBranchRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
            throw new InvalidOperationException("El código de sucursal es obligatorio.");

        var companyExists = await _context.Companies
            .AsNoTracking()
            .AnyAsync(x => x.EmpCodigo == EmpCodigo);

        if (!companyExists)
            throw new InvalidOperationException("La empresa no existe.");

        var branchExists = await _context.Branches
            .AsNoTracking()
            .AnyAsync(x => x.EmpCodigo == EmpCodigo && x.SucCodigo == request.Code);

        if (branchExists)
            throw new InvalidOperationException("Ya existe una sucursal con ese código en la empresa.");

        var branch = new Branch
        {
            EmpCodigo = EmpCodigo,
            SucCodigo = request.Code,
            SucNombre = request.Name,
            SucDireccion = request.Address,
            SucRuc = request.Ruc,
            SucIdTributario = request.TributaryId,
            BodCodigo = request.DefaultWarehouseCode
        };

        _context.Branches.Add(branch);
        await _context.SaveChangesAsync();

        return new BranchDto
        {
            EmpCodigo = branch.EmpCodigo,
            Code = branch.SucCodigo,
            Name = branch.SucNombre,
            Address = branch.SucDireccion,
            Ruc = branch.SucRuc,
            DefaultWarehouseCode = branch.BodCodigo,
            TributaryId = branch.SucIdTributario
        };
    }

    public async Task<WarehouseDto> CreateWarehouseAsync(int EmpCodigo, string branchCode, CreateWarehouseRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
            throw new InvalidOperationException("El código de bodega es obligatorio.");

        var branchExists = await _context.Branches
            .AsNoTracking()
            .AnyAsync(x => x.EmpCodigo == EmpCodigo && x.SucCodigo == branchCode);

        if (!branchExists)
            throw new InvalidOperationException("La sucursal no existe.");

        var warehouseExists = await _context.Warehouses
            .AsNoTracking()
            .AnyAsync(x => x.EmpCodigo == EmpCodigo && x.SucCodigo == branchCode && x.BodCodigo == request.Code);

        if (warehouseExists)
            throw new InvalidOperationException("Ya existe una bodega con ese código en la sucursal.");

        var warehouse = new Warehouse
        {
            EmpCodigo = EmpCodigo,
            SucCodigo = branchCode,
            BodCodigo = request.Code,
            BodNombre = request.Name
        };

        _context.Warehouses.Add(warehouse);
        await _context.SaveChangesAsync();

        return new WarehouseDto
        {
            EmpCodigo = warehouse.EmpCodigo,
            BranchCode = warehouse.SucCodigo,
            Code = warehouse.BodCodigo,
            Name = warehouse.BodNombre
        };
    }

    public async Task<PointOfSaleDto> CreatePointOfSaleAsync(int EmpCodigo, string branchCode, CreatePointOfSaleRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
            throw new InvalidOperationException("El código del punto de venta es obligatorio.");

        var branchExists = await _context.Branches
            .AsNoTracking()
            .AnyAsync(x => x.EmpCodigo == EmpCodigo && x.SucCodigo == branchCode);

        if (!branchExists)
            throw new InvalidOperationException("La sucursal no existe.");

        var pointExists = await _context.PointsOfSale
            .AsNoTracking()
            .AnyAsync(x => x.EmpCodigo == EmpCodigo && x.SucCodigo == branchCode && x.PtoCodigo == request.Code);

        if (pointExists)
            throw new InvalidOperationException("Ya existe un punto de venta con ese código en la sucursal.");

        var point = new PointOfSale
        {
            EmpCodigo = EmpCodigo,
            SucCodigo = branchCode,
            PtoCodigo = request.Code,
            PtoNombre = request.Name,
            PtoIdTributario = request.TributaryId,
            TipoPunto = request.PointType
        };

        _context.PointsOfSale.Add(point);
        await _context.SaveChangesAsync();

        return new PointOfSaleDto
        {
            EmpCodigo = point.EmpCodigo,
            BranchCode = point.SucCodigo,
            Code = point.PtoCodigo,
            Name = point.PtoNombre,
            TributaryId = point.PtoIdTributario,
            PointType = point.TipoPunto
        };
    }

    public async Task<List<CompanyDatabaseDto>> UpdateCompanyDatabasesAsync(int EmpCodigo, UpdateCompanyDatabasesRequestDto request)
    {
        var companyExists = await _context.Companies
            .AsNoTracking()
            .AnyAsync(x => x.EmpCodigo == EmpCodigo);

        if (!companyExists)
            throw new InvalidOperationException("La empresa no existe.");

        var normalizedItems = request.Databases.ToList();

        var adcDatabase = normalizedItems
            .FirstOrDefault(x => x.ArchiveType.Trim().ToUpper() == "ADC")
            ?.DatabaseName
            .Trim();

        if (!string.IsNullOrWhiteSpace(adcDatabase))
        {
            var groupedTypes = new[] { "ADC", "ALE", "PRO", "ROL" };

            normalizedItems.RemoveAll(x => groupedTypes.Contains(x.ArchiveType.Trim().ToUpper()));

            foreach (var type in groupedTypes)
            {
                normalizedItems.Add(new UpdateCompanyDatabaseItemDto
                {
                    ArchiveType = type,
                    DatabaseName = adcDatabase
                });
            }
        }

        foreach (var item in normalizedItems)
        {
            if (string.IsNullOrWhiteSpace(item.ArchiveType))
                throw new InvalidOperationException("El tipo de archivo/base es obligatorio.");

            if (string.IsNullOrWhiteSpace(item.DatabaseName))
                throw new InvalidOperationException("El nombre de la base es obligatorio.");

            var databaseExists = await DatabaseExistsAsync(item.DatabaseName);

            if (!databaseExists)
                throw new InvalidOperationException($"La base de datos '{item.DatabaseName}' no existe o no está ONLINE.");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            foreach (var item in normalizedItems)
            {
                var archiveType = item.ArchiveType.Trim().ToUpper();
                var databaseName = item.DatabaseName.Trim();

                var existing = await _context.CompanyDatabases
                    .FirstOrDefaultAsync(x => x.EmpCodigo == EmpCodigo && x.ArchTipo == archiveType);

                if (existing is null)
                {
                    _context.CompanyDatabases.Add(new CompanyDatabase
                    {
                        EmpCodigo = EmpCodigo,
                        ArchTipo = archiveType,
                        ArchNom = databaseName
                    });
                }
                else
                {
                    existing.ArchNom = databaseName;
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return await _context.CompanyDatabases
                .AsNoTracking()
                .Where(x => x.EmpCodigo == EmpCodigo)
                .OrderBy(x => x.ArchTipo)
                .Select(x => new CompanyDatabaseDto
                {
                    EmpCodigo = x.EmpCodigo,
                    DatabaseType = x.ArchTipo,
                    DatabaseName = x.ArchNom
                })
                .ToListAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task<bool> DatabaseExistsAsync(string databaseName)
    {
        var connection = _context.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        await using var command = connection.CreateCommand();

        command.CommandText = @"
            SELECT COUNT(1)
            FROM sys.databases
            WHERE name = @databaseName
              AND state_desc = 'ONLINE';";

        var parameter = command.CreateParameter();
        parameter.ParameterName = "@databaseName";
        parameter.Value = databaseName.Trim();

        command.Parameters.Add(parameter);

        var result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result) > 0;
    }

    public async Task<CompanyParameterDto> UpdateCompanyParametersAsync(int EmpCodigo, UpdateCompanyParametersRequestDto request)
    {
        var companyExists = await _context.Companies
            .AsNoTracking()
            .AnyAsync(x => x.EmpCodigo == EmpCodigo);

        if (!companyExists)
            throw new InvalidOperationException("La empresa no existe.");

        // ✅ OBTENER LOS PARÁMETROS EXISTENTES DE LA BD
        var parameters = await _context.CompanyParameters
            .FirstOrDefaultAsync(x => x.EmpCodigo == EmpCodigo);

        if (parameters is null)
            throw new InvalidOperationException("No existen parámetros para esta empresa.");

        // ✅ SOLO ACTUALIZAR LOS CAMPOS QUE VIENEN EN EL REQUEST
        if (!string.IsNullOrWhiteSpace(request.MainBranchCode))
            parameters.ParSucPri = request.MainBranchCode;

        if (!string.IsNullOrWhiteSpace(request.MainSaleDocument))
            parameters.ParDocPrincipalVta = request.MainSaleDocument;

        if (!string.IsNullOrWhiteSpace(request.SaleIvaCode))
            parameters.ParVenIVA = request.SaleIvaCode;

        if (!string.IsNullOrWhiteSpace(request.PurchaseIvaCode))
            parameters.ParComIVA = request.PurchaseIvaCode;

        if (request.CostDigits.HasValue)
            parameters.ParDigitosCostos = request.CostDigits.Value;

        if (request.PriceDigits.HasValue)
            parameters.ParDigitosPrecios = request.PriceDigits.Value;

        if (!string.IsNullOrWhiteSpace(request.ImagesPath))
            parameters.EmpPathImagenes = request.ImagesPath;

        if (!string.IsNullOrWhiteSpace(request.UrlSRI))
            parameters.ParUrlSRI = request.UrlSRI;

        if (request.ValidateSRI.HasValue)
            parameters.ParValiSRI = request.ValidateSRI.Value ? (byte)1 : (byte)0;

        if (request.ValidateDirectory.HasValue)
            parameters.ParValiDir = request.ValidateDirectory.Value ? (byte)1 : (byte)0;

        if (request.LimAtrasoEntrada.HasValue)
            parameters.ParLimAtrasoEntrada = request.LimAtrasoEntrada.Value;

        if (request.LimExtraSalida.HasValue)
            parameters.ParLimExtraSalida = request.LimExtraSalida.Value;

        if (request.LimExtraEntrada.HasValue)
            parameters.ParLimExtraEntrada = request.LimExtraEntrada.Value;

        if (request.Cheques.HasValue)
            parameters.ParCheques = request.Cheques.Value;

        if (!string.IsNullOrWhiteSpace(request.PagoCompras))
            parameters.ParPagoCompras = request.PagoCompras;

        if (!string.IsNullOrWhiteSpace(request.ClvDsc))
            parameters.ParClvDsc = request.ClvDsc;

        if (!string.IsNullOrWhiteSpace(request.ClvIVA))
            parameters.ParClvIVA = request.ClvIVA;

        await _context.SaveChangesAsync();

        // ✅ RETORNAR TODOS LOS CAMPOS - USAR LOS NOMBRES CORRECTOS DEL DTO
        return new CompanyParameterDto
        {
            EmpCodigo = parameters.EmpCodigo,

            // Contabilidad
            DefCtaNumNiveles = parameters.DefCtaNumNiveles,
            DefCtaNumGrupos = parameters.DefCtaNumGrupos,
            DefCtaNumDigNivel = parameters.DefCtaNumDigNivel,
            DefCtaNumNiveles1 = parameters.DefCtaNumNiveles1,
            DefCtaNumGrupos1 = parameters.DefCtaNumGrupos1,
            DefCtaNumDigNivel1 = parameters.DefCtaNumDigNivel1,
            DefCtaV = parameters.DefCtaV,

            // Cierre contable
            ParContiCierre = parameters.ParContiCierre,
            ParInvTipoCierre = parameters.ParInvTipoCierre,

            // IVA
            ParVenIVA = parameters.ParVenIVA,
            ParComIVA = parameters.ParComIVA,
            ParClvIVA = parameters.ParClvIVA,

            // Ventas
            ParVensNEm = parameters.ParVensNEm,
            ParVensNAcuDoc = parameters.ParVensNAcuDoc,
            ParDocPrincipalVta = parameters.ParDocPrincipalVta,
            ParPagoCompras = parameters.ParPagoCompras,

            // Compras
            ParComSNEmp = parameters.ParComSNEmp,
            ParComSNAcuDoc = parameters.ParComSNAcuDoc,

            // Presupuestos
            PrsptoNumNiveles = parameters.PrsptoNumNiveles,
            PrsptoNumGrupos = parameters.PrsptoNumGrupos,
            PrsptoNumDigNivel = parameters.PrsptoNumDigNivel,

            // Acumulación
            ParAcumHis = parameters.ParAcumHis,
            ParAcfNumNiv = parameters.ParAcfNumNiv,

            // Roles y claves
            ParRolCodMay = parameters.ParRolCodMay,
            ParRolTur = parameters.ParRolTur,
            ParClvDsc = parameters.ParClvDsc,

            // Sucursal principal
            MainBranchCode = parameters.ParSucPri,

            // Dígitos
            ParNumerodigitos = parameters.ParNumerodigitos,
            CostDigits = parameters.ParDigitosCostos,
            PriceDigits = parameters.ParDigitosPrecios,

            // Fechas y límites
            ParFecDes = parameters.ParFecDes,
            LimAtrasoEntrada = parameters.ParLimAtrasoEntrada,
            LimExtraSalida = parameters.ParLimExtraSalida,
            LimExtraEntrada = parameters.ParLimExtraEntrada,
            ParDiasMensualesAcf = parameters.ParDiasMensualesAcf,
            EmpDiasMensualesAcf = parameters.EmpDiasMensualesAcf,

            // Cheques
            ParCheques = parameters.ParCheques,

            // Cruce de documentos
            ParCruceDocSucursal = parameters.ParCruceDocSucursal,

            // Validaciones SRI
            ParValiDir = parameters.ParValiDir,
            ParValiSRI = parameters.ParValiSRI,
            UrlSRI = parameters.ParUrlSRI,

            // Paths y directorios
            ParPathImagenes = parameters.ParPathImagenes,
            ImagesPath = parameters.EmpPathImagenes,
            PathTmpServer = parameters.PathTmpServer,
            LongCodDirectorio = parameters.LongCodDirectorio,

            // Correo
            CtaLocalEmail = parameters.CtaLocalEmail
        };
    }

    public async Task DeleteCompanyAsync(int EmpCodigo)
    {
        var companyExists = await _context.Companies
            .AsNoTracking()
            .AnyAsync(x => x.EmpCodigo == EmpCodigo);

        if (!companyExists)
            throw new InvalidOperationException("La empresa no existe.");

        // 1. Obtener las bases de datos asociadas a la empresa
        var databases = await _context.CompanyDatabases
            .AsNoTracking()
            .Where(x => x.EmpCodigo == EmpCodigo)
            .ToListAsync();

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 2. Eliminar las bases de datos físicas (ADC y SRI)
            foreach (var db in databases)
            {
                // Solo eliminar bases de datos tipo ADC y SRI
                if (db.ArchTipo == "ADC" || db.ArchTipo == "SRI")
                {
                    await DropDatabaseAsync(db.ArchNom);
                    _logger.LogInformation("Base de datos eliminada: {DatabaseName}", db.ArchNom);
                }
            }

            // 3. Eliminar registros de la empresa en todas las tablas
            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            DELETE FROM dbo.sysdocaccs WHERE empresa = {EmpCodigo}");

            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            DELETE FROM dbo.sys_Documentos WHERE idEmpresa = {EmpCodigo}");

            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            DELETE FROM dbo.sys_Accesos WHERE IdEmpresa = {EmpCodigo}");

            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            DELETE FROM dbo.sys_ptoVta WHERE IdEmpresa = {EmpCodigo}");

            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            DELETE FROM dbo.sys_Bodegas WHERE IdEmpresa = {EmpCodigo}");

            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            DELETE FROM dbo.sys_Sucursales WHERE IdEmpresa = {EmpCodigo}");

            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            DELETE FROM dbo.Emp_PtoVta WHERE Emp_Codigo = {EmpCodigo}");

            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            DELETE FROM dbo.Emp_Bod WHERE Emp_Codigo = {EmpCodigo}");

            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            DELETE FROM dbo.Emp_Suc WHERE Emp_Codigo = {EmpCodigo}");

            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            DELETE FROM dbo.Emp_Arch WHERE Emp_Codigo = {EmpCodigo}");

            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            DELETE FROM dbo.Emp_Par WHERE Emp_Codigo = {EmpCodigo}");

            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            DELETE FROM dbo.Emp_Datos WHERE Emp_codigo = {EmpCodigo}");

            await transaction.CommitAsync();

            _logger.LogInformation("Empresa eliminada: {EmpCodigo}", EmpCodigo);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task DropDatabaseAsync(string databaseName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(databaseName))
                return;

            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            var masterConnection = connectionString.Replace("BdAdcomDx14DGC", "master");

            await using var connection = new SqlConnection(masterConnection);
            await connection.OpenAsync();

            // 1. Forzar cierre de conexiones
            var forceCloseCmd = new SqlCommand(
                $@"
            IF EXISTS (SELECT * FROM sys.databases WHERE name = '{databaseName}')
            BEGIN
                ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
            END
            ",
                connection);
            await forceCloseCmd.ExecuteNonQueryAsync();

            // 2. Eliminar la base de datos
            var dropCmd = new SqlCommand(
                $@"
            IF EXISTS (SELECT * FROM sys.databases WHERE name = '{databaseName}')
            BEGIN
                DROP DATABASE [{databaseName}];
            END
            ",
                connection);
            await dropCmd.ExecuteNonQueryAsync();

            _logger.LogInformation("Base de datos eliminada: {DatabaseName}", databaseName);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error al eliminar la base de datos: {DatabaseName}", databaseName);
            // No lanzamos excepción para no interrumpir el proceso
        }
    }
}