using Daxsys.Application.Auth.DTOs;
using Daxsys.Application.Common.Interfaces;
using Daxsys.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Daxsys.Infrastructure.Services;

public class UserContextService : Daxsys.Application.Auth.Interfaces.IUserContextService
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public UserContextService(AppDbContext context,ITokenService tokenService,IConfiguration configuration)
    {
        _context = context;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    public async Task<AuthContextResponseDto?> GetContextAsync(string userId, int companyId, string systemId)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdUsuario == userId);

        if (user is null)
            return null;

        var company = await _context.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.EmpCodigo == companyId);

        if (company is null)
            return null;

        var isAdmin = string.Equals(userId, "administrador", StringComparison.OrdinalIgnoreCase);

        List<AuthContextBranchDto> branches;
        List<AuthContextWarehouseDto> warehouses;
        List<AuthContextPointOfSaleDto> pointsOfSale;

        if (isAdmin)
        {
            branches = await _context.Branches
                .AsNoTracking()
                .Where(x => x.EmpCodigo == companyId)
                .OrderBy(x => x.SucCodigo)
                .Select(x => new AuthContextBranchDto
                {
                    Code = x.SucCodigo,
                    Name = x.SucNombre
                })
                .ToListAsync();

            warehouses = await _context.Warehouses
                .AsNoTracking()
                .Where(x => x.EmpCodigo == companyId)
                .OrderBy(x => x.SucCodigo)
                .ThenBy(x => x.BodCodigo)
                .Select(x => new AuthContextWarehouseDto
                {
                    BranchCode = x.SucCodigo,
                    Code = x.BodCodigo,
                    Name = x.BodNombre
                })
                .ToListAsync();

            pointsOfSale = await _context.PointsOfSale
                .AsNoTracking()
                .Where(x => x.EmpCodigo == companyId)
                .OrderBy(x => x.SucCodigo)
                .ThenBy(x => x.PtoCodigo)
                .Select(x => new AuthContextPointOfSaleDto
                {
                    BranchCode = x.SucCodigo,
                    Code = x.PtoCodigo,
                    Name = x.PtoNombre
                })
                .ToListAsync();
        }
        else
        {
            var branchCodes = await _context.UserBranches
                .AsNoTracking()
                .Where(x => x.IdUsuario == userId
                         && x.IdEmpresa == companyId
                         && x.AutorizaSuc == "S")
                .Select(x => x.CodSucursal)
                .ToListAsync();

            branches = await _context.Branches
                .AsNoTracking()
                .Where(x => x.EmpCodigo == companyId && branchCodes.Contains(x.SucCodigo))
                .OrderBy(x => x.SucCodigo)
                .Select(x => new AuthContextBranchDto
                {
                    Code = x.SucCodigo,
                    Name = x.SucNombre
                })
                .ToListAsync();

            var allowedWarehouses = await _context.UserWarehouses
                .AsNoTracking()
                .Where(x => x.IdUsuario == userId
                         && x.IdEmpresa == companyId
                         && x.AutorizaBod == "S")
                .Select(x => new
                {
                    BranchCode = x.CodSucursal,
                    WarehouseCode = x.CodBodega
                })
                .ToListAsync();

            var warehouseKeys = allowedWarehouses
                .Select(x => $"{x.BranchCode}|{x.WarehouseCode}")
                .ToHashSet();

            var companyWarehouses = await _context.Warehouses
                .AsNoTracking()
                .Where(x => x.EmpCodigo == companyId)
                .OrderBy(x => x.SucCodigo)
                .ThenBy(x => x.BodCodigo)
                .Select(x => new
                {
                    x.SucCodigo,
                    x.BodCodigo,
                    x.BodNombre
                })
                .ToListAsync();

            warehouses = companyWarehouses
                .Where(x => warehouseKeys.Contains($"{x.SucCodigo}|{x.BodCodigo}"))
                .Select(x => new AuthContextWarehouseDto
                {
                    BranchCode = x.SucCodigo,
                    Code = x.BodCodigo,
                    Name = x.BodNombre
                })
                .ToList();

            var allowedPoints = await _context.UserPointsOfSales
                .AsNoTracking()
                .Where(x => x.IdUsuario == userId
                         && x.IdEmpresa == companyId
                         && x.AutorizaPtoVta == "S")
                .Select(x => new
                {
                    BranchCode = x.CodSucursal,
                    PointCode = x.CodPtoVta
                })
                .ToListAsync();

            var pointKeys = allowedPoints
                .Select(x => $"{x.BranchCode}|{x.PointCode}")
                .ToHashSet();

            var companyPoints = await _context.PointsOfSale
                .AsNoTracking()
                .Where(x => x.EmpCodigo == companyId)
                .OrderBy(x => x.SucCodigo)
                .ThenBy(x => x.PtoCodigo)
                .Select(x => new
                {
                    x.SucCodigo,
                    x.PtoCodigo,
                    x.PtoNombre
                })
                .ToListAsync();

            pointsOfSale = companyPoints
                .Where(x => pointKeys.Contains($"{x.SucCodigo}|{x.PtoCodigo}"))
                .Select(x => new AuthContextPointOfSaleDto
                {
                    BranchCode = x.SucCodigo,
                    Code = x.PtoCodigo,
                    Name = x.PtoNombre
                })
                .ToList();
        }

        var menus = await _context.Menus
            .AsNoTracking()
            .Where(x => x.IdSistema == systemId && x.Activo)
            .OrderBy(x => x.Orden)
            .ToListAsync();

        List<string> allowedCodes;

        if (isAdmin)
        {
            allowedCodes = menus.Select(x => x.Codigo).ToList();
        }
        else
        {
            allowedCodes = await _context.UserAccesses
                .AsNoTracking()
                .Where(x => x.IdUsuario == userId
                         && x.IdEmpresa == companyId
                         && x.IdSistema == systemId
                         && x.Accesos == "T")
                .Select(x => x.IdOpcion!)
                .ToListAsync();
        }

        List<AuthContextMenuDto> BuildTree(int? parentId)
        {
            return menus
                .Where(x => x.IdPadre == parentId)
                .OrderBy(x => x.Orden)
                .Select(x => new AuthContextMenuDto
                {
                    IdMenu = x.IdMenu,
                    Codigo = x.Codigo,
                    Nombre = x.Nombre,
                    Ruta = x.Ruta,
                    Icono = x.Icono,
                    HasAccess = allowedCodes.Contains(x.Codigo),
                    Children = BuildTree(x.IdMenu)
                })
                .ToList();
        }

        var menuTree = BuildTree(null);

        List<AuthContextDocumentDto> documents;

        if (isAdmin)
        {
            var databaseName = await _context.CompanyDatabases
                .AsNoTracking()
                .Where(x => x.EmpCodigo == companyId && x.ArchTipo == systemId)
                .Select(x => x.ArchNom)
                .FirstOrDefaultAsync();

            databaseName = databaseName?.Trim();

            var connection = _context.Database.GetDbConnection();

            if (connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();

            await using var command = connection.CreateCommand();

            command.CommandText = $@"
        SELECT Opc_documento
        FROM [{databaseName}].dbo.AdcOpc
    ";

            var list = new List<AuthContextDocumentDto>();

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new AuthContextDocumentDto
                {
                    DocumentCode = reader["Opc_documento"].ToString()!,
                    Changes = "T"
                });
            }

            documents = list;
        }
        else
        {
            documents = await _context.UserDocuments
                .AsNoTracking()
                .Where(x => x.IdUsuario == userId
                         && x.IdEmpresa == companyId
                         && x.Cambios == "T")
                .OrderBy(x => x.CodDocumento)
                .Select(x => new AuthContextDocumentDto
                {
                    DocumentCode = x.CodDocumento,
                    Changes = x.Cambios
                })
                .ToListAsync();
        };


        // ===============================
        // CONTEXTO POR DEFECTO (Par_SucPri)
        // ===============================
        var companyParameter = await _context.CompanyParameters
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.EmpCodigo == companyId);

        var defaultBranchId = companyParameter?.ParSucPri;

        // Si no existe o no está permitida, toma la primera disponible
        if (string.IsNullOrWhiteSpace(defaultBranchId) ||
            !branches.Any(x => x.Code == defaultBranchId))
        {
            defaultBranchId = branches.FirstOrDefault()?.Code;
        }

        // Bodega por defecto desde Emp_Suc
        string? defaultWarehouseId = null;

        if (!string.IsNullOrWhiteSpace(defaultBranchId))
        {
            defaultWarehouseId = await _context.Branches
                .AsNoTracking()
                .Where(x => x.EmpCodigo == companyId && x.SucCodigo == defaultBranchId)
                .Select(x => x.BodCodigo)
                .FirstOrDefaultAsync();

            // validar que exista en las bodegas permitidas
            if (!warehouses.Any(x => x.BranchCode == defaultBranchId && x.Code == defaultWarehouseId))
            {
                defaultWarehouseId = warehouses
                    .FirstOrDefault(x => x.BranchCode == defaultBranchId)
                    ?.Code;
            }
        }

        // Punto de venta por defecto
        var defaultPointOfSaleId = pointsOfSale
            .FirstOrDefault(x => x.BranchCode == defaultBranchId)
            ?.Code;

        return new AuthContextResponseDto
        {
            UserId = userId,
            IsAdmin = isAdmin,
            CompanyId = company.EmpCodigo,
            CompanyName = company.EmpNombre,
            Branches = branches,
            Warehouses = warehouses,
            PointsOfSale = pointsOfSale,
            Menu = menuTree,
            Documents = documents,

            DefaultBranchId = defaultBranchId,
            DefaultWarehouseId = defaultWarehouseId,
            DefaultPointOfSaleId = defaultPointOfSaleId
        };
    }

    public async Task<SelectContextResponseDto?> SelectContextAsync(string userId,SelectContextRequestDto request)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdUsuario == userId);

        if (user is null)
            return null;

        var isAdmin = string.Equals(userId, "administrador", StringComparison.OrdinalIgnoreCase);

        var company = await _context.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.EmpCodigo == request.CompanyId);

        if (company is null)
            throw new InvalidOperationException("La empresa no existe.");

        if (string.IsNullOrWhiteSpace(request.BranchId))
            throw new InvalidOperationException("Debe seleccionar una sucursal.");

        var branch = await _context.Branches
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.EmpCodigo == request.CompanyId
                                   && x.SucCodigo == request.BranchId);

        if (branch is null)
            throw new InvalidOperationException("La sucursal no existe.");

        if (!isAdmin)
        {
            var hasBranchAccess = await _context.UserBranches
                .AsNoTracking()
                .AnyAsync(x => x.IdUsuario == userId
                            && x.IdEmpresa == request.CompanyId
                            && x.CodSucursal == request.BranchId
                            && x.AutorizaSuc == "S");

            if (!hasBranchAccess)
                throw new InvalidOperationException("El usuario no tiene acceso a la sucursal seleccionada.");
        }

        string? warehouseName = null;

        if (!string.IsNullOrWhiteSpace(request.WarehouseId))
        {
            var warehouse = await _context.Warehouses
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.EmpCodigo == request.CompanyId
                                       && x.SucCodigo == request.BranchId
                                       && x.BodCodigo == request.WarehouseId);

            if (warehouse is null)
                throw new InvalidOperationException("La bodega no existe en la sucursal seleccionada.");

            if (!isAdmin)
            {
                var hasWarehouseAccess = await _context.UserWarehouses
                    .AsNoTracking()
                    .AnyAsync(x => x.IdUsuario == userId
                                && x.IdEmpresa == request.CompanyId
                                && x.CodSucursal == request.BranchId
                                && x.CodBodega == request.WarehouseId
                                && x.AutorizaBod == "S");

                if (!hasWarehouseAccess)
                    throw new InvalidOperationException("El usuario no tiene acceso a la bodega seleccionada.");
            }

            warehouseName = warehouse.BodNombre;
        }

        string? pointOfSaleName = null;

        if (!string.IsNullOrWhiteSpace(request.PointOfSaleId))
        {
            var pointOfSale = await _context.PointsOfSale
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.EmpCodigo == request.CompanyId
                                       && x.SucCodigo == request.BranchId
                                       && x.PtoCodigo == request.PointOfSaleId);

            if (pointOfSale is null)
                throw new InvalidOperationException("El punto de venta no existe en la sucursal seleccionada.");

            if (!isAdmin)
            {
                var hasPointAccess = await _context.UserPointsOfSales
                    .AsNoTracking()
                    .AnyAsync(x => x.IdUsuario == userId
                                && x.IdEmpresa == request.CompanyId
                                && x.CodSucursal == request.BranchId
                                && x.CodPtoVta == request.PointOfSaleId
                                && x.AutorizaPtoVta == "S");

                if (!hasPointAccess)
                    throw new InvalidOperationException("El usuario no tiene acceso al punto de venta seleccionado.");
            }

            pointOfSaleName = pointOfSale.PtoNombre;
        }

        var expirationMinutesText = _configuration["Jwt:ExpirationMinutes"];
        var expirationMinutes = int.TryParse(expirationMinutesText, out var parsed)
            ? parsed
            : 480;

        var expiresAt = DateTime.Now.AddMinutes(expirationMinutes);

        var contextToken = _tokenService.GenerateContextToken(
            userId,
            isAdmin,
            company.EmpCodigo,
            branch.SucCodigo,
            request.WarehouseId,
            request.PointOfSaleId,
            expiresAt);


        return new SelectContextResponseDto
        {
            UserId = userId,
            IsAdmin = isAdmin,
            CompanyId = company.EmpCodigo,
            CompanyName = company.EmpNombre,
            BranchId = branch.SucCodigo,
            BranchName = branch.SucNombre,
            WarehouseId = request.WarehouseId,
            WarehouseName = warehouseName,
            PointOfSaleId = request.PointOfSaleId,
            PointOfSaleName = pointOfSaleName,
            ContextToken = contextToken,
            ExpiresAt = expiresAt
        };
    }


}