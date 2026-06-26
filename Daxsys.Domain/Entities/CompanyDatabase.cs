using System.ComponentModel.DataAnnotations.Schema;

namespace Daxsys.Domain.Entities;

[Table("Emp_Arch")]  // O el nombre real de la tabla
public class CompanyDatabase
{
    [Column("Emp_Codigo")]
    public int EmpCodigo { get; set; }

    [Column("Arch_Tipo")]
    public string ArchTipo { get; set; } = null!;

    [Column("Arch_Nom")]
    public string? ArchNom { get; set; }
}