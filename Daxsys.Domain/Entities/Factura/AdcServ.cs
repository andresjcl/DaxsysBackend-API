using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Daxsys.Domain.Entities.Factura;

public class AdcServ
{
    [Key]
    [Column("Sev_codigo")]
    public string Sev_codigo { get; set; } = null!;

    [Column("Sev_nombre")]
    public string? Sev_nombre { get; set; }

    [Column("Sev_unimed")]
    public string? Sev_unimed { get; set; }

    [Column("Sev_precvta")]
    public decimal? Sev_precvta { get; set; }

    [Column("Sev_sniva")]
    public bool? Sev_sniva { get; set; }

    [Column("Sev_PorIva")]
    public decimal? Sev_PorIva { get; set; }

    [Column("Sev_descuen")]
    public decimal? Sev_descuen { get; set; }

    [Column("Sev_Grupo")]
    public string? Sev_Grupo { get; set; }

    [Column("Sev_Categoria")]
    public string? Sev_Categoria { get; set; }

    [Column("Sev_Clase")]
    public string? Sev_Clase { get; set; }

    [Column("Sev_TipoSerSri")]
    public string? Sev_TipoSerSri { get; set; }
}