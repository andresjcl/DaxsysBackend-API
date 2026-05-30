using System;
using System.Collections.Generic;
using System.Text;

namespace Daxsys.Domain.Entities;

public class Warehouse
{
    public int EmpCodigo { get; set; }
    public string SucCodigo { get; set; } = null!;
    public string BodCodigo { get; set; } = null!;
    public string? BodNombre { get; set; }
    public string? BodIdCta { get; set; }
}
