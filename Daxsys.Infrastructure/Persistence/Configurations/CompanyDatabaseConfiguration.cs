using Daxsys.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Daxsys.Infrastructure.Persistence.Configurations;

public class CompanyDatabaseConfiguration : IEntityTypeConfiguration<CompanyDatabase>
{
    public void Configure(EntityTypeBuilder<CompanyDatabase> builder)
    {
        builder.ToTable("Emp_Arch");

        builder.HasKey(x => new { x.EmpCodigo, x.ArchTipo });

        builder.Property(x => x.EmpCodigo)
            .HasColumnName("Emp_Codigo");

        builder.Property(x => x.ArchTipo)
            .HasColumnName("Arch_Tipo")
            .HasMaxLength(3);

        builder.Property(x => x.ArchNom)
            .HasColumnName("Arch_Nom")
            .HasMaxLength(70);
    }
}