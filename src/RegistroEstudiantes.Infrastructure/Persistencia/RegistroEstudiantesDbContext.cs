using Microsoft.EntityFrameworkCore;
using RegistroEstudiantes.Domain.Estudiantes;
using RegistroEstudiantes.Domain.Usuarios;
using RegistroEstudiantes.Infrastructure.Persistencia.Lectura;

namespace RegistroEstudiantes.Infrastructure.Persistencia;

public class RegistroEstudiantesDbContext(DbContextOptions<RegistroEstudiantesDbContext> options)
    : DbContext(options)
{
    public DbSet<Estudiante> Estudiantes => Set<Estudiante>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<RolDb> Roles => Set<RolDb>();
    public DbSet<TipoIdentificacionDb> TiposIdentificacion => Set<TipoIdentificacionDb>();
    public DbSet<ProgramaDb> Programas => Set<ProgramaDb>();
    public DbSet<ProfesorDb> Profesores => Set<ProfesorDb>();
    public DbSet<MateriaDb> Materias => Set<MateriaDb>();
    public DbSet<PlanEstudiosDb> PlanesEstudios => Set<PlanEstudiosDb>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RegistroEstudiantesDbContext).Assembly);
    }
}
