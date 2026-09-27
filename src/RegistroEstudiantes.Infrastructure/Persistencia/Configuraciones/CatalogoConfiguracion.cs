using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RegistroEstudiantes.Infrastructure.Persistencia.Lectura;

namespace RegistroEstudiantes.Infrastructure.Persistencia.Configuraciones;

public class RolConfiguracion : IEntityTypeConfiguration<RolDb>
{
    public void Configure(EntityTypeBuilder<RolDb> builder)
    {
        builder.ToTable("Rol");
        builder.HasKey(r => r.IdRol);
    }
}

public class TipoIdentificacionConfiguracion : IEntityTypeConfiguration<TipoIdentificacionDb>
{
    public void Configure(EntityTypeBuilder<TipoIdentificacionDb> builder)
    {
        builder.ToTable("TipoIdentificacion");
        builder.HasKey(t => t.IdTipoIdentificacion);
    }
}

public class ProgramaConfiguracion : IEntityTypeConfiguration<ProgramaDb>
{
    public void Configure(EntityTypeBuilder<ProgramaDb> builder)
    {
        builder.ToTable("Programa");
        builder.HasKey(p => p.IdPrograma);
    }
}

public class ProfesorConfiguracion : IEntityTypeConfiguration<ProfesorDb>
{
    public void Configure(EntityTypeBuilder<ProfesorDb> builder)
    {
        builder.ToTable("Profesor");
        builder.HasKey(p => p.IdProfesor);
    }
}

public class MateriaConfiguracion : IEntityTypeConfiguration<MateriaDb>
{
    public void Configure(EntityTypeBuilder<MateriaDb> builder)
    {
        builder.ToTable("Materia");
        builder.HasKey(m => m.IdMateria);

        builder.HasOne<ProfesorDb>().WithMany()
               .HasForeignKey(m => m.IdProfesor)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class PlanEstudiosConfiguracion : IEntityTypeConfiguration<PlanEstudiosDb>
{
    public void Configure(EntityTypeBuilder<PlanEstudiosDb> builder)
    {
        builder.ToTable("PlanEstudios");
        builder.HasKey(pe => new { pe.IdPrograma, pe.IdMateria });
    }
}
