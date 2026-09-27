using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RegistroEstudiantes.Domain.Estudiantes;
using RegistroEstudiantes.Domain.Usuarios;
using RegistroEstudiantes.Infrastructure.Persistencia.Lectura;

namespace RegistroEstudiantes.Infrastructure.Persistencia.Configuraciones;

public class EstudianteConfiguracion : IEntityTypeConfiguration<Estudiante>
{
    public void Configure(EntityTypeBuilder<Estudiante> builder)
    {
        builder.ToTable("Estudiante");
        builder.HasKey(e => e.IdEstudiante);

        builder.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
        builder.Property(e => e.NumeroIdentificacion).HasMaxLength(20).IsRequired();

        builder.HasMany(e => e.Inscripciones)
               .WithOne()
               .HasForeignKey(i => i.IdEstudiante)
               .OnDelete(DeleteBehavior.Cascade);

        // EF Core escribe en la lista privada; la propiedad pública es de solo lectura.
        builder.Navigation(e => e.Inscripciones)
               .HasField("_inscripciones")
               .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Relaciones solo por Id; se declaran para que EF Core ordene las inserciones y los borrados.
        builder.HasOne<Usuario>().WithOne()
               .HasForeignKey<Estudiante>(e => e.IdUsuario)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ProgramaDb>().WithMany()
               .HasForeignKey(e => e.IdPrograma)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<TipoIdentificacionDb>().WithMany()
               .HasForeignKey(e => e.IdTipoIdentificacion)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
