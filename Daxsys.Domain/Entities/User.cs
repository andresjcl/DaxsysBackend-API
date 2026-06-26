using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Daxsys.Domain.Entities
{
    [Table("sys_Usuario")]
    [PrimaryKey(nameof(IdUsuario))]
    public class User
    {
        public string IdUsuario { get; set; } = null!;
        public string? CodUsuario { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaCaduca { get; set; }

        [Column("Contraseña")]
        public string? Contrasena { get; set; }

        [Column("FechaCambioContra")]
        public DateTime? FechaCambioContra { get; set; }

        [Column("DíasDuraContraseña")]
        public int? DiasDuraContrasena { get; set; }
    }
}