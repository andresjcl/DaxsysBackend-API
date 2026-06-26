using System;
using System.Collections.Generic;
using System.Text;

namespace Daxsys.Application.Features.Facturacion.DTOs
{
    public class FacturaLineaDto
    {
        public int NumLinea { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public decimal PrecioTotal { get; set; }
        public decimal Iva { get; set; }
        public DateTime? FechaLinea { get; set; }
    }
}
