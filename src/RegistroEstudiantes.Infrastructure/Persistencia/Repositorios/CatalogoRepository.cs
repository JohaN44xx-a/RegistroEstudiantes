using Microsoft.EntityFrameworkCore;
using RegistroEstudiantes.Application.Abstractions;

namespace RegistroEstudiantes.Infrastructure.Persistencia.Repositorios;

public class CatalogoRepository(RegistroEstudiantesDbContext contexto) : ICatalogoRepository
{
    public Task<MateriaDelPlan?> ObtenerMateriaDelPlanAsync(int idMateria, int idPrograma, CancellationToken ct = default)
        => (from pe in contexto.PlanesEstudios
            join m in contexto.Materias on pe.IdMateria equals m.IdMateria
            where pe.IdPrograma == idPrograma && m.IdMateria == idMateria
            select new MateriaDelPlan(m.IdMateria, m.IdProfesor))
           .FirstOrDefaultAsync(ct);

    public Task<bool> ExisteProgramaAsync(int idPrograma, CancellationToken ct = default)
        => contexto.Programas.AnyAsync(p => p.IdPrograma == idPrograma, ct);

    public Task<bool> ExisteTipoIdentificacionAsync(int idTipoIdentificacion, CancellationToken ct = default)
        => contexto.TiposIdentificacion.AnyAsync(t => t.IdTipoIdentificacion == idTipoIdentificacion, ct);
}
