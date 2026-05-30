using Daxsys.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Daxsys.Infrastructure.Persistence.Configurations;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("Emp_Suc");

        builder.HasKey(x => new { x.EmpCodigo, x.SucCodigo });

        builder.Property(x => x.EmpCodigo)
            .HasColumnName("Emp_Codigo");

        builder.Property(x => x.SucCodigo)
            .HasColumnName("Suc_Codigo")
            .HasMaxLength(3);

        builder.Property(x => x.SucNombre)
            .HasColumnName("Suc_Nombre")
            .HasMaxLength(40);

        builder.Property(x => x.SucDireccion)
            .HasColumnName("Suc_Direccion")
            .HasMaxLength(80);

        builder.Property(x => x.SucRuc)
            .HasColumnName("Suc_RUC")
            .HasMaxLength(20);

        builder.Property(x => x.SucSegSocial)
            .HasColumnName("Suc_SegSocial")
            .HasMaxLength(20);

        builder.Property(x => x.SucIdCta)
            .HasColumnName("Suc_IdCta")
            .HasMaxLength(15);

        builder.Property(x => x.BodCodigo)
            .HasColumnName("Bod_Codigo")
            .HasMaxLength(3);

        builder.Property(x => x.SucIdTributario)
            .HasColumnName("Suc_IdTributario")
            .HasMaxLength(10);

        builder.Property(x => x.PrecioVta)
            .HasColumnName("precioVta")
            .HasColumnType("numeric(18,0)");
    }
}