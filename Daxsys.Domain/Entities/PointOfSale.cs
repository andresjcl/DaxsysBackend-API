namespace Daxsys.Domain.Entities;

public class PointOfSale
{
    public int EmpCodigo { get; set; }
    public string SucCodigo { get; set; } = null!;
    public string PtoCodigo { get; set; } = null!;
    public string? PtoNombre { get; set; }
    public string? PtoIdTributario { get; set; }
    public string? PtoIdCta { get; set; }
    public string? TipoPunto { get; set; }
}