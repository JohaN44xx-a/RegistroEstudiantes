using Microsoft.EntityFrameworkCore;
using RegistroEstudiantes.Domain.Estudiantes;
using RegistroEstudiantes.Domain.Usuarios;
using RegistroEstudiantes.Infrastructure.Persistencia.Lectura;

namespace RegistroEstudiantes.Infrastructure.Persistencia;

/// <summary>
/// Mapea las tablas creadas por database/01_RegistroEstudiantes.sql.
/// No se usan migraciones: el script es la fuente de verdad del esquema.
/// </summary>
public class RegistroEstudiantesDbContext(DbContextOptions<RegistroEstudiantesDbContext> options)
    : DbContext(options)
{
    // Dominio (se escriben a través de los repositorios)
    public DbSet<Estudiante> Estudiantes => Set<Estudiante>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    // Catálogo (solo lectura)
    public DbSet<RolDb> Roles => Set<RolDb>();
    public DbSet<TipoIdentificacionDb> TiposIdentificacion => Set<TipoIdentificacionDb>();
    public DbSet<ProgramaDb> Programas => Set<ProgramaDb>();
    public DbSet<ProfesorDb> Profesores => Set<ProfesorDb>();
    public DbSet<MateriaDb> Materias => Set<MateriaDb>();
    public DbSet<PlanEstudiosDb> PlanesEstudios => Set<PlanEstudiosDb>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aplica todas las clases IEntityTypeConfiguration de este proyecto.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RegistroEstudiantesDbContext).Assembly);
    }
}
