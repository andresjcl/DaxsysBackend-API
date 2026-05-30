using Daxsys.Application.Companies.DTOs;
using Daxsys.Application.Companies.Interfaces;
using Daxsys.Domain.Entities;
using Daxsys.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Daxsys.Infrastructure.Services;

public class CompanyCommandService : ICompanyCommandService
{
    private readonly AppDbContext _context;

    public CompanyCommandService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CreateCompanyResponseDto> CreateCompanyAsync(CreateCompanyRequestDto request)
    {
        ValidateRequest(request);

        var companyExists = await _context.Companies
            .AsNoTracking()
            .AnyAsync(x => x.EmpCodigo == request.Company.Id);

        if (companyExists)
            throw new InvalidOperationException("Ya existe una empresa con ese código.");

        if (!string.IsNullOrWhiteSpace(request.Company.Ruc))
        {
            var rucExists = await _context.Companies
                .AsNoTracking()
                .AnyAsync(x => x.EmpRuc == request.Company.Ruc);

            if (rucExists)
                throw new InvalidOperationException("Ya existe una empresa con ese RUC.");
        }

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
            // 1. EMP_DATOS
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
            await _context.SaveChangesAsync();

            // 2. EMP_PAR
            var parameter = new CompanyParameter
            {
                EmpCodigo = request.Company.Id,
                ParConTipCierre = "A",
                ParInvTipCierre = "X",
                ParVenSNEmp = false,
                ParVenSNAcuDoc = false,
                ParComSNEmp = false,
                ParComSNAcuDoc = false,
                ParAcumHis = false,
                ParSucPri = request.Parameters.MainBranchCode ?? request.MainBranch.Code,
                ParDocPrincipalVta = request.Parameters.MainSaleDocument,
                ParVenIVA = request.Parameters.SaleIvaCode,
                ParComIVA = request.Parameters.PurchaseIvaCode,
                ParDigitosCostos = request.Parameters.CostDigits,
                ParDigitosPrecios = request.Parameters.PriceDigits,
                EmpPathImagenes = request.Parameters.ImagesPath
            };

            _context.CompanyParameters.Add(parameter);

            // 3. EMP_SUC
            var branch = new Branch
            {
                EmpCodigo = request.Company.Id,
                SucCodigo = request.MainBranch.Code,
                SucNombre = request.MainBranch.Name,
                SucDireccion = request.MainBranch.Address,
                SucRuc = request.MainBranch.Ruc,
                BodCodigo = request.MainWarehouse.Code,
                SucIdTributario = request.MainBranch.TributaryId
            };

            _context.Branches.Add(branch);
            await _context.SaveChangesAsync();

            // 4. EMP_BOD
            var warehouse = new Warehouse
            {
                EmpCodigo = request.Company.Id,
                SucCodigo = request.MainBranch.Code,
                BodCodigo = request.MainWarehouse.Code,
                BodNombre = request.MainWarehouse.Name
            };

            _context.Warehouses.Add(warehouse);

            // 5. EMP_PTOVTA
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

            // 6. EMP_ARCH
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

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new CreateCompanyResponseDto
            {
                CompanyId = company.EmpCodigo,
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

    public async Task<CompanyDetailDto> UpdateCompanyAsync(int companyId, UpdateCompanyRequestDto request)
    {
        var company = await _context.Companies
            .FirstOrDefaultAsync(x => x.EmpCodigo == companyId);

        if (company is null)
            throw new InvalidOperationException("La empresa no existe.");

        if (!string.IsNullOrWhiteSpace(request.Ruc) &&
            !string.Equals(company.EmpRuc, request.Ruc, StringComparison.OrdinalIgnoreCase))
        {
            var rucExists = await _context.Companies
                .AsNoTracking()
                .AnyAsync(x => x.EmpCodigo != companyId && x.EmpRuc == request.Ruc);

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

    public async Task<BranchDto> CreateBranchAsync(int companyId, CreateBranchRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
            throw new InvalidOperationException("El código de sucursal es obligatorio.");

        var companyExists = await _context.Companies
            .AsNoTracking()
            .AnyAsync(x => x.EmpCodigo == companyId);

        if (!companyExists)
            throw new InvalidOperationException("La empresa no existe.");

        var branchExists = await _context.Branches
            .AsNoTracking()
            .AnyAsync(x => x.EmpCodigo == companyId && x.SucCodigo == request.Code);

        if (branchExists)
            throw new InvalidOperationException("Ya existe una sucursal con ese código en la empresa.");

        var branch = new Branch
        {
            EmpCodigo = companyId,
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
            CompanyId = branch.EmpCodigo,
            Code = branch.SucCodigo,
            Name = branch.SucNombre,
            Address = branch.SucDireccion,
            Ruc = branch.SucRuc,
            DefaultWarehouseCode = branch.BodCodigo,
            TributaryId = branch.SucIdTributario
        };
    }

    public async Task<WarehouseDto> CreateWarehouseAsync(int companyId, string branchCode, CreateWarehouseRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
            throw new InvalidOperationException("El código de bodega es obligatorio.");

        var branchExists = await _context.Branches
            .AsNoTracking()
            .AnyAsync(x => x.EmpCodigo == companyId && x.SucCodigo == branchCode);

        if (!branchExists)
            throw new InvalidOperationException("La sucursal no existe.");

        var warehouseExists = await _context.Warehouses
            .AsNoTracking()
            .AnyAsync(x => x.EmpCodigo == companyId && x.SucCodigo == branchCode && x.BodCodigo == request.Code);

        if (warehouseExists)
            throw new InvalidOperationException("Ya existe una bodega con ese código en la sucursal.");

        var warehouse = new Warehouse
        {
            EmpCodigo = companyId,
            SucCodigo = branchCode,
            BodCodigo = request.Code,
            BodNombre = request.Name
        };

        _context.Warehouses.Add(warehouse);
        await _context.SaveChangesAsync();

        return new WarehouseDto
        {
            CompanyId = warehouse.EmpCodigo,
            BranchCode = warehouse.SucCodigo,
            Code = warehouse.BodCodigo,
            Name = warehouse.BodNombre
        };
    }

    public async Task<PointOfSaleDto> CreatePointOfSaleAsync(int companyId, string branchCode, CreatePointOfSaleRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
            throw new InvalidOperationException("El código del punto de venta es obligatorio.");

        var branchExists = await _context.Branches
            .AsNoTracking()
            .AnyAsync(x => x.EmpCodigo == companyId && x.SucCodigo == branchCode);

        if (!branchExists)
            throw new InvalidOperationException("La sucursal no existe.");

        var pointExists = await _context.PointsOfSale
            .AsNoTracking()
            .AnyAsync(x => x.EmpCodigo == companyId && x.SucCodigo == branchCode && x.PtoCodigo == request.Code);

        if (pointExists)
            throw new InvalidOperationException("Ya existe un punto de venta con ese código en la sucursal.");

        var point = new PointOfSale
        {
            EmpCodigo = companyId,
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
            CompanyId = point.EmpCodigo,
            BranchCode = point.SucCodigo,
            Code = point.PtoCodigo,
            Name = point.PtoNombre,
            TributaryId = point.PtoIdTributario,
            PointType = point.TipoPunto
        };
    }

    public async Task<List<CompanyDatabaseDto>> UpdateCompanyDatabasesAsync(int companyId,UpdateCompanyDatabasesRequestDto request)
    {
        var companyExists = await _context.Companies
            .AsNoTracking()
            .AnyAsync(x => x.EmpCodigo == companyId);

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
                    .FirstOrDefaultAsync(x => x.EmpCodigo == companyId && x.ArchTipo == archiveType);

                if (existing is null)
                {
                    _context.CompanyDatabases.Add(new CompanyDatabase
                    {
                        EmpCodigo = companyId,
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
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

   private async Task<bool> DatabaseExistsAsync(string databaseName)
    {
        var connection = _context.Database.GetDbConnection();

        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        await using var command = connection.CreateCommand();

        command.CommandText = @"
        SELECT COUNT(1)
        FROM sys.databases
        WHERE name = @databaseName
          AND state_desc = 'ONLINE';
    ";

        var parameter = command.CreateParameter();
        parameter.ParameterName = "@databaseName";
        parameter.Value = databaseName.Trim();

        command.Parameters.Add(parameter);

        var result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result) > 0;
    }

    public async Task<CompanyParameterDto> UpdateCompanyParametersAsync(int companyId,UpdateCompanyParametersRequestDto request)
    {
        var companyExists = await _context.Companies
            .AsNoTracking()
            .AnyAsync(x => x.EmpCodigo == companyId);

        if (!companyExists)
            throw new InvalidOperationException("La empresa no existe.");

        if (!string.IsNullOrWhiteSpace(request.MainBranchCode))
        {
            var branchExists = await _context.Branches
                .AsNoTracking()
                .AnyAsync(x => x.EmpCodigo == companyId && x.SucCodigo == request.MainBranchCode);

            if (!branchExists)
                throw new InvalidOperationException("La sucursal principal no existe.");
        }

        var parameters = await _context.CompanyParameters
            .FirstOrDefaultAsync(x => x.EmpCodigo == companyId);

        if (parameters is null)
        {
            parameters = new CompanyParameter
            {
                EmpCodigo = companyId,
                ParConTipCierre = "A",
                ParInvTipCierre = "X",
                ParVenSNEmp = false,
                ParVenSNAcuDoc = false,
                ParComSNEmp = false,
                ParComSNAcuDoc = false,
                ParAcumHis = false
            };

            _context.CompanyParameters.Add(parameters);
        }

        parameters.ParSucPri = request.MainBranchCode;
        parameters.ParDocPrincipalVta = request.MainSaleDocument;
        parameters.ParVenIVA = request.SaleIvaCode;
        parameters.ParComIVA = request.PurchaseIvaCode;
        parameters.ParDigitosCostos = request.CostDigits;
        parameters.ParDigitosPrecios = request.PriceDigits;
        parameters.EmpPathImagenes = request.ImagesPath;

        await _context.SaveChangesAsync();

        return new CompanyParameterDto
        {
            CompanyId = parameters.EmpCodigo,
            MainBranchCode = parameters.ParSucPri,
            MainSaleDocument = parameters.ParDocPrincipalVta,
            SaleIvaCode = parameters.ParVenIVA,
            PurchaseIvaCode = parameters.ParComIVA,
            CostDigits = parameters.ParDigitosCostos,
            PriceDigits = parameters.ParDigitosPrecios,
            ImagesPath = parameters.EmpPathImagenes
        };
    }

    public async Task DeleteCompanyAsync(int companyId)
    {
        var companyExists = await _context.Companies
            .AsNoTracking()
            .AnyAsync(x => x.EmpCodigo == companyId);

        if (!companyExists)
            throw new InvalidOperationException("La empresa no existe.");

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            DELETE FROM dbo.sysdocaccs
            WHERE empresa = {companyId}
        ");

            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            DELETE FROM dbo.sys_Documentos
            WHERE idEmpresa = {companyId}
        ");

            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            DELETE FROM dbo.sys_Accesos
            WHERE IdEmpresa = {companyId}
        ");

            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            DELETE FROM dbo.sys_ptoVta
            WHERE IdEmpresa = {companyId}
        ");

            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            DELETE FROM dbo.sys_Bodegas
            WHERE IdEmpresa = {companyId}
        ");

            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            DELETE FROM dbo.sys_Sucursales
            WHERE IdEmpresa = {companyId}
        ");

            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            DELETE FROM dbo.Emp_PtoVta
            WHERE Emp_Codigo = {companyId}
        ");

            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            DELETE FROM dbo.Emp_Bod
            WHERE Emp_Codigo = {companyId}
        ");

            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            DELETE FROM dbo.Emp_Suc
            WHERE Emp_Codigo = {companyId}
        ");

            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            DELETE FROM dbo.Emp_Arch
            WHERE Emp_Codigo = {companyId}
        ");

            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            DELETE FROM dbo.Emp_Par
            WHERE Emp_Codigo = {companyId}
        ");

            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            DELETE FROM dbo.Emp_Datos
            WHERE Emp_codigo = {companyId}
        ");

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}