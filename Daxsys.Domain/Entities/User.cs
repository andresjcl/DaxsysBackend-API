using System;
using System.Collections.Generic;
using System.Text;

namespace Daxsys.Domain.Entities
{
    public class User
    {
        public string IdUsuario { get; set; } = null!;
        public string? CodUsuario { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaCaduca { get; set; }
        public string? Contrasena { get; set; }
        public DateTime? FechaCambioContra { get; set; }
        public int? DiasDuraContrasena { get; set; }
    }
}
