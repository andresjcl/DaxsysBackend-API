using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Daxsys.Domain.Entities;

[Table("Emp_Datos")]
public class Company
{
    [Column("Emp_codigo")]
    public int EmpCodigo { get; set; }

    [Column("Emp_Nombre")]
    public string? EmpNombre { get; set; }

    [Column("Emp_Pais")]
    public string? EmpPais { get; set; }

    [Column("Emp_Provincia")]
    public string? EmpProvincia { get; set; }

    [Column("Emp_Ciudad")]
    public string? EmpCiudad { get; set; }

    [Column("Emp_Cantón")]
    public string? EmpCanton { get; set; }

    [Column("Emp_Dirección")]
    public string? EmpDireccion { get; set; }

    [Column("Emp_Telefono_1")]
    public string? EmpTelefono1 { get; set; }

    [Column("Emp_Telefono_2")]
    public string? EmpTelefono2 { get; set; }

    [Column("Emp_Fax")]
    public string? EmpFax { get; set; }

    [Column("Emp_Casilla")]
    public string? EmpCasilla { get; set; }

    [Column("Emp_Email")]
    public string? EmpEmail { get; set; }

    [Column("Emp_RUC")]
    public string? EmpRuc { get; set; }

    [Column("Emp_SegSocial")]
    public string? EmpSegSocial { get; set; }

    [Column("Emp_Presidente")]
    public string? EmpPresidente { get; set; }

    [Column("Emp_Gerente")]
    public string? EmpGerente { get; set; }

    [Column("Emp_RepLegal")]
    public string? EmpRepLegal { get; set; }

    [Column("Emp_Contador")]
    public string? EmpContador { get; set; }

    [Column("Emp_Logotipo")]
    public string? EmpLogotipo { get; set; }

    [Column("Emp_Defecto")]
    public bool EmpDefecto { get; set; }

    [Column("Emp_Lic1")]
    public string? EmpLic1 { get; set; }

    [Column("Emp_Lic2")]
    public string? EmpLic2 { get; set; }

    [Column("Emp_Lic3")]
    public string? EmpLic3 { get; set; }

    [Column("Emp_TipoBase")]
    public string? EmpTipoBase { get; set; }

    [Column("Emp_Lic4")]
    public string? EmpLic4 { get; set; }

    [Column("Emp_Lic5")]
    public string? EmpLic5 { get; set; }

    [Column("Emp_Lic6")]
    public string? EmpLic6 { get; set; }

    [Column("Emp_PathImagenes")]
    public string? EmpPathImagenes { get; set; }

    [Column("Emp_Conta")]
    public bool? EmpConta { get; set; }

    [Column("Emp_AgeRet")]
    public string? EmpAgeRet { get; set; }

    [Column("Emp_ContrBuyEsp")]
    public string? EmpContrBuyEsp { get; set; }

    [Column("Emp_Regimen")]
    public string? EmpRegimen { get; set; }
}