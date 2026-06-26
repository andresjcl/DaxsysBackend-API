using Daxsys.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Daxsys.Infrastructure.Persistence.Configurations;

public class CompanyParameterConfiguration : IEntityTypeConfiguration<CompanyParameter>
{
    public void Configure(EntityTypeBuilder<CompanyParameter> builder)
    {
        builder.ToTable("Emp_Par");
        builder.HasKey(x => x.EmpCodigo);

        // ==================== CLAVE PRIMARIA ====================
        builder.Property(x => x.EmpCodigo).HasColumnName("Emp_Codigo");

        // ==================== CONFIGURACIÓN CONTABLE ====================
        builder.Property(x => x.DefCtaNumNiveles).HasColumnName("DefCta_NumNiveles");
        builder.Property(x => x.DefCtaNumGrupos).HasColumnName("DefCta_NumGrupos");
        builder.Property(x => x.DefCtaNumDigNivel).HasColumnName("DefCta_NumDigNivel").HasMaxLength(50);
        builder.Property(x => x.DefCtaNumNiveles1).HasColumnName("DefCta_NumNiveles1");
        builder.Property(x => x.DefCtaNumGrupos1).HasColumnName("DefCta_NumGrupos1");
        builder.Property(x => x.DefCtaNumDigNivel1).HasColumnName("DefCta_NumDigNivel1").HasMaxLength(50);
        builder.Property(x => x.DefCtaV).HasColumnName("DefCtaV").HasColumnType("numeric(18,0)");

        // ==================== CIERRE CONTABLE ====================
        // ✅ CORREGIDO: ParContiCierre (no ParConTipCierre)
        builder.Property(x => x.ParContiCierre).HasColumnName("Par_ConTipCierre").HasMaxLength(1);

        // ✅ CORREGIDO: ParInvTipoCierre (no ParInvTipCierre)
        builder.Property(x => x.ParInvTipoCierre).HasColumnName("Par_InvTipCierre").HasMaxLength(1);

        // ==================== IVA ====================
        builder.Property(x => x.ParVenIVA).HasColumnName("Par_VenIVA").HasMaxLength(50);
        builder.Property(x => x.ParComIVA).HasColumnName("Par_ComIVA").HasMaxLength(12);
        builder.Property(x => x.ParClvIVA).HasColumnName("Par_ClvIVA").HasMaxLength(8);

        // ==================== VENTAS ====================
        // ✅ CORREGIDO: ParVensNEm (no ParVenSNEmp)
        builder.Property(x => x.ParVensNEm).HasColumnName("Par_VenSNEmp");

        // ✅ CORREGIDO: ParVensNAcuDoc (no ParVenSNAcuDoc)
        builder.Property(x => x.ParVensNAcuDoc).HasColumnName("Par_VenSNAcuDoc");

        builder.Property(x => x.ParDocPrincipalVta).HasColumnName("Par_DocPrincipalVta").HasMaxLength(5);
        builder.Property(x => x.ParPagoCompras).HasColumnName("Par_PagoCompras").HasMaxLength(3);

        // ==================== COMPRAS ====================
        builder.Property(x => x.ParComSNEmp).HasColumnName("Par_ComSNEmp");
        builder.Property(x => x.ParComSNAcuDoc).HasColumnName("Par_ComSNAcuDoc");

        // ==================== PRESUPUESTOS ====================
        builder.Property(x => x.PrsptoNumNiveles).HasColumnName("Prspto_NumNiveles");
        builder.Property(x => x.PrsptoNumGrupos).HasColumnName("Prspto_NumGrupos");
        builder.Property(x => x.PrsptoNumDigNivel).HasColumnName("Prspto_NumDigNivel").HasMaxLength(50);

        // ==================== ACUMULACIÓN HISTÓRICA ====================
        builder.Property(x => x.ParAcumHis).HasColumnName("Par_AcumHis");
        builder.Property(x => x.ParAcfNumNiv).HasColumnName("Par_AcfNumNiv");

        // ==================== ROLES Y CLAVES ====================
        builder.Property(x => x.ParRolCodMay).HasColumnName("Par_RolCodMay").HasMaxLength(10);
        builder.Property(x => x.ParRolTur).HasColumnName("Par_RolTur");
        builder.Property(x => x.ParClvDsc).HasColumnName("Par_ClvDsc").HasMaxLength(8);

        // ==================== SUCURSAL PRINCIPAL ====================
        builder.Property(x => x.ParSucPri).HasColumnName("Par_SucPri").HasMaxLength(3);

        // ==================== DÍGITOS ====================
        // ✅ CORREGIDO: ParNumerodigitos (no ParNumeroDigitos)
        builder.Property(x => x.ParNumerodigitos).HasColumnName("Par_Numerodigitos");
        builder.Property(x => x.ParDigitosCostos).HasColumnName("Par_DigitosCostos");
        builder.Property(x => x.ParDigitosPrecios).HasColumnName("Par_DigitosPrecios");

        // ==================== FECHAS Y LÍMITES ====================
        builder.Property(x => x.ParFecDes).HasColumnName("Par_FecDes");
        builder.Property(x => x.ParLimAtrasoEntrada).HasColumnName("Par_LimAtrasoEntrada");
        builder.Property(x => x.ParLimExtraSalida).HasColumnName("Par_LimExtraSalida");
        builder.Property(x => x.ParLimExtraEntrada).HasColumnName("Par_LimExtraEntrada");
        builder.Property(x => x.ParDiasMensualesAcf).HasColumnName("par_DiasMensualesAcf");
        builder.Property(x => x.EmpDiasMensualesAcf).HasColumnName("Emp_DiasMensualesAcf");

        // ==================== CHEQUES ====================
        builder.Property(x => x.ParCheques).HasColumnName("Par_Cheques");

        // ==================== CRUCE DE DOCUMENTOS ====================
        builder.Property(x => x.ParCruceDocSucursal).HasColumnName("Par_CruceDocSucursal");

        // ==================== VALIDACIONES SRI ====================
        builder.Property(x => x.ParValiDir).HasColumnName("par_ValiDir");
        builder.Property(x => x.ParValiSRI).HasColumnName("par_ValiSRI");
        builder.Property(x => x.ParUrlSRI).HasColumnName("par_UrlSRI").HasMaxLength(500);

        // ==================== PATHS Y DIRECTORIOS ====================
        builder.Property(x => x.ParPathImagenes).HasColumnName("par_PathImagenes").HasMaxLength(512);
        builder.Property(x => x.EmpPathImagenes).HasColumnName("Emp_PathImagenes").HasMaxLength(512);
        builder.Property(x => x.PathTmpServer).HasColumnName("path_tmpServer").HasMaxLength(200);
        builder.Property(x => x.LongCodDirectorio).HasColumnName("LongCodDirectorio");

        // ==================== CORREO ====================
        builder.Property(x => x.CtaLocalEmail).HasColumnName("CtaLocalEmail").HasMaxLength(200);
    }
}