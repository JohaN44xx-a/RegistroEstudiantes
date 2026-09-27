using Microsoft.EntityFrameworkCore;
using RegistroEstudiantes.Application.Abstractions;
using RegistroEstudiantes.Domain.Estudiantes;

namespace RegistroEstudiantes.Infrastructure.Persistencia.Repositorios;

public class EstudianteRepository(RegistroEstudiantesDbContext contexto) : IEstudianteRepository
{
    public Task<Estudiante?> ObtenerPorIdAsync(int idEstudiante, CancellationToken ct = default)
        => contexto.Estudiantes
                   .Include(e => e.Inscripciones) // el agregado siempre completo
                   .FirstOrDefaultAsync(e => e.IdEstudiante == idEstudiante, ct);

    public Task<int?> ObtenerIdPorUsuarioAsync(int idUsuario, CancellationToken ct = default)
        => contexto.Estudiantes
                   .Where(e => e.IdUsuario == idUsuario)
                   .Select(e => (int?)e.IdEstudiante)
                   .FirstOrDefaultAsync(ct);

    public Task<bool> ExisteIdentificacionAsync(int idTipoIdentificacion, string numeroIdentificacion,
                                                int? excluirIdEstudiante = null, CancellationToken ct = default)
        => contexto.Estudiantes.AnyAsync(e =>
               e.IdTipoIdentificacion == idTipoIdentificacion &&
               e.NumeroIdentificacion == numeroIdentificacion &&
               (excluirIdEstudiante == null || e.IdEstudiante != excluirIdEstudiante), ct);

    public void Agregar(Estudiante estudiante) => contexto.Estudiantes.Add(estudiante);

    public void Eliminar(Estudiante estudiante) => contexto.Estudiantes.Remove(estudiante);
}
