using Daxsys.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daxsys.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("sys_Usuario");

        builder.HasKey(x => x.IdUsuario);

        builder.Property(x => x.IdUsuario)
            .HasColumnName("IdUsuario")
            .HasMaxLength(15)
            .IsRequired();

        builder.Property(x => x.CodUsuario)
            .HasColumnName("CodUsuario")
            .HasMaxLength(15);

        builder.Property(x => x.FechaInicio)
            .HasColumnName("FechaInicio");

        builder.Property(x => x.FechaCaduca)
            .HasColumnName("FechaCaduca");

        builder.Property(x => x.Contrasena)
            .HasColumnName("Contraseña")
            .HasMaxLength(15);

        builder.Property(x => x.FechaCambioContra)
            .HasColumnName("FechaCambioContra");

        builder.Property(x => x.DiasDuraContrasena)
            .HasColumnName("DíasDuraContraseña");
    }
}
