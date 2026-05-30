using Daxsys.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Daxsys.Infrastructure.Persistence.Configurations;

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable("Emp_Bod");

        builder.HasKey(x => new { x.EmpCodigo, x.SucCodigo, x.BodCodigo });

        builder.Property(x => x.EmpCodigo)
            .HasColumnName("Emp_Codigo");

        builder.Property(x => x.SucCodigo)
            .HasColumnName("Suc_Codigo")
            .HasMaxLength(3);

        builder.Property(x => x.BodCodigo)
            .HasColumnName("Bod_codigo")
            .HasMaxLength(3);

        builder.Property(x => x.BodNombre)
            .HasColumnName("Bod_nombre")
            .HasMaxLength(40);

        builder.Property(x => x.BodIdCta)
            .HasColumnName("Bod_IdCta")
            .HasMaxLength(15);
    }
}