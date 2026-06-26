using System;
using System.Collections.Generic;
using System.Text;

namespace Daxsys.Application.Features.Facturacion.DTOs
{
    public class FacturaPagoDto
    {
        public decimal Valor { get; set; }
        public string? TipoPago { get; set; }
        public string? Descripcion { get; set; }
        public string? IdPago { get; set; }
    }
}
