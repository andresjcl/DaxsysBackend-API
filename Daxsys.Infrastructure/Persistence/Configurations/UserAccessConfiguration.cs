using Daxsys.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Daxsys.Infrastructure.Persistence.Configurations;

public class UserAccessConfiguration : IEntityTypeConfiguration<UserAccess>
{
    public void Configure(EntityTypeBuilder<UserAccess> builder)
    {
        builder.ToTable("sys_Accesos");

        builder.HasKey(x => x.IdClaveDoc);

        builder.Property(x => x.IdUsuario)
            .HasColumnName("IdUsuario")
            .HasMaxLength(15);

        builder.Property(x => x.IdEmpresa)
            .HasColumnName("IdEmpresa");

        builder.Property(x => x.IdSistema)
            .HasColumnName("IdSistema")
            .HasMaxLength(3);

        builder.Property(x => x.IdOpcion)
            .HasColumnName("IdOpcion")
            .HasMaxLength(255);

        builder.Property(x => x.IdNomOpcion)
            .HasColumnName("IdNomOpcion")
            .HasMaxLength(512);

        builder.Property(x => x.Accesos)
            .HasColumnName("Accesos")
            .HasMaxLength(512);

        builder.Property(x => x.IdClaveDoc)
    .HasColumnName("idclavedoc")
    .HasColumnType("numeric(18,0)")
    .ValueGeneratedOnAdd();
    }
}