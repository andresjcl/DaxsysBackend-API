namespace Daxsys.Domain.Entities;

public class UserDocumentAccess
{
    public int Empresa { get; set; }
    public string IdUsuario { get; set; } = null!;
    public string OpcDocumento { get; set; } = null!;
    public string Opcion { get; set; } = null!;
    public bool? Abierto { get; set; }
    public int? Cantidad { get; set; }
    public decimal? Minimo { get; set; }
    public decimal? Maximo { get; set; }
    public string? ValorFijo { get; set; }
    public decimal? AuxVal1 { get; set; }
    public decimal? AuxVal2 { get; set; }
    public string? AuxStr1 { get; set; }
    public string? AuxStr2 { get; set; }
}