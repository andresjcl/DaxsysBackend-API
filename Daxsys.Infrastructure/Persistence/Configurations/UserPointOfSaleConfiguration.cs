using Daxsys.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Daxsys.Infrastructure.Persistence.Configurations;

public class UserPointOfSaleConfiguration : IEntityTypeConfiguration<UserPointOfSale>
{
    public void Configure(EntityTypeBuilder<UserPointOfSale> builder)
    {
        builder.ToTable("sys_ptoVta");

        builder.HasKey(x => new { x.IdUsuario, x.IdEmpresa, x.CodSucursal, x.CodPtoVta });

        builder.Property(x => x.IdUsuario)
            .HasColumnName("IdUsuario")
            .HasMaxLength(15);

        builder.Property(x => x.IdEmpresa)
            .HasColumnName("IdEmpresa");

        builder.Property(x => x.CodSucursal)
            .HasColumnName("CodSucursal")
            .HasMaxLength(3);

        builder.Property(x => x.CodPtoVta)
            .HasColumnName("CodptoVta")
            .HasMaxLength(3);

        builder.Property(x => x.AutorizaPtoVta)
            .HasColumnName("AutorizaPtoVta")
            .HasMaxLength(1);
    }
}