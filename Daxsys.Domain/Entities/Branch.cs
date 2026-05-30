using System;
using System.Collections.Generic;
using System.Text;

namespace Daxsys.Domain.Entities;

public class Branch
{
    public int EmpCodigo { get; set; }
    public string SucCodigo { get; set; } = null!;
    public string? SucNombre { get; set; }
    public string? SucDireccion { get; set; }
    public string? SucRuc { get; set; }
    public string? SucSegSocial { get; set; }
    public string? SucIdCta { get; set; }
    public string? BodCodigo { get; set; }
    public string? SucIdTributario { get; set; }
    public decimal? PrecioVta { get; set; }
}
