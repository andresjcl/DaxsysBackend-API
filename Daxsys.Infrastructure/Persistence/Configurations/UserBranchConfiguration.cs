using Daxsys.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Daxsys.Infrastructure.Persistence.Configurations;

public class UserBranchConfiguration : IEntityTypeConfiguration<UserBranch>
{
    public void Configure(EntityTypeBuilder<UserBranch> builder)
    {
        builder.ToTable("sys_Sucursales");

        builder.HasKey(x => new { x.IdUsuario, x.CodSucursal });

        builder.Property(x => x.IdUsuario)
            .HasColumnName("IdUsuario")
            .HasMaxLength(15);

        builder.Property(x => x.IdEmpresa)
            .HasColumnName("IdEmpresa");

        builder.Property(x => x.CodSucursal)
            .HasColumnName("CodSucursal")
            .HasMaxLength(3);

        builder.Property(x => x.AutorizaSuc)
            .HasColumnName("AutorizaSuc")
            .HasMaxLength(1);
    }
}