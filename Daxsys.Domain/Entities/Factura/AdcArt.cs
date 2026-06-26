using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Daxsys.Domain.Entities.Factura;

[Table("AdcArt")]

public class AdcArt
{
[Key]
[Column("Art_codigo")]
public string Art_codigo { get; set; } = null!;

[Column("Art_nombre")]
public string? Art_nombre { get; set; }

[Column("Art_categor")]
public string? Art_categor { get; set; }

[Column("Art_clase")]
public string? Art_clase { get; set; }

[Column("Art_grupo")]
public string? Art_grupo { get; set; }

[Column("Art_subgrup")]
public string? Art_subgrup { get; set; }

[Column("Art_unimed")]
public string? Art_unimed { get; set; }

[Column("Art_sniva")]
public bool? Art_sniva { get; set; }

[Column("Art_PorIva")]
public decimal? Art_PorIva { get; set; }

[Column("Art_precvta1")]
public decimal? Art_precvta1 { get; set; }

[Column("Art_precvta2")]
public decimal? Art_precvta2 { get; set; }

[Column("Art_precvta3")]
public decimal? Art_precvta3 { get; set; }

[Column("Art_precvta4")]
public decimal? Art_precvta4 { get; set; }

[Column("Art_precvta5")]
public decimal? Art_precvta5 { get; set; }

[Column("Art_costucom")]
public decimal? Art_costucom { get; set; }

[Column("Art_ExistBod")]
public bool? Art_ExistBod { get; set; }

[Column("Art_descuen")]
public decimal? Art_descuen { get; set; }

[Column("Art_minbod")]
public decimal? Art_minbod { get; set; }

[Column("Art_maxbod")]
public decimal? Art_maxbod { get; set; }

[Column("Art_CompraMinima")]
public int? Art_CompraMinima { get; set; }

[NotMapped]
public decimal Stock { get; set; } // Se calcula desde AdcStk
}