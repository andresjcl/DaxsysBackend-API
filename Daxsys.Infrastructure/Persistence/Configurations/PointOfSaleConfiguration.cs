using Daxsys.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Daxsys.Infrastructure.Persistence.Configurations;

public class PointOfSaleConfiguration : IEntityTypeConfiguration<PointOfSale>
{
    public void Configure(EntityTypeBuilder<PointOfSale> builder)
    {
        builder.ToTable("Emp_PtoVta");

        builder.HasKey(x => new { x.EmpCodigo, x.SucCodigo, x.PtoCodigo });

        builder.Property(x => x.EmpCodigo)
            .HasColumnName("Emp_Codigo");

        builder.Property(x => x.SucCodigo)
            .HasColumnName("Suc_Codigo")
            .HasMaxLength(3);

        builder.Property(x => x.PtoCodigo)
            .HasColumnName("Pto_codigo")
            .HasMaxLength(3);

        builder.Property(x => x.PtoNombre)
            .HasColumnName("Pto_nombre")
            .HasMaxLength(40);

        builder.Property(x => x.PtoIdTributario)
            .HasColumnName("Pto_IDTributario")
            .HasMaxLength(10);

        builder.Property(x => x.PtoIdCta)
            .HasColumnName("Pto_IdCta")
            .HasMaxLength(15);

        builder.Property(x => x.TipoPunto)
            .HasColumnName("TipoPunto")
            .HasMaxLength(5);
    }
}