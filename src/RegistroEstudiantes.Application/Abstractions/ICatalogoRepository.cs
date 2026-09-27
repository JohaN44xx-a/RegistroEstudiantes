namespace RegistroEstudiantes.Application.Abstractions;

/// <summary>
/// Lo mínimo que el caso de uso necesita saber de una materia para
/// inscribirla: su Id y quién la dicta.
/// </summary>
public record MateriaDelPlan(int IdMateria, int IdProfesor);

/// <summary>
/// Puerto de salida para consultar el catálogo (programas, tipos de
/// documento, materias y planes de estudio). Es de solo lectura: el
/// catálogo se carga con el script SQL.
/// </summary>
public interface ICatalogoRepository
{
    /// <summary>
    /// Devuelve la materia solo si pertenece al plan de estudios del
    /// programa. Si no existe o no es del plan, devuelve null.
    /// </summary>
    Task<MateriaDelPlan?> ObtenerMateriaDelPlanAsync(int idMateria, int idPrograma, CancellationToken ct = default);

    Task<bool> ExisteProgramaAsync(int idPrograma, CancellationToken ct = default);
    Task<bool> ExisteTipoIdentificacionAsync(int idTipoIdentificacion, CancellationToken ct = default);
}
