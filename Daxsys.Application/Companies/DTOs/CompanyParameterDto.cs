namespace Daxsys.Application.Companies.DTOs;

public class CompanyParameterDto
{
    // ==================== CLAVE PRIMARIA ====================
    public int EmpCodigo { get; set; }

    // ==================== CONFIGURACIÓN CONTABLE ====================
    public int? DefCtaNumNiveles { get; set; }
    public int? DefCtaNumGrupos { get; set; }
    public string? DefCtaNumDigNivel { get; set; }
    public int? DefCtaNumNiveles1 { get; set; }
    public int? DefCtaNumGrupos1 { get; set; }
    public string? DefCtaNumDigNivel1 { get; set; }
    public decimal? DefCtaV { get; set; }

    // ==================== CIERRE CONTABLE ====================
    public string? ParContiCierre { get; set; }
    public string? ParInvTipoCierre { get; set; }

    // ==================== IVA ====================
    public string? ParVenIVA { get; set; }
    public string? ParComIVA { get; set; }
    public string? ParClvIVA { get; set; }

    // ==================== VENTAS ====================
    public bool ParVensNEm { get; set; }
    public bool ParVensNAcuDoc { get; set; }
    public string? ParDocPrincipalVta { get; set; }
    public string? ParPagoCompras { get; set; }

    // ==================== COMPRAS ====================
    public bool ParComSNEmp { get; set; }
    public bool ParComSNAcuDoc { get; set; }

    // ==================== PRESUPUESTOS ====================
    public int? PrsptoNumNiveles { get; set; }
    public int? PrsptoNumGrupos { get; set; }
    public string? PrsptoNumDigNivel { get; set; }

    // ==================== ACUMULACIÓN HISTÓRICA ====================
    public bool ParAcumHis { get; set; }
    public int? ParAcfNumNiv { get; set; }

    // ==================== ROLES Y CLAVES ====================
    public string? ParRolCodMay { get; set; }
    public int? ParRolTur { get; set; }
    public string? ParClvDsc { get; set; }

    // ==================== SUCURSAL PRINCIPAL ====================
    public string? MainBranchCode { get; set; }

    // ==================== DÍGITOS ====================
    public int? ParNumerodigitos { get; set; }
    public int? CostDigits { get; set; }
    public int? PriceDigits { get; set; }

    // ==================== FECHAS Y LÍMITES ====================
    public int? ParFecDes { get; set; }
    public int? LimAtrasoEntrada { get; set; }
    public int? LimExtraSalida { get; set; }
    public int? LimExtraEntrada { get; set; }
    public int? ParDiasMensualesAcf { get; set; }
    public int? EmpDiasMensualesAcf { get; set; }

    // ==================== CHEQUES ====================
    public byte? ParCheques { get; set; }

    // ==================== CRUCE DE DOCUMENTOS ====================
    public byte? ParCruceDocSucursal { get; set; }

    // ==================== VALIDACIONES SRI ====================
    public byte? ParValiDir { get; set; }
    public byte? ParValiSRI { get; set; }
    public string? UrlSRI { get; set; }

    // ==================== PATHS Y DIRECTORIOS ====================
    public string? ParPathImagenes { get; set; }
    public string? ImagesPath { get; set; }
    public string? PathTmpServer { get; set; }
    public int? LongCodDirectorio { get; set; }

    // ==================== CORREO ====================
    public string? CtaLocalEmail { get; set; }
}