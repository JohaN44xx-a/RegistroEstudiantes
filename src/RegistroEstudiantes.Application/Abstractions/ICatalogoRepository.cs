namespace RegistroEstudiantes.Application.Abstractions;

public record MateriaDelPlan(int IdMateria, int IdProfesor);

public interface ICatalogoRepository
{
    // Null si la materia no existe o no pertenece al plan del programa.
    Task<MateriaDelPlan?> ObtenerMateriaDelPlanAsync(int idMateria, int idPrograma, CancellationToken ct = default);

    Task<bool> ExisteProgramaAsync(int idPrograma, CancellationToken ct = default);
    Task<bool> ExisteTipoIdentificacionAsync(int idTipoIdentificacion, CancellationToken ct = default);
}
