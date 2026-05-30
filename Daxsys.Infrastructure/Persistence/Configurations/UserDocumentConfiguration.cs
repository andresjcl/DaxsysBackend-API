using Daxsys.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Daxsys.Infrastructure.Persistence.Configurations;

public class UserDocumentConfiguration : IEntityTypeConfiguration<UserDocument>
{
    public void Configure(EntityTypeBuilder<UserDocument> builder)
    {
        builder.ToTable("sys_Documentos");

        builder.HasKey(x => new
        {
            x.IdUsuario,
            x.IdEmpresa,
            x.CodDocumento
        });

        builder.Property(x => x.IdUsuario)
            .HasColumnName("IdUsuario")
            .HasMaxLength(15)
            .IsRequired();

        builder.Property(x => x.IdEmpresa)
            .HasColumnName("idEmpresa")
            .IsRequired();

        builder.Property(x => x.CodDocumento)
            .HasColumnName("CodDocumento")
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(x => x.Cambios)
            .HasColumnName("Cambios")
            .HasMaxLength(15);
    }
}