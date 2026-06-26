// Daxsys.Domain/Entities/Factura/PorcentajeIva.cs
namespace Daxsys.Domain.Entities.Factura;

public class PorcentajeIva
{
    public decimal Porcentaje { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int Clave { get; set; }
}