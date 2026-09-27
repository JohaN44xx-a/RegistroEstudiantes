using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RegistroEstudiantes.Domain.Usuarios;
using RegistroEstudiantes.Infrastructure.Persistencia.Lectura;

namespace RegistroEstudiantes.Infrastructure.Persistencia.Configuraciones;

public class UsuarioConfiguracion : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuario");
        builder.HasKey(u => u.IdUsuario);

        builder.Property(u => u.Nombre).HasMaxLength(100).IsRequired();
        builder.Property(u => u.CorreoElectronico).HasMaxLength(256).IsRequired();
        builder.Property(u => u.ContrasenaHash).HasMaxLength(500).IsRequired();

        builder.HasOne<RolDb>().WithMany()
               .HasForeignKey(u => u.IdRol)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
