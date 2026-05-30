using Daxsys.Application.System.DTOs;
using Daxsys.Application.System.Interfaces;
using Daxsys.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Daxsys.Infrastructure.Services;

public class SystemQueryService : ISystemQueryService
{
    private readonly AppDbContext _context;

    public SystemQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DatabaseInfoDto>> GetDatabasesAsync()
    {
        var result = new List<DatabaseInfoDto>();

        var connection = _context.Database.GetDbConnection();

        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        await using var command = connection.CreateCommand();

        command.CommandText = @"
            SELECT name AS DatabaseName
            FROM sys.databases
            WHERE database_id > 4
              AND state_desc = 'ONLINE'
            ORDER BY name;
        ";

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new DatabaseInfoDto
            {
                DatabaseName = reader["DatabaseName"].ToString()!
            });
        }

        return result;
    }

    public async Task InitializeSystemAsync(InitializeSystemRequestDto request)
    {
        if (request.ConfirmationText != "BORRAR TODO E INICIALIZAR")
            throw new InvalidOperationException("Confirmación inválida.");

        if (string.IsNullOrWhiteSpace(request.TransactionalDatabase))
            throw new InvalidOperationException("Debe seleccionar la base transaccional.");

        var transactionalExists = await DatabaseExistsAsync(request.TransactionalDatabase);

        if (!transactionalExists)
            throw new InvalidOperationException($"La base '{request.TransactionalDatabase}' no existe o no está ONLINE.");

        if (!string.IsNullOrWhiteSpace(request.SriDatabase))
        {
            var sriExists = await DatabaseExistsAsync(request.SriDatabase);

            if (!sriExists)
                throw new InvalidOperationException($"La base SRI '{request.SriDatabase}' no existe o no está ONLINE.");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Limpiar permisos y seguridad
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM dbo.sysdocaccs");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM dbo.sys_Documentos");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM dbo.sys_Accesos");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM dbo.sys_ptoVta");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM dbo.sys_Bodegas");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM dbo.sys_Sucursales");

            // 2. Limpiar maestros de empresa
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM dbo.Emp_PtoVta");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM dbo.Emp_Bod");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM dbo.Emp_Suc");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM dbo.Emp_Arch");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM dbo.Emp_Par");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM dbo.Emp_Datos");

            // 3. Limpiar usuarios
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM dbo.sys_Usuario");

            // 4. Insertar empresa base
            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            INSERT INTO dbo.Emp_Datos
            (
                Emp_codigo,
                Emp_Nombre,
                Emp_RUC,
                Emp_Defecto
            )
            VALUES
            (
                {request.CompanyId},
                {request.CompanyName},
                {request.Ruc},
                1
            )
        ");

            // 5. Insertar parámetros mínimos
            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            INSERT INTO dbo.Emp_Par
            (
                Emp_Codigo,
                Par_ConTipCierre,
                Par_InvTipCierre,
                Par_VenSNEmp,
                Par_VenSNAcuDoc,
                Par_ComSNEmp,
                Par_ComSNAcuDoc,
                Par_AcumHis,
                Par_SucPri
            )
            VALUES
            (
                {request.CompanyId},
                'A',
                'X',
                0,
                0,
                0,
                0,
                0,
                {request.BranchCode}
            )
        ");

            // 6. Insertar sucursal principal
            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            INSERT INTO dbo.Emp_Suc
            (
                Emp_Codigo,
                Suc_Codigo,
                Suc_Nombre,
                Bod_Codigo,
                Suc_RUC
            )
            VALUES
            (
                {request.CompanyId},
                {request.BranchCode},
                {request.BranchName},
                {request.WarehouseCode},
                {request.Ruc}
            )
        ");

            // 7. Insertar bodega principal
            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            INSERT INTO dbo.Emp_Bod
            (
                Emp_Codigo,
                Suc_Codigo,
                Bod_codigo,
                Bod_nombre
            )
            VALUES
            (
                {request.CompanyId},
                {request.BranchCode},
                {request.WarehouseCode},
                {request.WarehouseName}
            )
        ");

            // 8. Insertar Emp_Arch
            var transactionalDb = request.TransactionalDatabase.Trim();
            var sriDb = string.IsNullOrWhiteSpace(request.SriDatabase)
                ? request.TransactionalDatabase.Trim()
                : request.SriDatabase.Trim();

            foreach (var type in new[] { "ADC", "ALE", "PRO", "ROL" })
            {
                await _context.Database.ExecuteSqlInterpolatedAsync($@"
                INSERT INTO dbo.Emp_Arch
                (
                    Emp_Codigo,
                    Arch_Tipo,
                    Arch_Nom
                )
                VALUES
                (
                    {request.CompanyId},
                    {type},
                    {transactionalDb}
                )
            ");
            }

            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            INSERT INTO dbo.Emp_Arch
            (
                Emp_Codigo,
                Arch_Tipo,
                Arch_Nom
            )
            VALUES
            (
                {request.CompanyId},
                'SRI',
                {sriDb}
            )
        ");

            // 9. Insertar usuario administrador
            var fechaInicio = DateTime.Now;
            var fechaCaduca = new DateTime(9999, 12, 31);
            var fechaCambioContra = DateTime.Now;

            await _context.Database.ExecuteSqlInterpolatedAsync($@"
                INSERT INTO dbo.sys_Usuario
                (
                    IdUsuario,
                    CodUsuario,
                    FechaInicio,
                    FechaCaduca,
                    Contraseña,
                    FechaCambioContra,
                    DíasDuraContraseña
                )
                VALUES
                (
                    {request.AdminUserId},
                    {request.AdminUserId},
                    {fechaInicio},
                    {fechaCaduca},
                    {request.AdminPassword},
                    {fechaCambioContra},
                    9999
                )
            ");

            await transaction.CommitAsync();
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


}