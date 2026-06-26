using Daxsys.Application.Companies.DTOs;
using Daxsys.Application.Companies.Interfaces;
using Daxsys.Domain.Entities;
using Daxsys.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

namespace Daxsys.Infrastructure.Services;

public class CompanyRestoreService : ICompanyRestoreService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CompanyRestoreService> _logger;

    public CompanyRestoreService(
        AppDbContext context,
        IConfiguration configuration,
        ILogger<CompanyRestoreService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<RestoreCompanyResponseDto> RestoreCompanyFromBackupAsync(RestoreCompanyRequestDto request)
    {

        await EnsureColumnsAreExtendedAsync();
        // ==================== VALIDACIONES OBLIGATORIAS ====================

        // 1. Validar que la empresa origen existe
        var sourceCompany = await _context.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.EmpCodigo == request.SourceCompanyId && x.EmpDefecto == true);

        if (sourceCompany == null)
            throw new InvalidOperationException($"No se encontró la empresa base (Emp_Defecto = 1) con código {request.SourceCompanyId}.");

        // 2. Validar datos obligatorios
        if (string.IsNullOrWhiteSpace(request.NewCompanyName))
            throw new InvalidOperationException("El nombre de la nueva empresa es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.NewRuc))
            throw new InvalidOperationException("El RUC de la nueva empresa es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.NewInitials))
            throw new InvalidOperationException("Las iniciales de la nueva empresa son obligatorias.");

        if (string.IsNullOrWhiteSpace(request.BranchCode))
            throw new InvalidOperationException("El código de la sucursal es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.BranchName))
            throw new InvalidOperationException("El nombre de la sucursal es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.WarehouseCode))
            throw new InvalidOperationException("El código de la bodega es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.WarehouseName))
            throw new InvalidOperationException("El nombre de la bodega es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.BackupPath))
            throw new InvalidOperationException("La ruta de backup es obligatoria.");

        var backupPath = request.BackupPath.TrimEnd('\\') + "\\";

        // 3. Auto-generar el nuevo ID de empresa
        var newCompanyId = await GetNextCompanyIdAsync();
        _logger.LogInformation("Nuevo ID de empresa generado: {NewCompanyId}", newCompanyId);

        // 4. Validar RUC único
        var rucExists = await _context.Companies
            .AsNoTracking()
            .AnyAsync(x => x.EmpRuc == request.NewRuc);

        if (rucExists)
            throw new InvalidOperationException($"Ya existe una empresa con el RUC {request.NewRuc}.");

        // 5. Obtener bases de datos de la empresa origen
        var sourceDatabases = await _context.CompanyDatabases
            .AsNoTracking()
            .Where(x => x.EmpCodigo == request.SourceCompanyId)
            .ToListAsync();

        if (!sourceDatabases.Any())
            throw new InvalidOperationException("La empresa base no tiene bases de datos configuradas.");

        // 6. Construir los nuevos nombres de bases de datos
        var newDatabases = new List<(string DatabaseType, string SourceName, string NewName)>();
        var restoredDatabases = new List<RestoredDatabaseDto>();

        foreach (var db in sourceDatabases)
        {
            var newName = GenerateNewDatabaseName(db.ArchNom, request.NewInitials);
            newDatabases.Add((db.ArchTipo, db.ArchNom ?? "", newName));
            _logger.LogInformation("Base de datos: {SourceName} -> {NewName}", db.ArchNom, newName);
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 7. Restaurar cada base de datos
            foreach (var (dbType, sourceName, newName) in newDatabases)
            {
                var restored = await RestoreDatabaseAsync(sourceName, newName, backupPath);
                restoredDatabases.Add(new RestoredDatabaseDto
                {
                    DatabaseType = dbType,
                    SourceDatabaseName = sourceName,
                    NewDatabaseName = newName,
                    Restored = restored
                });

                // ✅ LIMPIAR TABLAS TRANSACCIONALES DESPUÉS DE CADA RESTAURACIÓN
                // La base de datos SRI (facturación electrónica) NO se limpia
                if (dbType != "SRI")
                {
                    await CleanTransactionalTablesAsync(newName, keepMasterData: true);
                    _logger.LogInformation("Tablas limpiadas en la base de datos: {NewName}", newName);
                }
            }

            // ==================== 8. CREAR EMPRESA (PRIMERO) ====================
            var newCompany = new Company
            {
                EmpCodigo = newCompanyId,
                EmpNombre = TruncateString(request.NewCompanyName, 80),
                EmpPais = TruncateString(request.Pais ?? sourceCompany.EmpPais, 25),
                EmpProvincia = TruncateString(request.Provincia ?? sourceCompany.EmpProvincia, 25),
                EmpCiudad = TruncateString(request.Ciudad ?? sourceCompany.EmpCiudad, 25),
                EmpCanton = TruncateString(request.Canton ?? sourceCompany.EmpCanton, 25),
                EmpDireccion = TruncateString(request.Direccion ?? sourceCompany.EmpDireccion, 80),
                EmpTelefono1 = TruncateString(request.Telefono1 ?? sourceCompany.EmpTelefono1, 15),
                EmpTelefono2 = TruncateString(request.Telefono2 ?? sourceCompany.EmpTelefono2, 15),
                EmpEmail = TruncateString(request.Email ?? sourceCompany.EmpEmail, 50),
                EmpRuc = TruncateString(request.NewRuc, 20),
                EmpDefecto = false,
                EmpTipoBase = TruncateString(request.TipoBase ?? sourceCompany.EmpTipoBase, 3)
            };

            _context.Companies.Add(newCompany);

            // ✅ GUARDAR LA EMPRESA PRIMERO
            await _context.SaveChangesAsync();
            _logger.LogInformation("Empresa guardada: {CompanyId}", newCompanyId);






            // ==================== 9. CREAR SUCURSAL (SEGUNDO) ====================
            var branch = new Branch
            {
                EmpCodigo = newCompanyId,
                SucCodigo = TruncateString(request.BranchCode, 3),
                SucNombre = TruncateString(request.BranchName, 80),
                SucDireccion = TruncateString(newCompany.EmpDireccion, 200),
                SucRuc = TruncateString(request.NewRuc, 20),
                SucIdTributario = "001",
                BodCodigo = TruncateString(request.WarehouseCode, 3)
            };

            _context.Branches.Add(branch);

            // ✅ GUARDAR LA SUCURSAL
            await _context.SaveChangesAsync();
            _logger.LogInformation("Sucursal guardada: {BranchCode}", branch.SucCodigo);

            // ==================== 10. CREAR BODEGA (TERCERO) ====================
            var warehouse = new Warehouse
            {
                EmpCodigo = newCompanyId,
                SucCodigo = TruncateString(request.BranchCode, 3),
                BodCodigo = TruncateString(request.WarehouseCode, 3),
                BodNombre = TruncateString(request.WarehouseName, 40)
            };

            _context.Warehouses.Add(warehouse);

            // ==================== 11. CREAR PUNTO DE VENTA ====================
            var pointOfSale = new PointOfSale
            {
                EmpCodigo = newCompanyId,
                SucCodigo = TruncateString(request.BranchCode, 3),
                PtoCodigo = "001",
                PtoNombre = "PUNTO DE VENTA 1",
                PtoIdTributario = "001",
                TipoPunto = "FISICO"
            };

            _context.PointsOfSale.Add(pointOfSale);

            // ==================== 12. CREAR PARÁMETROS ====================
            var sourceParams = await _context.CompanyParameters
    .AsNoTracking()
    .FirstOrDefaultAsync(x => x.EmpCodigo == request.SourceCompanyId);

            if (sourceParams == null)
                throw new InvalidOperationException($"No se encontraron parámetros para la empresa origen {request.SourceCompanyId}.");

            var newParams = new CompanyParameter
            {
                EmpCodigo = newCompanyId,

                // ==================== TODOS LOS CAMPOS SE COPIAN DE LA EMPRESA ORIGEN ====================
                DefCtaNumNiveles = sourceParams.DefCtaNumNiveles,
                DefCtaNumGrupos = sourceParams.DefCtaNumGrupos,
                DefCtaNumDigNivel = sourceParams.DefCtaNumDigNivel,
                DefCtaNumNiveles1 = sourceParams.DefCtaNumNiveles1,
                DefCtaNumGrupos1 = sourceParams.DefCtaNumGrupos1,
                DefCtaNumDigNivel1 = sourceParams.DefCtaNumDigNivel1,
                DefCtaV = sourceParams.DefCtaV,

                ParContiCierre = sourceParams.ParContiCierre,
                ParInvTipoCierre = sourceParams.ParInvTipoCierre,

                ParVenIVA = sourceParams.ParVenIVA,
                ParComIVA = sourceParams.ParComIVA,
                ParClvIVA = sourceParams.ParClvIVA,

                ParVensNEm = sourceParams.ParVensNEm,
                ParVensNAcuDoc = sourceParams.ParVensNAcuDoc,
                ParDocPrincipalVta = sourceParams.ParDocPrincipalVta,
                ParPagoCompras = sourceParams.ParPagoCompras,

                ParComSNEmp = sourceParams.ParComSNEmp,
                ParComSNAcuDoc = sourceParams.ParComSNAcuDoc,

                PrsptoNumNiveles = sourceParams.PrsptoNumNiveles,
                PrsptoNumGrupos = sourceParams.PrsptoNumGrupos,
                PrsptoNumDigNivel = sourceParams.PrsptoNumDigNivel,

                ParAcumHis = sourceParams.ParAcumHis,
                ParAcfNumNiv = sourceParams.ParAcfNumNiv,

                ParRolCodMay = sourceParams.ParRolCodMay,
                ParRolTur = sourceParams.ParRolTur,
                ParClvDsc = sourceParams.ParClvDsc,

                // ✅ SOLO ESTE CAMPO SE ACTUALIZA CON LA NUEVA SUCURSAL
                ParSucPri = TruncateString(request.BranchCode, 3),

                ParNumerodigitos = sourceParams.ParNumerodigitos,
                ParDigitosCostos = sourceParams.ParDigitosCostos,
                ParDigitosPrecios = sourceParams.ParDigitosPrecios,

                ParFecDes = sourceParams.ParFecDes,
                ParLimAtrasoEntrada = sourceParams.ParLimAtrasoEntrada,
                ParLimExtraSalida = sourceParams.ParLimExtraSalida,
                ParLimExtraEntrada = sourceParams.ParLimExtraEntrada,
                ParDiasMensualesAcf = sourceParams.ParDiasMensualesAcf,
                EmpDiasMensualesAcf = sourceParams.EmpDiasMensualesAcf,

                ParCheques = sourceParams.ParCheques,
                ParCruceDocSucursal = sourceParams.ParCruceDocSucursal,

                ParValiDir = sourceParams.ParValiDir,
                ParValiSRI = sourceParams.ParValiSRI,
                ParUrlSRI = sourceParams.ParUrlSRI,

                ParPathImagenes = sourceParams.ParPathImagenes,
                EmpPathImagenes = sourceParams.EmpPathImagenes,
                PathTmpServer = sourceParams.PathTmpServer,
                LongCodDirectorio = sourceParams.LongCodDirectorio,

                CtaLocalEmail = sourceParams.CtaLocalEmail
            };

            _context.CompanyParameters.Add(newParams);

            // ==================== 13. CREAR BASES DE DATOS ====================
            foreach (var (dbType, _, newName) in newDatabases)
            {
                var db = new CompanyDatabase
                {
                    EmpCodigo = newCompanyId,
                    ArchTipo = TruncateString(dbType, 3),
                    ArchNom = TruncateString(newName, 70)
                };

                _context.CompanyDatabases.Add(db);
            }

            // ✅ GUARDAR TODOS LOS CAMBIOS RESTANTES
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Empresa restaurada exitosamente: {CompanyId} - {CompanyName} - RUC: {Ruc}",
                newCompany.EmpCodigo, newCompany.EmpNombre, newCompany.EmpRuc);

            return new RestoreCompanyResponseDto
            {
                CompanyId = newCompany.EmpCodigo,
                CompanyName = newCompany.EmpNombre ?? "",
                Ruc = newCompany.EmpRuc ?? "",
                Initials = request.NewInitials,
                Databases = restoredDatabases,
                Success = true,
                Message = "Empresa restaurada exitosamente"
            };
        }
        catch (DbUpdateException dbEx)
        {
            await transaction.RollbackAsync();
            _logger.LogError(dbEx, "Error al guardar cambios en la base de datos");

            var innerException = dbEx.InnerException?.Message ?? dbEx.Message;
            throw new InvalidOperationException($"Error al guardar la empresa: {innerException}");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error al restaurar empresa");
            throw;
        }
    }

    private async Task<int> GetNextCompanyIdAsync()
    {
        var maxId = await _context.Companies
            .AsNoTracking()
            .MaxAsync(x => (int?)x.EmpCodigo) ?? 0;
        return maxId + 1;
    }

    private string GenerateNewDatabaseName(string? sourceName, string initials)
    {
        if (string.IsNullOrWhiteSpace(sourceName))
            return $"BD_{initials}";

        if (sourceName.Length >= 3)
        {
            var baseName = sourceName.Substring(0, sourceName.Length - 3);
            return $"{baseName}{initials.ToUpper()}";
        }

        return $"{sourceName}_{initials.ToUpper()}";
    }

    private async Task<bool> RestoreDatabaseAsync(string sourceDatabase, string newDatabaseName, string backupPath)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        var masterConnection = connectionString.Replace(sourceDatabase, "master");

        var backupFile = $"{backupPath}{sourceDatabase}_{DateTime.Now:yyyyMMdd_HHmmss}.bak";

        await using var connection = new SqlConnection(masterConnection);
        await connection.OpenAsync();

        // 1. Obtener el directorio de la base de datos origen
        var directoryCmd = new SqlCommand(
            $@"
            SELECT TOP 1 
                LEFT(physical_name, LEN(physical_name) - CHARINDEX('\', REVERSE(physical_name)) + 1) AS Directory
            FROM sys.master_files
            WHERE database_id = DB_ID('{sourceDatabase}')
            ",
            connection);
        var directory = await directoryCmd.ExecuteScalarAsync() as string ?? backupPath;

        // 2. Obtener los nombres lógicos de los archivos
        var fileInfoCmd = new SqlCommand(
            $@"
            SELECT 
                name AS LogicalName,
                type_desc AS Type
            FROM sys.master_files
            WHERE database_id = DB_ID('{sourceDatabase}')
            ",
            connection);

        var logicalNames = new List<(string LogicalName, string Type)>();
        using (var reader = await fileInfoCmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                logicalNames.Add((
                    reader["LogicalName"].ToString() ?? "",
                    reader["Type"].ToString() ?? ""
                ));
            }
        }

        // 3. Crear backup
        var backupCmd = new SqlCommand(
            $"BACKUP DATABASE [{sourceDatabase}] TO DISK = '{backupFile}' WITH FORMAT, COPY_ONLY, STATS = 10",
            connection);
        await backupCmd.ExecuteNonQueryAsync();

        // 4. Eliminar base de datos destino si existe
        var dropCmd = new SqlCommand(
            $@"
            IF EXISTS (SELECT * FROM sys.databases WHERE name = '{newDatabaseName}')
            BEGIN
                ALTER DATABASE [{newDatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE [{newDatabaseName}];
            END
            ",
            connection);
        await dropCmd.ExecuteNonQueryAsync();

        // 5. Construir los MOVE para cada archivo
        var moveClauses = new List<string>();
        foreach (var (logicalName, type) in logicalNames)
        {
            string extension = type == "ROWS" ? ".mdf" : (type == "LOG" ? ".ldf" : ".ndf");
            var newPath = Path.Combine(directory, $"{newDatabaseName}{extension}");
            moveClauses.Add($"MOVE '{logicalName}' TO '{newPath}'");
        }

        var moveClause = string.Join(", ", moveClauses);

        // 6. Restaurar la base de datos
        var restoreCmd = new SqlCommand(
            $@"
            RESTORE DATABASE [{newDatabaseName}] 
            FROM DISK = '{backupFile}'
            WITH {moveClause}, REPLACE, STATS = 10;
            ",
            connection);
        await restoreCmd.ExecuteNonQueryAsync();

        _logger.LogInformation("Base de datos restaurada: {NewDatabase} desde {SourceDatabase} en {Directory}",
            newDatabaseName, sourceDatabase, directory);

        return true;
    }

    private async Task EnsureColumnsAreExtendedAsync()
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // 1. Verificar la longitud actual de las columnas
            var checkCmd = new SqlCommand(@"
            SELECT 
                COLUMNPROPERTY(OBJECT_ID('Emp_Datos'), 'Emp_Nombre', 'PRECISION') AS EmpNombreLen,
                COLUMNPROPERTY(OBJECT_ID('Emp_Suc'), 'Suc_Nombre', 'PRECISION') AS SucNombreLen,
                COLUMNPROPERTY(OBJECT_ID('Emp_Suc'), 'Suc_Direccion', 'PRECISION') AS SucDireccionLen
        ", connection);

            using var reader = await checkCmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var empNombreLen = reader["EmpNombreLen"] as int? ?? 0;
                var sucNombreLen = reader["SucNombreLen"] as int? ?? 0;
                var sucDireccionLen = reader["SucDireccionLen"] as int? ?? 0;

                _logger.LogInformation("Longitudes actuales - Emp_Nombre: {EmpLen}, Suc_Nombre: {SucLen}, Suc_Direccion: {SucDirLen}",
                    empNombreLen, sucNombreLen, sucDireccionLen);
            }
            reader.Close();

            // 2. Ejecutar los ALTER TABLE (sin comentarios //)
            var script = @"
            ALTER TABLE Emp_Datos ALTER COLUMN Emp_Nombre NVARCHAR(80) NULL;
            ALTER TABLE Emp_Suc ALTER COLUMN Suc_Nombre NVARCHAR(80) NULL;
            ALTER TABLE Emp_Suc ALTER COLUMN Suc_Direccion NVARCHAR(200) NULL;
            ALTER TABLE Emp_Datos ALTER COLUMN Emp_Direccion NVARCHAR(200) NULL;
        ";

            using var command = new SqlCommand(script, connection);
            await command.ExecuteNonQueryAsync();

            _logger.LogInformation("Columnas ampliadas correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "No se pudieron ampliar las columnas");
            // No lanzamos excepción para no interrumpir el proceso
        }
    }

    private string TruncateString(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        return value.Length > maxLength ? value.Substring(0, maxLength) : value;
    }

    private async Task CleanTransactionalTablesAsync(string databaseName, bool keepMasterData = true)
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            var newConnectionString = connectionString.Replace("BdAdcomDx14DGC", databaseName);

            await using var connection = new SqlConnection(newConnectionString);
            await connection.OpenAsync();

            // ============================================================
            // SCRIPT DE LIMPIEZA SEGÚN OPCIÓN
            // ============================================================
            string script;

            if (keepMasterData)
            {
                // ✅ OPCIÓN 1: Conservar datos maestros (clientes, productos, configuración)
                script = @"
                -- ============================================================
                -- LIMPIEZA DE TABLAS TRANSACCIONALES (CONSERVANDO MAESTROS)
                -- ============================================================

                -- 1. DOCUMENTOS Y TRANSACCIONES
                DELETE FROM AdcCtaMov;
                DELETE FROM ADCARTMOV;
                DELETE FROM AdcMovPer;
                DELETE FROM ADCPAG;
                DELETE FROM ADCTRA;
                DELETE FROM ADCDOC;

                -- 2. NUMERACIÓN Y IDENTIFICADORES
                DELETE FROM ADCDOCNUM;
                DELETE FROM IDDOC;

                -- 3. REINICIAR IDENTIFICADORES
                IF EXISTS (SELECT * FROM sys.identity_columns WHERE OBJECT_NAME(OBJECT_ID) = 'ADCDOC' AND last_value IS NOT NULL)
                    DBCC CHECKIDENT ('ADCDOC', RESEED, 0);
                
                IF EXISTS (SELECT * FROM sys.identity_columns WHERE OBJECT_NAME(OBJECT_ID) = 'ADCTRA' AND last_value IS NOT NULL)
                    DBCC CHECKIDENT ('ADCTRA', RESEED, 0);
                
                IF EXISTS (SELECT * FROM sys.identity_columns WHERE OBJECT_NAME(OBJECT_ID) = 'ADCPAG' AND last_value IS NOT NULL)
                    DBCC CHECKIDENT ('ADCPAG', RESEED, 0);

                -- 4. INSERTAR NUMERACIÓN INICIAL
                -- (Se puede personalizar según necesidades)
            ";
            }
            else
            {
                // ✅ OPCIÓN 2: Limpiar todo (incluyendo maestros)
                script = @"
                -- ============================================================
                -- LIMPIEZA COMPLETA DE TABLAS (INCLUYENDO MAESTROS)
                -- ============================================================

                -- 1. DOCUMENTOS Y TRANSACCIONES
                DELETE FROM AdcCtaMov;
                DELETE FROM ADCARTMOV;
                DELETE FROM AdcMovPer;
                DELETE FROM ADCPAG;
                DELETE FROM ADCTRA;
                DELETE FROM ADCDOC;

                -- 2. NUMERACIÓN Y IDENTIFICADORES
                DELETE FROM ADCDOCNUM;
                DELETE FROM IDDOC;

                -- 3. DATOS MAESTROS
                DELETE FROM ADCART;
                DELETE FROM IDENTIFICACION;
                DELETE FROM AdcFelec;
                DELETE FROM AdcNiv;
                DELETE FROM AdcNivAcf;
                DELETE FROM DEFCON;

                -- 4. REINICIAR IDENTIFICADORES
                IF EXISTS (SELECT * FROM sys.identity_columns WHERE OBJECT_NAME(OBJECT_ID) = 'ADCDOC' AND last_value IS NOT NULL)
                    DBCC CHECKIDENT ('ADCDOC', RESEED, 0);
                
                IF EXISTS (SELECT * FROM sys.identity_columns WHERE OBJECT_NAME(OBJECT_ID) = 'ADCTRA' AND last_value IS NOT NULL)
                    DBCC CHECKIDENT ('ADCTRA', RESEED, 0);
                
                IF EXISTS (SELECT * FROM sys.identity_columns WHERE OBJECT_NAME(OBJECT_ID) = 'ADCPAG' AND last_value IS NOT NULL)
                    DBCC CHECKIDENT ('ADCPAG', RESEED, 0);
            ";
            }

            using var command = new SqlCommand(script, connection);
            await command.ExecuteNonQueryAsync();

            _logger.LogInformation("Tablas limpiadas en la base de datos: {DatabaseName} (KeepMasterData: {KeepMasterData})",
                databaseName, keepMasterData);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error al limpiar tablas en {DatabaseName}", databaseName);
            // No lanzamos excepción para no interrumpir el proceso
        }
    }
}