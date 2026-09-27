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

        // Las inscripciones son parte del agregado. EF Core escribe
        // directo en la lista privada _inscripciones, porque la propiedad
        // pública es de solo lectura.
        builder.HasMany(e => e.Inscripciones)
               .WithOne()
               .HasForeignKey(i => i.IdEstudiante)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(e => e.Inscripciones)
               .HasField("_inscripciones")
               .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Relaciones hacia otros agregados/catálogos solo por Id (sin
        // propiedades de navegación). Se declaran para que EF Core conozca
        // el orden correcto al insertar y borrar.
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
