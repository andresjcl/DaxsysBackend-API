using Daxsys.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Daxsys.Infrastructure.Persistence.Configurations;

public class MenuConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.ToTable("sys_Menu");

        builder.HasKey(x => x.IdMenu);

        builder.Property(x => x.IdMenu).HasColumnName("IdMenu");
        builder.Property(x => x.IdPadre).HasColumnName("IdPadre");
        builder.Property(x => x.IdSistema).HasColumnName("IdSistema").HasMaxLength(3);
        builder.Property(x => x.Codigo).HasColumnName("Codigo").HasMaxLength(50);
        builder.Property(x => x.Nombre).HasColumnName("Nombre").HasMaxLength(150);
        builder.Property(x => x.Ruta).HasColumnName("Ruta").HasMaxLength(250);
        builder.Property(x => x.Icono).HasColumnName("Icono").HasMaxLength(100);
        builder.Property(x => x.Orden).HasColumnName("Orden");
        builder.Property(x => x.Nivel).HasColumnName("Nivel");
        builder.Property(x => x.Activo).HasColumnName("Activo");
    }
}