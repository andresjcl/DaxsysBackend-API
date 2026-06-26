using System;
using System.Collections.Generic;
using System.Text;

namespace Daxsys.Application.Features.Facturacion.DTOs
{
    public class ClienteIdentificacionDto
    {
        public string Codigo { get; set; } = string.Empty;           // 10 dígitos (cedula, ruc, pasaporte)
        public string TipoIdentificacion { get; set; } = string.Empty; // C, R, P
        public string CedulaIdentidadRuc { get; set; } = string.Empty; // Número completo
        public string Nombres { get; set; } = string.Empty;
        public string? Apellidos { get; set; }
        public string NombreImpresion { get; set; } = string.Empty;
        public string? Domicilio { get; set; }
        public string? NumeroDomicilio { get; set; }
        public string? Sector { get; set; }
        public string? Telefono1 { get; set; }
        public string? Telefono2 { get; set; }
        public string? Telefono3 { get; set; }
        public string? CorreoElectronico { get; set; }
        public string? Pais { get; set; }
        public string? Provincia { get; set; }
        public string? Ciudad { get; set; }
    }
}
