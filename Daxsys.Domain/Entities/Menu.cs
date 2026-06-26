using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Daxsys.Domain.Entities;

[Table("sys_Menu")]
[PrimaryKey(nameof(IdMenu))]

public class Menu
{
    public int IdMenu { get; set; }
    public int? IdPadre { get; set; }
    public string IdSistema { get; set; } = null!;
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Ruta { get; set; }
    public string? Icono { get; set; }
    public int Orden { get; set; }
    public int Nivel { get; set; }
    public bool Activo { get; set; }
}