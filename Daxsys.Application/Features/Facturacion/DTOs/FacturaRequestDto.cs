using System;
using System.Collections.Generic;
using System.Text;

namespace Daxsys.Application.Features.Facturacion.DTOs
{
    public class FacturaRequestDto
    {
        // Datos de la sucursal
        public string Sucursal { get; set; } = string.Empty;
        public string? Bodega { get; set; }
        public string? PuntoVta { get; set; }
        public string? NroIdDoc { get; set; }

        // Datos de la factura
        public DateTime Fecha { get; set; }
        public string? Detalle { get; set; }

        // Datos del cliente factura
        public string? NombreCliente { get; set; }
        public string? CodigoCliente { get; set; }
        public string? CiRuc { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono1 { get; set; }
        public string? Telefono2 { get; set; }
        public string? CorreoCliente { get; set; }

        // Totales
        public decimal PorcenIva { get; set; }
        public decimal ValorIva { get; set; }
        public decimal TotCiva { get; set; }
        public decimal TotSiva { get; set; }
        public decimal ValorTotal { get; set; }

        // Líneas de detalle
        public List<FacturaLineaDto> Lineas { get; set; } = new();

        // Pagos
        public List<FacturaPagoDto> Pagos { get; set; } = new();

        public ClienteIdentificacionDto ClienteIdentificacion { get; set; } = new();
    }

}
