using System.ComponentModel.DataAnnotations.Schema;

namespace Daxsys.Domain.Entities;

[Table("Emp_Par")]
public class CompanyParameter
{
    // ==================== CLAVE PRIMARIA ====================
    [Column("Emp_Codigo")]
    public int EmpCodigo { get; set; }

    // ==================== CONFIGURACIÓN CONTABLE ====================
    [Column("DefCta_NumNiveles")]
    public int? DefCtaNumNiveles { get; set; }

    [Column("DefCta_NumGrupos")]
    public int? DefCtaNumGrupos { get; set; }

    [Column("DefCta_NumDigNivel")]
    public string? DefCtaNumDigNivel { get; set; }

    [Column("DefCta_NumNiveles1")]
    public int? DefCtaNumNiveles1 { get; set; }

    [Column("DefCta_NumGrupos1")]
    public int? DefCtaNumGrupos1 { get; set; }

    [Column("DefCta_NumDigNivel1")]
    public string? DefCtaNumDigNivel1 { get; set; }

    [Column("DefCtaV")]
    public decimal? DefCtaV { get; set; }

    // ==================== CIERRE CONTABLE ====================
    [Column("Par_ConTipCierre")]
    public string ParContiCierre { get; set; } = null!;

    [Column("Par_InvTipCierre")]
    public string? ParInvTipoCierre { get; set; }

    // ==================== IVA ====================
    [Column("Par_VenIVA")]
    public string? ParVenIVA { get; set; }

    [Column("Par_ComIVA")]
    public string? ParComIVA { get; set; }

    [Column("Par_ClvIVA")]
    public string? ParClvIVA { get; set; }

    // ==================== VENTAS ====================
    [Column("Par_VenSNEmp")]
    public bool ParVensNEm { get; set; }

    [Column("Par_VenSNAcuDoc")]
    public bool ParVensNAcuDoc { get; set; }

    [Column("Par_DocPrincipalVta")]
    public string? ParDocPrincipalVta { get; set; }

    [Column("Par_PagoCompras")]
    public string? ParPagoCompras { get; set; }

    // ==================== COMPRAS ====================
    [Column("Par_ComSNEmp")]
    public bool ParComSNEmp { get; set; }

    [Column("Par_ComSNAcuDoc")]
    public bool ParComSNAcuDoc { get; set; }

    // ==================== PRESUPUESTOS ====================
    [Column("Prspto_NumNiveles")]
    public int? PrsptoNumNiveles { get; set; }

    [Column("Prspto_NumGrupos")]
    public int? PrsptoNumGrupos { get; set; }

    [Column("Prspto_NumDigNivel")]
    public string? PrsptoNumDigNivel { get; set; }

    // ==================== ACUMULACIÓN HISTÓRICA ====================
    [Column("Par_AcumHis")]
    public bool ParAcumHis { get; set; }

    [Column("Par_AcfNumNiv")]
    public int? ParAcfNumNiv { get; set; }

    // ==================== ROLES Y CLAVES ====================
    [Column("Par_RolCodMay")]
    public string? ParRolCodMay { get; set; }

    [Column("Par_RolTur")]
    public int? ParRolTur { get; set; }

    [Column("Par_ClvDsc")]
    public string? ParClvDsc { get; set; }

    // ==================== SUCURSAL PRINCIPAL ====================
    [Column("Par_SucPri")]
    public string? ParSucPri { get; set; }

    // ==================== DÍGITOS ====================
    [Column("Par_Numerodigitos")]
    public int? ParNumerodigitos { get; set; }

    [Column("Par_DigitosCostos")]
    public int? ParDigitosCostos { get; set; }

    [Column("Par_DigitosPrecios")]
    public int? ParDigitosPrecios { get; set; }

    // ==================== FECHAS Y LÍMITES ====================
    [Column("Par_FecDes")]
    public int? ParFecDes { get; set; }

    [Column("Par_LimAtrasoEntrada")]
    public int? ParLimAtrasoEntrada { get; set; }

    [Column("Par_LimExtraSalida")]
    public int? ParLimExtraSalida { get; set; }

    [Column("Par_LimExtraEntrada")]
    public int? ParLimExtraEntrada { get; set; }

    [Column("par_DiasMensualesAcf")]
    public int? ParDiasMensualesAcf { get; set; }

    [Column("Emp_DiasMensualesAcf")]
    public int? EmpDiasMensualesAcf { get; set; }

    // ==================== CHEQUES ====================
    [Column("Par_Cheques")]
    public byte? ParCheques { get; set; }

    // ==================== CRUCE DE DOCUMENTOS ====================
    [Column("Par_CruceDocSucursal")]
    public byte? ParCruceDocSucursal { get; set; }

    // ==================== VALIDACIONES SRI ====================
    [Column("par_ValiDir")]
    public byte? ParValiDir { get; set; }

    [Column("par_ValiSRI")]
    public byte? ParValiSRI { get; set; }

    [Column("par_UrlSRI")]
    public string? ParUrlSRI { get; set; }

    // ==================== PATHS Y DIRECTORIOS ====================
    [Column("par_PathImagenes")]
    public string? ParPathImagenes { get; set; }

    [Column("Emp_PathImagenes")]
    public string? EmpPathImagenes { get; set; }

    [Column("path_tmpServer")]
    public string? PathTmpServer { get; set; }

    [Column("LongCodDirectorio")]
    public int? LongCodDirectorio { get; set; }

    // ==================== CORREO ====================
    [Column("CtaLocalEmail")]
    public string? CtaLocalEmail { get; set; }
}