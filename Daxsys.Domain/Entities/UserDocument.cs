using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Daxsys.Domain.Entities;

[Table("sys_Documentos")]
[PrimaryKey(nameof(IdUsuario), nameof(IdEmpresa), nameof(CodDocumento))]

public class UserDocument
{
    public string IdUsuario { get; set; } = null!;
    public byte IdEmpresa { get; set; }
    public string CodDocumento { get; set; } = null!;
    public string? Cambios { get; set; }
}