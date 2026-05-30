using Daxsys.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Daxsys.Infrastructure.Persistence.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Emp_Datos");

        builder.HasKey(x => x.EmpCodigo);

        builder.Property(x => x.EmpCodigo)
            .HasColumnName("Emp_codigo");

        builder.Property(x => x.EmpNombre)
            .HasColumnName("Emp_Nombre")
            .HasMaxLength(50);

        builder.Property(x => x.EmpPais)
            .HasColumnName("Emp_Pais")
            .HasMaxLength(25);

        builder.Property(x => x.EmpProvincia)
            .HasColumnName("Emp_Provincia")
            .HasMaxLength(25);

        builder.Property(x => x.EmpCiudad)
            .HasColumnName("Emp_Ciudad")
            .HasMaxLength(25);

        builder.Property(x => x.EmpCanton)
            .HasColumnName("Emp_Cantón")
            .HasMaxLength(25);

        builder.Property(x => x.EmpDireccion)
            .HasColumnName("Emp_Dirección")
            .HasMaxLength(80);

        builder.Property(x => x.EmpTelefono1)
            .HasColumnName("Emp_Telefono_1")
            .HasMaxLength(15);

        builder.Property(x => x.EmpTelefono2)
            .HasColumnName("Emp_Telefono_2")
            .HasMaxLength(15);

        builder.Property(x => x.EmpFax)
            .HasColumnName("Emp_Fax")
            .HasMaxLength(15);

        builder.Property(x => x.EmpCasilla)
            .HasColumnName("Emp_Casilla")
            .HasMaxLength(15);

        builder.Property(x => x.EmpEmail)
            .HasColumnName("Emp_Email")
            .HasMaxLength(50);

        builder.Property(x => x.EmpRuc)
            .HasColumnName("Emp_RUC")
            .HasMaxLength(20);

        builder.Property(x => x.EmpSegSocial)
            .HasColumnName("Emp_SegSocial")
            .HasMaxLength(50);

        builder.Property(x => x.EmpPresidente)
            .HasColumnName("Emp_Presidente")
            .HasMaxLength(40);

        builder.Property(x => x.EmpGerente)
            .HasColumnName("Emp_Gerente")
            .HasMaxLength(50);

        builder.Property(x => x.EmpRepLegal)
            .HasColumnName("Emp_RepLegal")
            .HasMaxLength(40);

        builder.Property(x => x.EmpContador)
            .HasColumnName("Emp_Contador")
            .HasMaxLength(50);

        builder.Property(x => x.EmpLogotipo)
            .HasColumnName("Emp_Logotipo")
            .HasMaxLength(80);

        builder.Property(x => x.EmpDefecto)
            .HasColumnName("Emp_Defecto");

        builder.Property(x => x.EmpLic1)
            .HasColumnName("Emp_Lic1")
            .HasMaxLength(128);

        builder.Property(x => x.EmpLic2)
            .HasColumnName("Emp_Lic2")
            .HasMaxLength(128);

        builder.Property(x => x.EmpLic3)
            .HasColumnName("Emp_Lic3")
            .HasMaxLength(128);

        builder.Property(x => x.EmpPathImagenes)
            .HasColumnName("Emp_PathImagenes")
            .HasMaxLength(512);

        builder.Property(x => x.EmpLic4)
            .HasColumnName("Emp_Lic4")
            .HasMaxLength(128);

        builder.Property(x => x.EmpLic5)
            .HasColumnName("Emp_Lic5")
            .HasMaxLength(128);

        builder.Property(x => x.EmpLic6)
            .HasColumnName("Emp_Lic6")
            .HasMaxLength(128);

        builder.Property(x => x.EmpTipoBase)
            .HasColumnName("Emp_TipoBase")
            .HasMaxLength(3);
    }
}