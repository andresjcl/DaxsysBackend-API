using Daxsys.Domain.Entities;
using Daxsys.Domain.Entities.Factura;
using Microsoft.EntityFrameworkCore;

namespace Daxsys.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<PointOfSale> PointsOfSale => Set<PointOfSale>();
    public DbSet<CompanyParameter> CompanyParameters => Set<CompanyParameter>();
    public DbSet<CompanyDatabase> CompanyDatabases => Set<CompanyDatabase>();
    public DbSet<UserBranch> UserBranches => Set<UserBranch>();
    public DbSet<UserWarehouse> UserWarehouses => Set<UserWarehouse>();
    public DbSet<UserPointOfSale> UserPointsOfSales => Set<UserPointOfSale>();
    public DbSet<UserAccess> UserAccesses => Set<UserAccess>();
    public DbSet<UserDocument> UserDocuments => Set<UserDocument>();
    public DbSet<UserDocumentAccess> UserDocumentAccesses => Set<UserDocumentAccess>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<AdcArt> AdcArt { get; set; }
    public DbSet<AdcServ> AdcServ { get; set; }
    public DbSet<AdcStk> AdcStk { get; set; }
    public DbSet<AdcDocNum> AdcDocNum { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ✅ CONFIGURACIÓN PARA Company (Emp_Datos)
        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("Emp_Datos");
            entity.HasKey(e => e.EmpCodigo);

            entity.Property(e => e.EmpCodigo).HasColumnName("Emp_codigo");
            entity.Property(e => e.EmpNombre).HasColumnName("Emp_Nombre");
            entity.Property(e => e.EmpPais).HasColumnName("Emp_Pais");
            entity.Property(e => e.EmpProvincia).HasColumnName("Emp_Provincia");
            entity.Property(e => e.EmpCiudad).HasColumnName("Emp_Ciudad");
            entity.Property(e => e.EmpCanton).HasColumnName("Emp_Cantón");
            entity.Property(e => e.EmpDireccion).HasColumnName("Emp_Dirección");
            entity.Property(e => e.EmpTelefono1).HasColumnName("Emp_Telefono_1");
            entity.Property(e => e.EmpTelefono2).HasColumnName("Emp_Telefono_2");
            entity.Property(e => e.EmpFax).HasColumnName("Emp_Fax");
            entity.Property(e => e.EmpCasilla).HasColumnName("Emp_Casilla");
            entity.Property(e => e.EmpEmail).HasColumnName("Emp_Email");
            entity.Property(e => e.EmpRuc).HasColumnName("Emp_RUC");
            entity.Property(e => e.EmpSegSocial).HasColumnName("Emp_SegSocial");
            entity.Property(e => e.EmpPresidente).HasColumnName("Emp_Presidente");
            entity.Property(e => e.EmpGerente).HasColumnName("Emp_Gerente");
            entity.Property(e => e.EmpRepLegal).HasColumnName("Emp_RepLegal");
            entity.Property(e => e.EmpContador).HasColumnName("Emp_Contador");
            entity.Property(e => e.EmpLogotipo).HasColumnName("Emp_Logotipo");
            entity.Property(e => e.EmpDefecto).HasColumnName("Emp_Defecto");
            entity.Property(e => e.EmpLic1).HasColumnName("Emp_Lic1");
            entity.Property(e => e.EmpLic2).HasColumnName("Emp_Lic2");
            entity.Property(e => e.EmpLic3).HasColumnName("Emp_Lic3");
            entity.Property(e => e.EmpTipoBase).HasColumnName("Emp_TipoBase");
            entity.Property(e => e.EmpLic4).HasColumnName("Emp_Lic4");
            entity.Property(e => e.EmpLic5).HasColumnName("Emp_Lic5");
            entity.Property(e => e.EmpLic6).HasColumnName("Emp_Lic6");
            entity.Property(e => e.EmpPathImagenes).HasColumnName("Emp_PathImagenes");
            entity.Property(e => e.EmpConta).HasColumnName("Emp_Conta");
            entity.Property(e => e.EmpAgeRet).HasColumnName("Emp_AgeRet");
            entity.Property(e => e.EmpContrBuyEsp).HasColumnName("Emp_ContrBuyEsp");
            entity.Property(e => e.EmpRegimen).HasColumnName("Emp_Regimen");
        });

        // ✅ CONFIGURACIÓN PARA CompanyDatabase (Arch_Datos)
        modelBuilder.Entity<CompanyDatabase>(entity =>
        {
            entity.ToTable("Emp_Arch");
            entity.HasKey(e => new { e.EmpCodigo, e.ArchTipo });
            entity.Property(e => e.EmpCodigo).HasColumnName("Emp_Codigo");
            entity.Property(e => e.ArchTipo).HasColumnName("Arch_Tipo");
            entity.Property(e => e.ArchNom).HasColumnName("Arch_Nom");
        });

        // ✅ CONFIGURACIÓN PARA CompanyParameter (Emp_Par)
        modelBuilder.Entity<CompanyParameter>(entity =>
        {
            entity.ToTable("Emp_Par");
            entity.HasKey(e => e.EmpCodigo);
            entity.Property(e => e.EmpCodigo).HasColumnName("Emp_Codigo");
            entity.Property(e => e.ParContiCierre).HasColumnName("Par_ConTipCierre");
            entity.Property(e => e.ParInvTipoCierre).HasColumnName("Par_InvTipCierre");
            entity.Property(e => e.ParVenIVA).HasColumnName("Par_VenIVA");
            entity.Property(e => e.ParVensNEm).HasColumnName("Par_VenSNEmp");
            entity.Property(e => e.ParVensNAcuDoc).HasColumnName("Par_VenSNAcuDoc");
            entity.Property(e => e.ParComIVA).HasColumnName("Par_ComIVA");
            entity.Property(e => e.ParComSNEmp).HasColumnName("Par_ComSNEmp");
            entity.Property(e => e.ParComSNAcuDoc).HasColumnName("Par_ComSNAcuDoc");
            entity.Property(e => e.ParSucPri).HasColumnName("Par_SucPri");
            entity.Property(e => e.ParDocPrincipalVta).HasColumnName("Par_DocPrincipalVta");
            entity.Property(e => e.ParDigitosCostos).HasColumnName("Par_DigitosCostos");
            entity.Property(e => e.ParDigitosPrecios).HasColumnName("Par_DigitosPrecios");
            entity.Property(e => e.ParPathImagenes).HasColumnName("par_PathImagenes");
            entity.Property(e => e.EmpPathImagenes).HasColumnName("Emp_PathImagenes");
            entity.Property(e => e.ParUrlSRI).HasColumnName("par_UrlSRI");
        });

        // ✅ CONFIGURACIÓN PARA CompanyParameter (Emp_Par)
        modelBuilder.Entity<CompanyParameter>(entity =>
        {
            entity.ToTable("Emp_Par");
            entity.HasKey(e => e.EmpCodigo);

            entity.Property(e => e.EmpCodigo).HasColumnName("Emp_Codigo");
            entity.Property(e => e.DefCtaNumNiveles).HasColumnName("DefCta_NumNiveles");
            entity.Property(e => e.DefCtaNumGrupos).HasColumnName("DefCta_NumGrupos");
            entity.Property(e => e.DefCtaNumDigNivel).HasColumnName("DefCta_NumDigNivel");
            entity.Property(e => e.DefCtaNumNiveles1).HasColumnName("DefCta_NumNiveles1");
            entity.Property(e => e.DefCtaNumGrupos1).HasColumnName("DefCta_NumGrupos1");
            entity.Property(e => e.DefCtaNumDigNivel1).HasColumnName("DefCta_NumDigNivel1");
            entity.Property(e => e.DefCtaV).HasColumnName("DefCtaV");
            entity.Property(e => e.ParContiCierre).HasColumnName("Par_ConTipCierre");
            entity.Property(e => e.ParInvTipoCierre).HasColumnName("Par_InvTipCierre");
            entity.Property(e => e.ParVenIVA).HasColumnName("Par_VenIVA");
            entity.Property(e => e.ParComIVA).HasColumnName("Par_ComIVA");
            entity.Property(e => e.ParClvIVA).HasColumnName("Par_ClvIVA");
            entity.Property(e => e.ParVensNEm).HasColumnName("Par_VenSNEmp");
            entity.Property(e => e.ParVensNAcuDoc).HasColumnName("Par_VenSNAcuDoc");
            entity.Property(e => e.ParDocPrincipalVta).HasColumnName("Par_DocPrincipalVta");
            entity.Property(e => e.ParPagoCompras).HasColumnName("Par_PagoCompras");
            entity.Property(e => e.ParComSNEmp).HasColumnName("Par_ComSNEmp");
            entity.Property(e => e.ParComSNAcuDoc).HasColumnName("Par_ComSNAcuDoc");
            entity.Property(e => e.PrsptoNumNiveles).HasColumnName("Prspto_NumNiveles");
            entity.Property(e => e.PrsptoNumGrupos).HasColumnName("Prspto_NumGrupos");
            entity.Property(e => e.PrsptoNumDigNivel).HasColumnName("Prspto_NumDigNivel");
            entity.Property(e => e.ParAcumHis).HasColumnName("Par_AcumHis");
            entity.Property(e => e.ParAcfNumNiv).HasColumnName("Par_AcfNumNiv");
            entity.Property(e => e.ParRolCodMay).HasColumnName("Par_RolCodMay");
            entity.Property(e => e.ParRolTur).HasColumnName("Par_RolTur");
            entity.Property(e => e.ParClvDsc).HasColumnName("Par_ClvDsc");
            entity.Property(e => e.ParSucPri).HasColumnName("Par_SucPri");
            entity.Property(e => e.ParNumerodigitos).HasColumnName("Par_Numerodigitos");
            entity.Property(e => e.ParDigitosCostos).HasColumnName("Par_DigitosCostos");
            entity.Property(e => e.ParDigitosPrecios).HasColumnName("Par_DigitosPrecios");
            entity.Property(e => e.ParFecDes).HasColumnName("Par_FecDes");
            entity.Property(e => e.ParLimAtrasoEntrada).HasColumnName("Par_LimAtrasoEntrada");
            entity.Property(e => e.ParLimExtraSalida).HasColumnName("Par_LimExtraSalida");
            entity.Property(e => e.ParLimExtraEntrada).HasColumnName("Par_LimExtraEntrada");
            entity.Property(e => e.ParDiasMensualesAcf).HasColumnName("par_DiasMensualesAcf");
            entity.Property(e => e.EmpDiasMensualesAcf).HasColumnName("Emp_DiasMensualesAcf");
            entity.Property(e => e.ParCheques).HasColumnName("Par_Cheques");
            entity.Property(e => e.ParCruceDocSucursal).HasColumnName("Par_CruceDocSucursal");
            entity.Property(e => e.ParValiDir).HasColumnName("par_ValiDir");
            entity.Property(e => e.ParValiSRI).HasColumnName("par_ValiSRI");
            entity.Property(e => e.ParUrlSRI).HasColumnName("par_UrlSRI");
            entity.Property(e => e.ParPathImagenes).HasColumnName("par_PathImagenes");
            entity.Property(e => e.EmpPathImagenes).HasColumnName("Emp_PathImagenes");
            entity.Property(e => e.PathTmpServer).HasColumnName("path_tmpServer");
            entity.Property(e => e.LongCodDirectorio).HasColumnName("LongCodDirectorio");
            entity.Property(e => e.CtaLocalEmail).HasColumnName("CtaLocalEmail");
        });

        // ✅ CONFIGURACIÓN PARA Branch (Emp_suc)
        modelBuilder.Entity<Branch>(entity =>
        {
            entity.ToTable("Emp_suc");
            entity.HasKey(e => new { e.EmpCodigo, e.SucCodigo });
            entity.Property(e => e.EmpCodigo).HasColumnName("Emp_Codigo");
            entity.Property(e => e.SucCodigo).HasColumnName("Suc_Codigo");
            entity.Property(e => e.SucNombre).HasColumnName("Suc_Nombre");
            entity.Property(e => e.SucDireccion).HasColumnName("Suc_Direccion");
            entity.Property(e => e.SucRuc).HasColumnName("Suc_RUC");
            entity.Property(e => e.SucSegSocial).HasColumnName("Suc_SegSocial");
            entity.Property(e => e.SucIdCta).HasColumnName("Suc_IdCta");
            entity.Property(e => e.BodCodigo).HasColumnName("Bod_Codigo");
            entity.Property(e => e.SucIdTributario).HasColumnName("Suc_IdTributario");
            entity.Property(e => e.PrecioVta).HasColumnName("precioVta");
        });

        // ✅ CONFIGURACIÓN PARA Warehouse (Emp_bod)
        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.ToTable("Emp_bod");
            entity.HasKey(e => new { e.EmpCodigo, e.SucCodigo, e.BodCodigo });
            entity.Property(e => e.EmpCodigo).HasColumnName("Emp_Codigo");
            entity.Property(e => e.SucCodigo).HasColumnName("Suc_Codigo");
            entity.Property(e => e.BodCodigo).HasColumnName("Bod_codigo");
            entity.Property(e => e.BodNombre).HasColumnName("Bod_nombre");
            entity.Property(e => e.BodIdCta).HasColumnName("Bod_IdCta");
        });

        // ✅ CONFIGURACIÓN PARA PointOfSale (Emp_PtoVta)
        modelBuilder.Entity<PointOfSale>(entity =>
        {
            entity.ToTable("Emp_PtoVta");
            entity.HasKey(e => new { e.EmpCodigo, e.SucCodigo, e.PtoCodigo });
            entity.Property(e => e.EmpCodigo).HasColumnName("Emp_Codigo");
            entity.Property(e => e.SucCodigo).HasColumnName("Suc_Codigo");
            entity.Property(e => e.PtoCodigo).HasColumnName("Pto_codigo");
            entity.Property(e => e.PtoNombre).HasColumnName("Pto_nombre");
            entity.Property(e => e.PtoIdTributario).HasColumnName("Pto_IDTributario");
            entity.Property(e => e.PtoIdCta).HasColumnName("Pto_IdCta");
            entity.Property(e => e.TipoPunto).HasColumnName("TipoPunto");
        });

        // ✅ CONFIGURACIÓN PARA sys_Usuario
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("sys_Usuario");
            entity.HasKey(e => e.IdUsuario);
            entity.Property(e => e.IdUsuario).HasColumnName("IdUsuario");
            entity.Property(e => e.CodUsuario).HasColumnName("CodUsuario");
            entity.Property(e => e.FechaInicio).HasColumnName("FechaInicio");
            entity.Property(e => e.FechaCaduca).HasColumnName("FechaCaduca");
            entity.Property(e => e.Contrasena).HasColumnName("Contraseña");
            entity.Property(e => e.FechaCambioContra).HasColumnName("FechaCambioContra");
            entity.Property(e => e.DiasDuraContrasena).HasColumnName("DíasDuraContraseña");
        });

        // ✅ CONFIGURACIÓN PARA AdcArt
        modelBuilder.Entity<AdcArt>(entity =>
        {
            entity.HasKey(e => e.Art_codigo);
            entity.ToTable("AdcArt");
        });

        // ✅ CONFIGURACIÓN PARA AdcServ
        modelBuilder.Entity<AdcServ>(entity =>
        {
            entity.HasKey(e => e.Sev_codigo);
            entity.ToTable("AdcServ");
        });

        // ✅ CONFIGURACIÓN PARA AdcStk
        modelBuilder.Entity<AdcStk>(entity =>
        {
            entity.HasKey(e => new { e.Bod_codigo, e.Art_codigo, e.Suc_codigo });
            entity.ToTable("AdcStk");
        });

        // ✅ CONFIGURACIÓN PARA AdcDocNum
        modelBuilder.Entity<AdcDocNum>(entity =>
        {
            entity.HasKey(e => new { e.Id_Lugar, e.id_Documento });
            entity.ToTable("AdcDocNum");
        });
    }
}