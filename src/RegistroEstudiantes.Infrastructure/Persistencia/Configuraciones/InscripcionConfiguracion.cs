using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RegistroEstudiantes.Domain.Estudiantes;
using RegistroEstudiantes.Infrastructure.Persistencia.Lectura;

namespace RegistroEstudiantes.Infrastructure.Persistencia.Configuraciones;

public class InscripcionConfiguracion : IEntityTypeConfiguration<Inscripcion>
{
    public void Configure(EntityTypeBuilder<Inscripcion> builder)
    {
        builder.ToTable("Inscripcion");
        builder.HasKey(i => i.IdInscripcion);

        builder.Property(i => i.FechaInscripcion).HasColumnType("datetime2(0)");

        // FK compuesta del script: la pareja (materia, profesor) debe existir en Materia.
        builder.HasOne<MateriaDb>().WithMany()
               .HasForeignKey(i => new { i.IdMateria, i.IdProfesor })
               .HasPrincipalKey(m => new { m.IdMateria, m.IdProfesor })
               .OnDelete(DeleteBehavior.Restrict);
    }
}
