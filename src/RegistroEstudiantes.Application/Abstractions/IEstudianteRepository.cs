using RegistroEstudiantes.Domain.Estudiantes;

namespace RegistroEstudiantes.Application.Abstractions;

public interface IEstudianteRepository
{
    // Siempre carga el agregado completo, con sus inscripciones.
    Task<Estudiante?> ObtenerPorIdAsync(int idEstudiante, CancellationToken ct = default);

    Task<int?> ObtenerIdPorUsuarioAsync(int idUsuario, CancellationToken ct = default);

    Task<bool> ExisteIdentificacionAsync(int idTipoIdentificacion, string numeroIdentificacion,
                                         int? excluirIdEstudiante = null, CancellationToken ct = default);

    void Agregar(Estudiante estudiante);
    void Eliminar(Estudiante estudiante);
}
