using System;
using System.Collections.Generic;
using System.Text;

namespace Daxsys.Application.Features.Facturacion.DTOs
{
    public class FacturaResponseDto
    {
        public bool Success { get; set; }
        public string? Sucursal { get; set; }
        public decimal DocNumero { get; set; }
        public decimal IdClaveDoc { get; set; }  // ← Agregar esta línea
        public decimal Total { get; set; }
        public string? Mensaje { get; set; }
    }
}
