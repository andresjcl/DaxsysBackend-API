using Daxsys.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Daxsys.Infrastructure.Persistence.Configurations;

public class UserDocumentAccessConfiguration : IEntityTypeConfiguration<UserDocumentAccess>
{
    public void Configure(EntityTypeBuilder<UserDocumentAccess> builder)
    {
        builder.ToTable("sysdocaccs");

        builder.HasKey(x => new
        {
            x.Empresa,
            x.IdUsuario,
            x.OpcDocumento,
            x.Opcion
        });

        builder.Property(x => x.Empresa)
            .HasColumnName("empresa");

        builder.Property(x => x.IdUsuario)
            .HasColumnName("idUsuario")
            .HasMaxLength(15);

        builder.Property(x => x.OpcDocumento)
            .HasColumnName("opc_documento")
            .HasMaxLength(3);

        builder.Property(x => x.Opcion)
            .HasColumnName("opcion")
            .HasMaxLength(50);

        builder.Property(x => x.Abierto)
            .HasColumnName("Abierto");

        builder.Property(x => x.Cantidad)
            .HasColumnName("Cantidad");

        builder.Property(x => x.Minimo)
            .HasColumnName("Minimo")
            .HasColumnType("numeric(18,2)");

        builder.Property(x => x.Maximo)
            .HasColumnName("Maximo")
            .HasColumnType("numeric(18,2)");

        builder.Property(x => x.ValorFijo)
            .HasColumnName("ValorFijo")
            .HasMaxLength(50);

        builder.Property(x => x.AuxVal1)
            .HasColumnName("AuxVal1")
            .HasColumnType("numeric(18,2)");

        builder.Property(x => x.AuxVal2)
            .HasColumnName("AuxVal2")
            .HasColumnType("numeric(18,2)");

        builder.Property(x => x.AuxStr1)
            .HasColumnName("AuxStr1")
            .HasMaxLength(100);

        builder.Property(x => x.AuxStr2)
            .HasColumnName("AuxStr2")
            .HasMaxLength(100);
    }
}