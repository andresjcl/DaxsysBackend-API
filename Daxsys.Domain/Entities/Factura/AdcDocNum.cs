using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Daxsys.Domain.Entities.Factura;

[Table("AdcDocNum")]
public class AdcDocNum
{
    public string Id_Lugar { get; set; } = null!;
    public string id_Documento { get; set; } = null!;
    public decimal? UltimoNumero { get; set; }
    public DateTime? UltimaFecha { get; set; }
    public string? id_bodega { get; set; }
    public string? id_Banco { get; set; }
    public string? id_Directorio { get; set; }
    public string? id_Sri { get; set; }
    public decimal idclave { get; set; }
}