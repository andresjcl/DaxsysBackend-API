using Daxsys.Application.Users.DTOs;
using Daxsys.Application.Users.Interfaces;
using Daxsys.Domain.Entities;
using Daxsys.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Daxsys.Infrastructure.Services;

public class UserManagementService : IUserManagementService
{
    private readonly AppDbContext _context;

    public UserManagementService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserListItemDto>> GetUsersAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .OrderBy(x => x.IdUsuario)
            .Select(x => new UserListItemDto
            {
                Id = x.IdUsuario,
                Code = x.CodUsuario,
                StartDate = x.FechaInicio,
                EndDate = x.FechaCaduca,
                PasswordChangeDate = x.FechaCambioContra,
                PasswordDurationDays = x.DiasDuraContrasena,
                IsAdmin = x.IdUsuario.ToLower() == "administrador"
            })
            .ToListAsync();
    }

    public async Task<UserDetailDto?> GetUserByIdAsync(string userId)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(x => x.IdUsuario == userId)
            .Select(x => new UserDetailDto
            {
                Id = x.IdUsuario,
                Code = x.CodUsuario,
                StartDate = x.FechaInicio,
                EndDate = x.FechaCaduca,
                PasswordChangeDate = x.FechaCambioContra,
                PasswordDurationDays = x.DiasDuraContrasena,
                IsAdmin = x.IdUsuario.ToLower() == "administrador"
            })
            .FirstOrDefaultAsync();
    }

    public async Task<UserDetailDto> CreateUserAsync(CreateUserRequestDto request)
    {
        ValidateCreateRequest(request);

        var exists = await _context.Users
            .AsNoTracking()
            .AnyAsync(x => x.IdUsuario == request.Id);

        if (exists)
            throw new InvalidOperationException("Ya existe un usuario con ese id.");

        var user = new User
        {
            IdUsuario = request.Id,
            CodUsuario = request.Code,
            FechaInicio = request.StartDate ?? DateTime.Today,
            FechaCaduca = request.EndDate ?? new DateTime(9999, 12, 31),
            Contrasena = request.Password,
            FechaCambioContra = request.PasswordChangeDate ?? DateTime.Today,
            DiasDuraContrasena = request.PasswordDurationDays ?? 90
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserDetailDto
        {
            Id = user.IdUsuario,
            Code = user.CodUsuario,
            StartDate = user.FechaInicio,
            EndDate = user.FechaCaduca,
            PasswordChangeDate = user.FechaCambioContra,
            PasswordDurationDays = user.DiasDuraContrasena,
            IsAdmin = string.Equals(user.IdUsuario, "administrador", StringComparison.OrdinalIgnoreCase)
        };
    }

    public async Task<UserDetailDto> UpdateUserAsync(string userId, UpdateUserRequestDto request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.IdUsuario == userId);

        if (user is null)
            throw new InvalidOperationException("El usuario no existe.");

        user.CodUsuario = request.Code;
        user.FechaInicio = request.StartDate;
        user.FechaCaduca = request.EndDate;
        user.DiasDuraContrasena = request.PasswordDurationDays;

        await _context.SaveChangesAsync();

        return new UserDetailDto
        {
            Id = user.IdUsuario,
            Code = user.CodUsuario,
            StartDate = user.FechaInicio,
            EndDate = user.FechaCaduca,
            PasswordChangeDate = user.FechaCambioContra,
            PasswordDurationDays = user.DiasDuraContrasena,
            IsAdmin = string.Equals(user.IdUsuario, "administrador", StringComparison.OrdinalIgnoreCase)
        };
    }

    public async Task<UserDetailDto> ChangePasswordAsync(string userId, ChangePasswordRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.NewPassword))
            throw new InvalidOperationException("La nueva contraseña es obligatoria.");

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.IdUsuario == userId);

        if (user is null)
            throw new InvalidOperationException("El usuario no existe.");

        user.Contrasena = request.NewPassword;
        user.FechaCambioContra = DateTime.Today;

        await _context.SaveChangesAsync();

        return new UserDetailDto
        {
            Id = user.IdUsuario,
            Code = user.CodUsuario,
            StartDate = user.FechaInicio,
            EndDate = user.FechaCaduca,
            PasswordChangeDate = user.FechaCambioContra,
            PasswordDurationDays = user.DiasDuraContrasena,
            IsAdmin = string.Equals(user.IdUsuario, "administrador", StringComparison.OrdinalIgnoreCase)
        };
    }

    private static void ValidateCreateRequest(CreateUserRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Id))
            throw new InvalidOperationException("El id del usuario es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new InvalidOperationException("La contraseña es obligatoria.");
    }

    public async Task AssignBranchesAsync(string userId, AssignBranchesRequestDto request)
    {
        await EnsureUserExists(userId);

        var validBranchCodes = await _context.Branches
            .AsNoTracking()
            .Where(x => x.EmpCodigo == request.CompanyId)
            .Select(x => x.SucCodigo)
            .ToListAsync();

        var requestedCodes = request.Branches.Select(x => x.BranchId).ToList();

        if (requestedCodes.Any(x => !validBranchCodes.Contains(x)))
            throw new InvalidOperationException("Una o más sucursales no existen en la empresa.");

        var current = await _context.UserBranches
            .Where(x => x.IdUsuario == userId && x.IdEmpresa == request.CompanyId)
            .ToListAsync();

        _context.UserBranches.RemoveRange(current);

        var authorized = request.Branches
            .Where(x => x.Authorized)
            .Select(x => new UserBranch
            {
                IdUsuario = userId,
                IdEmpresa = (byte)request.CompanyId,
                CodSucursal = x.BranchId,
                AutorizaSuc = "S"
            })
            .ToList();

        _context.UserBranches.AddRange(authorized);
        await _context.SaveChangesAsync();
    }

    public async Task AssignWarehousesAsync(string userId, AssignWarehousesRequestDto request)
    {
        await EnsureUserExists(userId);

        var branchExists = await _context.Branches
            .AsNoTracking()
            .AnyAsync(x => x.EmpCodigo == request.CompanyId && x.SucCodigo == request.BranchId);

        if (!branchExists)
            throw new InvalidOperationException("La sucursal no existe.");

        var userHasBranch = await _context.UserBranches
            .AsNoTracking()
            .AnyAsync(x => x.IdUsuario == userId && x.IdEmpresa == request.CompanyId && x.CodSucursal == request.BranchId);

        if (!userHasBranch && !string.Equals(userId, "administrador", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Primero debes asignar la sucursal al usuario.");

        var validWarehouseCodes = await _context.Warehouses
            .AsNoTracking()
            .Where(x => x.EmpCodigo == request.CompanyId && x.SucCodigo == request.BranchId)
            .Select(x => x.BodCodigo)
            .ToListAsync();

        var requestedCodes = request.Warehouses.Select(x => x.WarehouseId).ToList();

        if (requestedCodes.Any(x => !validWarehouseCodes.Contains(x)))
            throw new InvalidOperationException("Una o más bodegas no existen en la sucursal.");

        var current = await _context.UserWarehouses
            .Where(x => x.IdUsuario == userId
                     && x.IdEmpresa == request.CompanyId
                     && x.CodSucursal == request.BranchId)
            .ToListAsync();

        _context.UserWarehouses.RemoveRange(current);

        var authorized = request.Warehouses
            .Where(x => x.Authorized)
            .Select(x => new UserWarehouse
            {
                IdUsuario = userId,
                IdEmpresa = (byte)request.CompanyId,
                CodSucursal = request.BranchId,
                CodBodega = x.WarehouseId,
                AutorizaBod = "S"
            })
            .ToList();

        _context.UserWarehouses.AddRange(authorized);
        await _context.SaveChangesAsync();
    }

    public async Task AssignPointsOfSaleAsync(string userId, AssignPointsOfSaleRequestDto request)
    {
        await EnsureUserExists(userId);

        var branchExists = await _context.Branches
            .AsNoTracking()
            .AnyAsync(x => x.EmpCodigo == request.CompanyId && x.SucCodigo == request.BranchId);

        if (!branchExists)
            throw new InvalidOperationException("La sucursal no existe.");

        var userHasBranch = await _context.UserBranches
            .AsNoTracking()
            .AnyAsync(x => x.IdUsuario == userId && x.IdEmpresa == request.CompanyId && x.CodSucursal == request.BranchId);

        if (!userHasBranch && !string.Equals(userId, "administrador", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Primero debes asignar la sucursal al usuario.");

        var validPointCodes = await _context.PointsOfSale
            .AsNoTracking()
            .Where(x => x.EmpCodigo == request.CompanyId && x.SucCodigo == request.BranchId)
            .Select(x => x.PtoCodigo)
            .ToListAsync();

        var requestedCodes = request.PointsOfSale.Select(x => x.PointOfSaleId).ToList();

        if (requestedCodes.Any(x => !validPointCodes.Contains(x)))
            throw new InvalidOperationException("Uno o más puntos de venta no existen en la sucursal.");

        var current = await _context.UserPointsOfSales
            .Where(x => x.IdUsuario == userId
                     && x.IdEmpresa == request.CompanyId
                     && x.CodSucursal == request.BranchId)
            .ToListAsync();

        _context.UserPointsOfSales.RemoveRange(current);

        var authorized = request.PointsOfSale
            .Where(x => x.Authorized)
            .Select(x => new UserPointOfSale
            {
                IdUsuario = userId,
                IdEmpresa = (byte)request.CompanyId,
                CodSucursal = request.BranchId,
                CodPtoVta = x.PointOfSaleId,
                AutorizaPtoVta = "S"
            })
            .ToList();

        _context.UserPointsOfSales.AddRange(authorized);
        await _context.SaveChangesAsync();
    }

    public async Task<UserPermissionContextDto?> GetPermissionContextAsync(string userId, int companyId)
    {
        var exists = await _context.Users
            .AsNoTracking()
            .AnyAsync(x => x.IdUsuario == userId);

        if (!exists)
            return null;

        var branches = await _context.UserBranches
            .AsNoTracking()
            .Where(x => x.IdUsuario == userId && x.IdEmpresa == companyId)
            .OrderBy(x => x.CodSucursal)
            .Select(x => x.CodSucursal)
            .ToListAsync();

        var warehouses = await _context.UserWarehouses
            .AsNoTracking()
            .Where(x => x.IdUsuario == userId && x.IdEmpresa == companyId)
            .OrderBy(x => x.CodSucursal)
            .ThenBy(x => x.CodBodega)
            .Select(x => new UserWarehouseContextDto
            {
                BranchId = x.CodSucursal ?? "",
                WarehouseId = x.CodBodega
            })
            .ToListAsync();

        var points = await _context.UserPointsOfSales
            .AsNoTracking()
            .Where(x => x.IdUsuario == userId && x.IdEmpresa == companyId)
            .OrderBy(x => x.CodSucursal)
            .ThenBy(x => x.CodPtoVta)
            .Select(x => new UserPointOfSaleContextDto
            {
                BranchId = x.CodSucursal,
                PointOfSaleId = x.CodPtoVta
            })
            .ToListAsync();

        return new UserPermissionContextDto
        {
            UserId = userId,
            CompanyId = companyId,
            Branches = branches,
            Warehouses = warehouses,
            PointsOfSale = points
        };
    }

    private async Task EnsureUserExists(string userId)
    {
        var exists = await _context.Users
            .AsNoTracking()
            .AnyAsync(x => x.IdUsuario == userId);

        if (!exists)
            throw new InvalidOperationException("El usuario no existe.");
    }

    public async Task AssignAccessesAsync(string userId, AssignAccessesRequestDto request)
    {
        await EnsureUserExists(userId);

        var current = await _context.UserAccesses
            .Where(x => x.IdUsuario == userId
                     && x.IdEmpresa == request.CompanyId
                     && x.IdSistema == request.SystemId)
            .ToListAsync();

        _context.UserAccesses.RemoveRange(current);

        var items = request.Accesses
        .Where(x => !string.IsNullOrWhiteSpace(x.OptionId))
        .Select(x => new UserAccess
        {
            IdUsuario = userId,
            IdEmpresa = request.CompanyId,
            IdSistema = request.SystemId,
            IdOpcion = x.OptionId,
            IdNomOpcion = x.OptionName,
            Accesos = "T"
        })
        .ToList();

        _context.UserAccesses.AddRange(items);
        await _context.SaveChangesAsync();
    }

    public async Task<List<UserAccessDto>> GetAccessesAsync(string userId, int companyId)
    {
        return await _context.UserAccesses
            .AsNoTracking()
            .Where(x => x.IdUsuario == userId && x.IdEmpresa == companyId)
            .OrderBy(x => x.IdSistema)
            .ThenBy(x => x.IdOpcion)
            .Select(x => new UserAccessDto
            {
                CompanyId = (int)x.IdEmpresa,
                SystemId = x.IdSistema,
                OptionId = x.IdOpcion,
                OptionName = x.IdNomOpcion,
                AccessValue = x.Accesos
            })
            .ToListAsync();
    }

    public async Task AssignDocumentAccessesAsync(string userId, AssignDocumentAccessesRequestDto request)
    {
        await EnsureUserExists(userId);

        if (string.IsNullOrWhiteSpace(request.DocumentCode))
            throw new InvalidOperationException("El código del documento es obligatorio.");

        var current = await _context.UserDocumentAccesses
            .Where(x => x.Empresa == request.CompanyId
                     && x.IdUsuario == userId
                     && x.OpcDocumento == request.DocumentCode)
            .ToListAsync();

        _context.UserDocumentAccesses.RemoveRange(current);

        var items = request.Options
            .Where(x => !string.IsNullOrWhiteSpace(x.Option))
            .Select(x => new UserDocumentAccess
            {
                Empresa = request.CompanyId,
                IdUsuario = userId,
                OpcDocumento = request.DocumentCode,
                Opcion = x.Option,
                Abierto = x.Opened,
                Cantidad = x.Quantity,
                Minimo = x.Minimo,
                Maximo = x.Maximo,
                ValorFijo = x.FixedValue,
                AuxVal1 = x.AuxVal1,
                AuxVal2 = x.AuxVal2,
                AuxStr1 = x.AuxStr1,
                AuxStr2 = x.AuxStr2
            })
            .ToList();

        _context.UserDocumentAccesses.AddRange(items);
        await _context.SaveChangesAsync();
    }

    public async Task<List<UserDocumentDto>> GetDocumentsAsync(string userId, int companyId)
    {
        return await _context.UserDocuments
            .AsNoTracking()
            .Where(x => x.IdUsuario == userId && x.IdEmpresa == companyId)
            .OrderBy(x => x.CodDocumento)
            .Select(x => new UserDocumentDto
            {
                CompanyId = x.IdEmpresa,
                DocumentCode = x.CodDocumento,
                Changes = x.Cambios
            })
            .ToListAsync();
    }


    //public async Task AssignDocumentsAsync(string userId, AssignDocumentsRequestDto request)
    //{
    //    await EnsureUserExists(userId);

    //    await using var transaction = await _context.Database.BeginTransactionAsync();

    //    try
    //    {
    //        var currentDocuments = await _context.UserDocuments
    //            .Where(x => x.IdUsuario == userId && x.IdEmpresa == request.CompanyId)
    //            .ToListAsync();

    //        _context.UserDocuments.RemoveRange(currentDocuments);

    //        var currentDocumentAccesses = await _context.UserDocumentAccesses
    //            .Where(x => x.IdUsuario == userId && x.Empresa == request.CompanyId)
    //            .ToListAsync();

    //        _context.UserDocumentAccesses.RemoveRange(currentDocumentAccesses);

    //        var documents = request.Documents
    //            .Where(x => !string.IsNullOrWhiteSpace(x.DocumentCode))
    //            .Select(x => new UserDocument
    //            {
    //                IdUsuario = userId,
    //                IdEmpresa = (byte)request.CompanyId,
    //                CodDocumento = x.DocumentCode!.Trim().ToUpper(),
    //                Cambios = string.IsNullOrWhiteSpace(x.Changes) ? "T" : x.Changes!.Trim().ToUpper()
    //            })
    //            .ToList();

    //        _context.UserDocuments.AddRange(documents);

    //        if (request.GenerateDefaultAccesses)
    //        {
    //            foreach (var document in documents)
    //            {
    //                var defaultAccesses = BuildDefaultDocumentAccesses(
    //                    request.CompanyId,
    //                    userId,
    //                    document.CodDocumento);

    //                _context.UserDocumentAccesses.AddRange(defaultAccesses);
    //            }
    //        }

    //        await _context.SaveChangesAsync();
    //        await transaction.CommitAsync();
    //    }
    //    catch
    //    {
    //        await transaction.RollbackAsync();
    //        throw;
    //    }
    //}

    public async Task AssignDocumentsAsync(string userId, AssignDocumentsRequestDto request)
    {
        await EnsureUserExists(userId);

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var newDocumentCodes = request.Documents
                .Where(x => !string.IsNullOrWhiteSpace(x.DocumentCode))
                .Select(x => x.DocumentCode!.Trim().ToUpper())
                .Distinct()
                .ToList();

            var currentDocuments = await _context.UserDocuments
                .Where(x => x.IdUsuario == userId && x.IdEmpresa == request.CompanyId)
                .ToListAsync();

            var currentDocumentCodes = currentDocuments
                .Select(x => x.CodDocumento.Trim().ToUpper())
                .ToList();

            var removedDocumentCodes = currentDocumentCodes
                .Except(newDocumentCodes)
                .ToList();

            var addedDocumentCodes = newDocumentCodes
                .Except(currentDocumentCodes)
                .ToList();

            // 1. Borrar documentos actuales y recrear sys_Documentos
            _context.UserDocuments.RemoveRange(currentDocuments);

            var documents = request.Documents
                .Where(x => !string.IsNullOrWhiteSpace(x.DocumentCode))
                .Select(x => new UserDocument
                {
                    IdUsuario = userId,
                    IdEmpresa = (byte)request.CompanyId,
                    CodDocumento = x.DocumentCode!.Trim().ToUpper(),
                    Cambios = string.IsNullOrWhiteSpace(x.Changes)
                        ? "T"
                        : x.Changes!.Trim().ToUpper()
                })
                .ToList();

            _context.UserDocuments.AddRange(documents);

            // 2. Eliminar sysdocaccs SOLO de documentos removidos
            if (removedDocumentCodes.Any())
            {
                var accessesToRemove = await _context.UserDocumentAccesses
                    .Where(x => x.IdUsuario == userId
                             && x.Empresa == request.CompanyId
                             && removedDocumentCodes.Contains(x.OpcDocumento))
                    .ToListAsync();

                _context.UserDocumentAccesses.RemoveRange(accessesToRemove);
            }

            // 3. Generar permisos base SOLO para documentos nuevos
            if (request.GenerateDefaultAccesses)
            {
                foreach (var documentCode in addedDocumentCodes)
                {
                    var defaultAccesses = BuildDefaultDocumentAccesses(
                        request.CompanyId,
                        userId,
                        documentCode);

                    _context.UserDocumentAccesses.AddRange(defaultAccesses);
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static List<UserDocumentAccess> BuildDefaultDocumentAccesses(int companyId, string userId, string documentCode)
    {
        var options = new[]
        {
        "Crear",
        "Consultar",
        "Imprimir",
        "Modificar",
        "Eliminar",
        "Anular",
        "Bodega",
        "CierreCaja",
        "Contabilidad",
        "DescuentoDocumento",
        "DescuentoUnitario",
        "DetalleProducto",
        "EntregaExpress",
        "FechaDocumento",
        "FormaDePago",
        "Gastos",
        "Impuestos",
        "Ingresos",
        "NúmeroDocumento",
        "PrecioUnitario",
        "Vendedor"
    };

        return options.Select(option => new UserDocumentAccess
        {
            Empresa = companyId,
            IdUsuario = userId,
            OpcDocumento = documentCode,
            Opcion = option,
            Abierto = option is "Crear" or "Consultar" or "Imprimir",
            Cantidad = 0,
            Minimo = 0,
            Maximo = option == "Imprimir" ? 1 : 0,
            ValorFijo = "",
            AuxVal1 = 0,
            AuxVal2 = 0,
            AuxStr1 = "",
            AuxStr2 = ""
        }).ToList();
    }


    public async Task<List<UserDocumentAccessDto>> GetDocumentAccessesAsync(string userId, int companyId, string documentCode)
    {
        return await _context.UserDocumentAccesses
            .AsNoTracking()
            .Where(x => x.IdUsuario == userId
                     && x.Empresa == companyId
                     && x.OpcDocumento == documentCode)
            .OrderBy(x => x.Opcion)
            .Select(x => new UserDocumentAccessDto
            {
                CompanyId = x.Empresa,
                UserId = x.IdUsuario,
                DocumentCode = x.OpcDocumento,
                Option = x.Opcion,
                Opened = x.Abierto,
                Quantity = x.Cantidad,
                Minimo = x.Minimo,
                Maximo = x.Maximo,
                FixedValue = x.ValorFijo,
                AuxVal1 = x.AuxVal1,
                AuxVal2 = x.AuxVal2,
                AuxStr1 = x.AuxStr1,
                AuxStr2 = x.AuxStr2
            })
            .ToListAsync();
    }

    public async Task<List<AssignableBranchDto>> GetAssignableBranchesAsync(string userId, int companyId)
    {
        await EnsureUserExists(userId);

        var assigned = await _context.UserBranches
            .AsNoTracking()
            .Where(x => x.IdUsuario == userId
                     && x.IdEmpresa == companyId
                     && x.AutorizaSuc == "S")
            .Select(x => x.CodSucursal)
            .ToListAsync();

        return await _context.Branches
            .AsNoTracking()
            .Where(x => x.EmpCodigo == companyId)
            .OrderBy(x => x.SucCodigo)
            .Select(x => new AssignableBranchDto
            {
                CompanyId = x.EmpCodigo,
                BranchId = x.SucCodigo,
                BranchName = x.SucNombre,
                HasAccess = assigned.Contains(x.SucCodigo)
            })
            .ToListAsync();
    }

    public async Task<List<AssignableWarehouseDto>> GetAssignableWarehousesAsync(string userId,int companyId,string branchId)
    {
        await EnsureUserExists(userId);

        var assigned = await _context.UserWarehouses
            .AsNoTracking()
            .Where(x => x.IdUsuario == userId
                     && x.IdEmpresa == companyId
                     && x.CodSucursal == branchId
                     && x.AutorizaBod == "S")
            .Select(x => x.CodBodega)
            .ToListAsync();

        return await _context.Warehouses
            .AsNoTracking()
            .Where(x => x.EmpCodigo == companyId && x.SucCodigo == branchId)
            .OrderBy(x => x.BodCodigo)
            .Select(x => new AssignableWarehouseDto
            {
                CompanyId = x.EmpCodigo,
                BranchId = x.SucCodigo,
                WarehouseId = x.BodCodigo,
                WarehouseName = x.BodNombre,
                HasAccess = assigned.Contains(x.BodCodigo)
            })
            .ToListAsync();
    }

    public async Task<List<AssignablePointOfSaleDto>> GetAssignablePointsOfSaleAsync(string userId,int companyId,string branchId)
    {
        await EnsureUserExists(userId);

        var assigned = await _context.UserPointsOfSales
            .AsNoTracking()
            .Where(x => x.IdUsuario == userId
                     && x.IdEmpresa == companyId
                     && x.CodSucursal == branchId
                     && x.AutorizaPtoVta == "S")
            .Select(x => x.CodPtoVta)
            .ToListAsync();

        return await _context.PointsOfSale
            .AsNoTracking()
            .Where(x => x.EmpCodigo == companyId && x.SucCodigo == branchId)
            .OrderBy(x => x.PtoCodigo)
            .Select(x => new AssignablePointOfSaleDto
            {
                CompanyId = x.EmpCodigo,
                BranchId = x.SucCodigo,
                PointOfSaleId = x.PtoCodigo,
                PointOfSaleName = x.PtoNombre,
                HasAccess = assigned.Contains(x.PtoCodigo)
            })
            .ToListAsync();
    }

    public async Task<List<AssignableDocumentDto>> GetAssignableDocumentsAsync(string userId,int companyId,string archiveType)
    {
        await EnsureUserExists(userId);

        var databaseName = await _context.CompanyDatabases
            .AsNoTracking()
            .Where(x => x.EmpCodigo == companyId && x.ArchTipo == archiveType)
            .Select(x => x.ArchNom)
            .FirstOrDefaultAsync();

        if (string.IsNullOrWhiteSpace(databaseName))
            throw new InvalidOperationException("No se encontró la base transaccional configurada.");

        databaseName = databaseName.Trim();

        if (!IsSafeSqlIdentifier(databaseName))
            throw new InvalidOperationException("El nombre de la base transaccional no es válido.");

        var assignedDocuments = await _context.UserDocuments
            .AsNoTracking()
            .Where(x => x.IdUsuario == userId
                     && x.IdEmpresa == companyId
                     && x.Cambios == "T")
            .Select(x => x.CodDocumento)
            .ToListAsync();

        var result = new List<AssignableDocumentDto>();

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

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var code = reader["Opc_documento"]?.ToString()?.Trim() ?? "";

            result.Add(new AssignableDocumentDto
            {
                CompanyId = companyId,
                DocumentCode = code,
                DocumentName = reader["Opc_nombre"]?.ToString()?.Trim(),
                DocumentType = reader["Opc_tipo"]?.ToString()?.Trim(),
                HasAccess = assignedDocuments.Contains(code)
            });
        }

        return result;
    }

    private static bool IsSafeSqlIdentifier(string value)
    {
        return value.All(c => char.IsLetterOrDigit(c) || c == '_');
    }

}