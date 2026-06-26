using Daxsys.Application.Features.Facturacion.DTOs;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using System.Data.SqlClient;

namespace Daxsys.Application.Features.Facturacion.Services;

public class FacturaService
{
    private readonly IConfiguration _configuration;
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores = new();

    public FacturaService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<FacturaResponseDto> CrearFactura(string connectionString, FacturaRequestDto request, string empresaId)
    {
        var response = new FacturaResponseDto { Success = false };
        var semaphore = _semaphores.GetOrAdd(request.Sucursal, new SemaphoreSlim(1, 1));

        await semaphore.WaitAsync();

        SqlConnection connection = null;
        SqlTransaction transaction = null;

        try
        {
            connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            transaction = connection.BeginTransaction();

            // 1. Validar o crear el cliente en Identificacion
            string codigoCliente = "";
            if (!string.IsNullOrEmpty(request.CiRuc))
            {
                codigoCliente = await ObtenerOInsertarCliente(connection, transaction,
                    request.CiRuc,
                    request.NombreCliente ?? "",
                    request.Direccion,
                    request.Telefono1,
                    request.CorreoCliente);

                request.CodigoCliente = codigoCliente;
            }
            else
            {
                throw new Exception("El campo ciRuc es obligatorio");
            }

            // 2. Calcular totales y validar productos
            var (totalConIva, totalBaseIva, totalBaseSinIva, totalValorIva, porcentajeIva, productosInfo)
                = await CalcularTotalesConIva(connection, transaction, request.Lineas, request.Fecha);

            // 3. Generar IdLugar: Sucursal + NroIdDoc
            string idLugar = $"{request.Sucursal}{request.NroIdDoc}";

            // 4. Obtener número de documento
            var docNumero = await ObtenerSiguienteNumero(connection, transaction, idLugar);

            // 5. Insertar cabecera
            var idClaveDoc = await InsertarCabecera(connection, transaction, request,
                docNumero, totalConIva, totalBaseIva, totalValorIva, porcentajeIva, empresaId);

            // 6. Insertar líneas
            await InsertarLineas(connection, transaction, request, docNumero, idClaveDoc, porcentajeIva, totalConIva, productosInfo);

            // 7. Insertar pagos
            if (request.Pagos != null && request.Pagos.Any())
            {
                await InsertarPagos(connection, transaction, request, docNumero, idClaveDoc);
            }

            await transaction.CommitAsync();

            response.Success = true;
            response.Sucursal = request.Sucursal;
            response.DocNumero = docNumero;
            response.IdClaveDoc = idClaveDoc;
            response.Total = totalConIva;
            response.Mensaje = "Factura creada exitosamente";
        }
        catch (Exception ex)
        {
            if (transaction != null) await transaction.RollbackAsync();
            response.Mensaje = $"Error: {ex.Message}";
        }
        finally
        {
            transaction?.Dispose();
            connection?.Dispose();
            semaphore.Release();
        }
        return response;
    }

    private async Task<decimal> ObtenerSiguienteNumero(SqlConnection connection, SqlTransaction transaction, string idLugar)
    {
        string sqlUpdate = @"
            UPDATE AdcDocNum 
            SET UltimoNumero = UltimoNumero + 1,
                UltimaFecha = GETDATE()
            WHERE Id_Lugar = @idLugar AND id_Documento = 'FAC'
            
            SELECT UltimoNumero FROM AdcDocNum 
            WHERE Id_Lugar = @idLugar AND id_Documento = 'FAC'";

        using var cmd = new SqlCommand(sqlUpdate, connection, transaction);
        cmd.Parameters.AddWithValue("@idLugar", idLugar);

        var result = await cmd.ExecuteScalarAsync();

        if (result == null || result == DBNull.Value)
        {
            string sqlInsert = @"
                INSERT INTO AdcDocNum (Id_Lugar, id_Documento, UltimoNumero, UltimaFecha)
                VALUES (@idLugar, 'FAC', 1, GETDATE())
                SELECT 1";

            using var cmdInsert = new SqlCommand(sqlInsert, connection, transaction);
            cmdInsert.Parameters.AddWithValue("@idLugar", idLugar);
            result = await cmdInsert.ExecuteScalarAsync();
        }

        return Convert.ToDecimal(result);
    }

    private async Task<decimal> ObtenerSiguienteIdClaveDoc(SqlConnection connection, SqlTransaction transaction)
    {
        var sql = "SELECT ISNULL(MAX(IdClaveDoc), 0) + 1 FROM AdcDoc";
        using var cmd = new SqlCommand(sql, connection, transaction);
        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToDecimal(result);
    }

    private async Task<decimal> ObtenerPorcentajeIva(SqlConnection connection, SqlTransaction transaction, DateTime fecha)
    {
        try
        {
            string sql = @"
                SELECT Porcentaje 
                FROM Ivaretdax.dbo.PorcentajeIva 
                WHERE @fecha BETWEEN FechaInicio AND FechaFin";

            using var cmd = new SqlCommand(sql, connection, transaction);
            cmd.Parameters.AddWithValue("@fecha", fecha);

            var result = await cmd.ExecuteScalarAsync();

            if (result == null || result == DBNull.Value)
            {
                return 15m;
            }

            decimal porcentaje = Convert.ToDecimal(result);
            return porcentaje * 100;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener porcentaje de IVA: {ex.Message}");
        }
    }

    private async Task<(bool existe, string nombre, bool tieneIva, decimal precio, bool sncomp, string artClase)> ValidarProducto(SqlConnection connection, SqlTransaction transaction, string codigoProducto)
    {
        try
        {
            string sql = @"
            SELECT Art_nombre, Art_sniva, Art_precvta1, Art_sncomp, Art_clase
            FROM ADCART 
            WHERE Art_codigo = @codigo";

            using var cmd = new SqlCommand(sql, connection, transaction);
            cmd.Parameters.AddWithValue("@codigo", codigoProducto);

            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                string nombre = reader["Art_nombre"]?.ToString() ?? "";
                bool tieneIva = false;
                bool sncomp = false;
                string artClase = "";

                if (reader["Art_sniva"] != DBNull.Value)
                {
                    int sniva = Convert.ToInt32(reader["Art_sniva"]);
                    tieneIva = (sniva == 1);
                }

                if (reader["Art_sncomp"] != DBNull.Value)
                {
                    int sncompValue = Convert.ToInt32(reader["Art_sncomp"]);
                    sncomp = (sncompValue == 1);
                }

                if (reader["Art_clase"] != DBNull.Value)
                {
                    artClase = reader["Art_clase"]?.ToString() ?? "";
                }

                decimal precio = 0;
                if (reader["Art_precvta1"] != DBNull.Value)
                {
                    precio = Convert.ToDecimal(reader["Art_precvta1"]);
                }

                return (true, nombre, tieneIva, precio, sncomp, artClase);
            }

            return (false, "", false, 0, false, "");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al validar producto {codigoProducto}: {ex.Message}");
        }
    }

    private async Task<(decimal totalConIva, decimal totalBaseIva, decimal totalBaseSinIva, decimal totalValorIva, decimal porcentajeIva, List<(string nombre, bool sncomp, string artClase)> productosInfo)> CalcularTotalesConIva(SqlConnection connection, SqlTransaction transaction, List<FacturaLineaDto> lineas, DateTime fecha)
    {
        decimal totalBaseIva = 0;
        decimal totalBaseSinIva = 0;
        decimal totalValorIva = 0;

        var productosInfo = new List<(string nombre, bool sncomp, string artClase)>();

        decimal porcentajeIvaGeneral = await ObtenerPorcentajeIva(connection, transaction, fecha);

        foreach (var linea in lineas)
        {
            var producto = await ValidarProducto(connection, transaction, linea.Codigo);

            if (!producto.existe)
            {
                throw new Exception($"Producto {linea.Codigo} no encontrado en el catálogo");
            }

            productosInfo.Add((producto.nombre, producto.sncomp, producto.artClase));
            linea.Nombre = producto.nombre;

            if (linea.PrecioUnitario == 0 && producto.precio > 0)
            {
                linea.PrecioUnitario = producto.precio;
            }

            decimal subtotal = linea.Cantidad * linea.PrecioUnitario;

            if (producto.tieneIva)
            {
                totalBaseIva += subtotal;
                totalValorIva += subtotal * (porcentajeIvaGeneral / 100);
            }
            else
            {
                totalBaseSinIva += subtotal;
            }
        }

        decimal totalConIva = totalBaseIva + totalBaseSinIva + totalValorIva;

        return (totalConIva, totalBaseIva, totalBaseSinIva, totalValorIva, porcentajeIvaGeneral, productosInfo);
    }

    private async Task<decimal> InsertarCabecera(SqlConnection connection, SqlTransaction transaction, FacturaRequestDto request, decimal docNumero, decimal totalConIva, decimal totalBaseIva, decimal totalValorIva, decimal porcentajeIva, string empresaId)
    {
        var idClaveDoc = await ObtenerSiguienteIdClaveDoc(connection, transaction);

        var sql = @"
        INSERT INTO AdcDoc (
            Doc_sucursal, Doc_Bodega, Opc_documento, Doc_numero, IdClaveDoc,
            Doc_fecha, Doc_Hora, Doc_codper, Doc_codusu, Doc_porceniva, 
            Doc_valoriva, Doc_totciva, Doc_totsiva, Doc_valor, Doc_detalle,
            Doc_NombreImp, Doc_CiRuc, Doc_Direccion, Doc_Telefono1, Doc_Telefono2,
            Doc_NroIdDoc, PuntoVta, AuxVar1, Doc_Estado, Doc_FecGraba,
            Doc_TipoDoc, Doc_Contado, Doc_Contabilidad, Doc_Inventario, Doc_Ventas,
            Doc_docnombre, BaseImp1, PorcImp1, ValorImp1, AuxNum1,
            Doc_Oculto, Doc_Banco, Doc_Compras, Doc_FechaModifica, Adi_TipoDocSri,
            AuxVar9
        ) VALUES (
            @sucursal, @bodega, 'FAC', @docNumero, @idClaveDoc,
            @fecha, GETDATE(), @codCliente, 'API', @porcenIva, 
            @valorIva, @totCiva, 0, @valorTotal, @detalle,
            @nombreCliente, @ciRuc, @direccion, @telefono1, @telefono2,
            @nroIdDoc, @puntoVta, @empresaId, 1, GETDATE(),
            'FAC', @valorTotal, 1, -1, 1,
            'Factura cliente', @totCiva, @porcenIva, 0, 1,
            0, 0, 0, @fecha, '18',
            @correoCliente
        )";

        using var cmd = new SqlCommand(sql, connection, transaction);
        cmd.Parameters.AddWithValue("@sucursal", request.Sucursal);
        cmd.Parameters.AddWithValue("@bodega", string.IsNullOrEmpty(request.Bodega) ? (object)DBNull.Value : request.Bodega);
        cmd.Parameters.AddWithValue("@docNumero", docNumero);
        cmd.Parameters.AddWithValue("@idClaveDoc", idClaveDoc);
        cmd.Parameters.AddWithValue("@fecha", request.Fecha);
        cmd.Parameters.AddWithValue("@codCliente", request.CodigoCliente);
        cmd.Parameters.AddWithValue("@porcenIva", porcentajeIva);
        cmd.Parameters.AddWithValue("@valorIva", totalValorIva);
        cmd.Parameters.AddWithValue("@totCiva", totalBaseIva);
        cmd.Parameters.AddWithValue("@valorTotal", totalConIva);
        cmd.Parameters.AddWithValue("@detalle", request.Detalle ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@nombreCliente", request.NombreCliente ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@ciRuc", request.CiRuc ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@direccion", request.Direccion ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@telefono1", request.Telefono1 ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@telefono2", request.Telefono2 ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@nroIdDoc", request.NroIdDoc ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@puntoVta", "API");
        cmd.Parameters.AddWithValue("@empresaId", empresaId);
        cmd.Parameters.AddWithValue("@correoCliente", request.CorreoCliente ?? (object)DBNull.Value);

        await cmd.ExecuteNonQueryAsync();
        return idClaveDoc;
    }

    private async Task InsertarLineas(SqlConnection connection, SqlTransaction transaction, FacturaRequestDto request, decimal docNumero, decimal idClaveDoc, decimal porcentajeIvaGeneral, decimal totalFactura, List<(string nombre, bool sncomp, string artClase)> productosInfo)
    {
        var sql = @"
        INSERT INTO AdcTra (
            Doc_sucursal, Doc_Bodega, Opc_documento, Doc_numero, IdClaveDoc,
            Tra_numlinea, Tra_Codigo, Tra_nombre, Tra_cantidad, Tra_valor,
            Tra_precuni, Tra_prectot, Tra_fecha, Tra_TipoDoc, Tra_Estado,
            Tra_Inventario, Tra_Ventas, Tra_sniva, Tra_Individual, Tra_quetipo,
            Tra_medida, Tra_multiplo, Tra_piezas, Tra_porceniva, Tra_valoriva,
            Tra_numprecio, Tra_costuni, Tra_costtot, Tra_Oculto, Tra_Compras,
            Tra_Activo, tra_producto, Tra_Despachado,
            tra_anio, tra_mes, tra_dia
        ) VALUES (
            @sucursal, @bodega, 'FAC', @docNumero, @idClaveDoc,
            @numLinea, @codigo, @nombre, @cantidad, @totalFactura,
            @precUni, @precTot, @fechaLinea, 'FAC', 1,
            -1, 1, @sniva, 'N', 'A',
            'und', 1, 0, @porcenIva, @valorIva,
            2, 0, 0, 0, 0,
            0, @traProducto, @despachado,
            @anio, @mes, @dia
        )";

        int anio = DateTime.Now.Year;
        int mes = DateTime.Now.Month;
        int dia = DateTime.Now.Day;

        for (int i = 0; i < request.Lineas.Count; i++)
        {
            var linea = request.Lineas[i];
            var productoInfo = productosInfo[i];

            var subtotal = linea.Cantidad * linea.PrecioUnitario;
            var valorIva = subtotal * (porcentajeIvaGeneral / 100);
            var precioTotal = subtotal + valorIva;
            var sniva = 1;
            var traProducto = productoInfo.sncomp ? 1 : 0;
            var despachado = string.IsNullOrEmpty(productoInfo.artClase) ? "" : productoInfo.artClase;

            using var cmd = new SqlCommand(sql, connection, transaction);
            cmd.Parameters.AddWithValue("@sucursal", request.Sucursal);
            cmd.Parameters.AddWithValue("@bodega", string.IsNullOrEmpty(request.Bodega) ? (object)DBNull.Value : request.Bodega);
            cmd.Parameters.AddWithValue("@docNumero", docNumero);
            cmd.Parameters.AddWithValue("@idClaveDoc", idClaveDoc);
            cmd.Parameters.AddWithValue("@numLinea", linea.NumLinea);
            cmd.Parameters.AddWithValue("@codigo", linea.Codigo);
            cmd.Parameters.AddWithValue("@nombre", productoInfo.nombre);
            cmd.Parameters.AddWithValue("@cantidad", linea.Cantidad);
            cmd.Parameters.AddWithValue("@totalFactura", totalFactura);
            cmd.Parameters.AddWithValue("@precUni", linea.PrecioUnitario);
            cmd.Parameters.AddWithValue("@precTot", precioTotal);
            cmd.Parameters.AddWithValue("@fechaLinea", linea.FechaLinea ?? DateTime.Now);
            cmd.Parameters.AddWithValue("@sniva", sniva);
            cmd.Parameters.AddWithValue("@porcenIva", porcentajeIvaGeneral);
            cmd.Parameters.AddWithValue("@valorIva", valorIva);
            cmd.Parameters.AddWithValue("@traProducto", traProducto);
            cmd.Parameters.AddWithValue("@despachado", despachado);
            cmd.Parameters.AddWithValue("@anio", anio);
            cmd.Parameters.AddWithValue("@mes", mes);
            cmd.Parameters.AddWithValue("@dia", dia);

            await cmd.ExecuteNonQueryAsync();
        }
    }

    private async Task InsertarPagos(SqlConnection connection, SqlTransaction transaction, FacturaRequestDto request, decimal docNumero, decimal idClaveDoc)
    {
        var sql = @"
        INSERT INTO AdcPag (
            Doc_sucursal, Opc_documento, Doc_numero, IdClaveDoc, Pag_Numero,
            Pag_Valor, Pag_TipoPago, Pag_Descripcion, Pag_Idpago, Pag_Formapago,
            Pag_Autoriza, Doc_Fecha, Pag_Cuotas
        ) VALUES (
            @sucursal, 'FAC', @docNumero, @idClaveDoc, @numPago,
            @valor, @tipoPago, @descripcion, @idPago, 2,
            1, GETDATE(), 0
        )";

        for (int i = 0; i < request.Pagos.Count; i++)
        {
            var pago = request.Pagos[i];
            string tipoPago = "4";
            string descripcion = "";
            string idPago = pago.IdPago ?? "GLO";

            switch (idPago)
            {
                case "GLO":
                    descripcion = "GLOVO";
                    break;
                case "RAP":
                    descripcion = "RAPPI";
                    break;
                case "UBE":
                    descripcion = "UBER";
                    break;
                default:
                    descripcion = pago.Descripcion ?? "PAGO CON TARJETA";
                    break;
            }

            using var cmd = new SqlCommand(sql, connection, transaction);
            cmd.Parameters.AddWithValue("@sucursal", request.Sucursal);
            cmd.Parameters.AddWithValue("@docNumero", docNumero);
            cmd.Parameters.AddWithValue("@idClaveDoc", idClaveDoc);
            cmd.Parameters.AddWithValue("@numPago", i + 1);
            cmd.Parameters.AddWithValue("@valor", pago.Valor);
            cmd.Parameters.AddWithValue("@tipoPago", tipoPago);
            cmd.Parameters.AddWithValue("@descripcion", descripcion);
            cmd.Parameters.AddWithValue("@idPago", idPago);

            await cmd.ExecuteNonQueryAsync();
        }
    }

    private string DeterminarTipoIdentificacion(string ciRuc)
    {
        if (string.IsNullOrEmpty(ciRuc))
            return "C";

        string limpio = new string(ciRuc.Where(char.IsDigit).ToArray());

        if (limpio.Length == 10)
            return "C";
        else if (limpio.Length == 13)
            return "R";
        else
            return "P";
    }

    private string ObtenerCodigoCliente(string ciRuc)
    {
        if (string.IsNullOrEmpty(ciRuc))
            return ciRuc;

        if (ciRuc.Any(char.IsLetter))
        {
            return ciRuc.Length > 15 ? ciRuc.Substring(0, 15) : ciRuc;
        }

        string soloDigitos = new string(ciRuc.Where(char.IsDigit).ToArray());

        if (soloDigitos.Length >= 10)
            return soloDigitos.Substring(0, 10);
        else
            return soloDigitos;
    }

    private async Task<string> ObtenerOInsertarCliente(SqlConnection connection, SqlTransaction transaction, string ciRuc, string nombres, string domicilio, string telefono1, string correo)
    {
        try
        {
            string cedulaIdentidadRuc = ciRuc;
            string codigoCliente = ObtenerCodigoCliente(ciRuc);
            string tipoIdentificacion = DeterminarTipoIdentificacion(ciRuc);

            string sqlValidar = @"
            SELECT Codigo FROM Identificacion 
            WHERE CedulaIdentidadRuc = @cedulaRuc OR Codigo = @codigo";

            using var cmdValidar = new SqlCommand(sqlValidar, connection, transaction);
            cmdValidar.Parameters.AddWithValue("@cedulaRuc", cedulaIdentidadRuc);
            cmdValidar.Parameters.AddWithValue("@codigo", codigoCliente);

            var existe = await cmdValidar.ExecuteScalarAsync();

            if (existe != null && existe != DBNull.Value)
            {
                return existe.ToString();
            }

            string sqlInsert = @"
            INSERT INTO Identificacion (
                TipoPersona, EsCliente, EsProveedor, EsEmpleado, EsBanco, EsAsociado, EsVendedor,
                Codigo, TipoIdentificacion, CedulaIdentidadRuc, Nombres, Apellidos, NombreImpresion,
                Pais, Provincia, Ciudad, Domicilio, Telefono1, CorreoElectrónico,
                CodGrabo, ComisionVenta, ExoneradoIva, EsDirecta, Grupo1, Grupo2, Grupo3,
                esRise, ObligLlevarConta, RegimenMicroempresas
            ) VALUES (
                'N', 1, 0, 0, 0, 0, 0,
                @codigo, @tipoIdentificacion, @cedulaRuc, @nombres, '', @nombres,
                '593', '', '', @domicilio, @telefono1, @correo,
                'ADMINISTRADOR', 0.00, 0, 'NO', 'CLIENTE', '', '',
                0, 0, 0
            )";

            using var cmdInsert = new SqlCommand(sqlInsert, connection, transaction);
            cmdInsert.Parameters.AddWithValue("@codigo", codigoCliente);
            cmdInsert.Parameters.AddWithValue("@tipoIdentificacion", tipoIdentificacion);
            cmdInsert.Parameters.AddWithValue("@cedulaRuc", cedulaIdentidadRuc);
            cmdInsert.Parameters.AddWithValue("@nombres", nombres);
            cmdInsert.Parameters.AddWithValue("@domicilio", domicilio ?? (object)DBNull.Value);
            cmdInsert.Parameters.AddWithValue("@telefono1", telefono1 ?? (object)DBNull.Value);
            cmdInsert.Parameters.AddWithValue("@correo", correo ?? (object)DBNull.Value);

            await cmdInsert.ExecuteNonQueryAsync();

            return codigoCliente;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al validar/crear cliente: {ex.Message}");
        }
    }
}