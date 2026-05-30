namespace Daxsys.Domain.Entities;

public class CompanyParameter
{
    public int EmpCodigo { get; set; }
    public int? DefCtaNumNiveles { get; set; }
    public int? DefCtaNumGrupos { get; set; }
    public string? DefCtaNumDigNivel { get; set; }
    public string ParConTipCierre { get; set; } = null!;
    public string? ParInvTipCierre { get; set; }
    public string? ParVenIVA { get; set; }
    public bool ParVenSNEmp { get; set; }
    public bool ParVenSNAcuDoc { get; set; }
    public string? ParComIVA { get; set; }
    public bool ParComSNEmp { get; set; }
    public bool ParComSNAcuDoc { get; set; }
    public int? ParAcfNumNiv { get; set; }
    public string? ParRolCodMay { get; set; }
    public int? ParRolTur { get; set; }
    public string? ParSucPri { get; set; }
    public string? ParClvDsc { get; set; }
    public string? ParClvIVA { get; set; }
    public bool ParAcumHis { get; set; }
    public int? ParFecDes { get; set; }
    public int? ParNumeroDigitos { get; set; }
    public int? ParLimAtrasoEntrada { get; set; }
    public int? ParLimExtraSalida { get; set; }
    public int? ParLimExtraEntrada { get; set; }
    public string? ParDocPrincipalVta { get; set; }
    public byte? ParCheques { get; set; }
    public string? ParPagoCompras { get; set; }
    public int? DefCtaNumNiveles1 { get; set; }
    public int? DefCtaNumGrupos1 { get; set; }
    public string? DefCtaNumDigNivel1 { get; set; }
    public decimal? DefCtaV { get; set; }
    public int? ParDigitosCostos { get; set; }
    public int? ParDigitosPrecios { get; set; }
    public byte? ParCruceDocSucursal { get; set; }
    public string? EmpPathImagenes { get; set; }
    public int? EmpDiasMensualesAcf { get; set; }
    public string? PathTmpServer { get; set; }
    public string? CtaLocalEmail { get; set; }
    public int? PrsptoNumNiveles { get; set; }
    public int? PrsptoNumGrupos { get; set; }
    public string? PrsptoNumDigNivel { get; set; }
    public string? ParPathImagenes { get; set; }
    public int? LongCodDirectorio { get; set; }
}