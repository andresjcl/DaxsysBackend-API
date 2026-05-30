using Daxsys.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Daxsys.Infrastructure.Persistence.Configurations;

public class UserWarehouseConfiguration : IEntityTypeConfiguration<UserWarehouse>
{
    public void Configure(EntityTypeBuilder<UserWarehouse> builder)
    {
        builder.ToTable("sys_Bodegas");

        builder.HasKey(x => new { x.IdUsuario, x.CodBodega });

        builder.Property(x => x.IdUsuario)
            .HasColumnName("IdUsuario")
            .HasMaxLength(15);

        builder.Property(x => x.IdEmpresa)
            .HasColumnName("IdEmpresa");

        builder.Property(x => x.CodSucursal)
            .HasColumnName("CodSucursal")
            .HasMaxLength(3);

        builder.Property(x => x.CodBodega)
            .HasColumnName("CodBodega")
            .HasMaxLength(3);

        builder.Property(x => x.AutorizaBod)
            .HasColumnName("AutorizaBod")
            .HasMaxLength(1);
    }
}