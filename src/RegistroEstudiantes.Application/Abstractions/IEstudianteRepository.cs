using RegistroEstudiantes.Domain.Estudiantes;

namespace RegistroEstudiantes.Application.Abstractions;

/// <summary>
/// Puerto de salida para el agregado Estudiante.
/// Siempre devuelve al estudiante CON sus inscripciones, porque el
/// agregado necesita toda su información para aplicar las reglas.
/// </summary>
public interface IEstudianteRepository
{
    Task<Estudiante?> ObtenerPorIdAsync(int idEstudiante, CancellationToken ct = default);

    /// <summary>Solo el Id, sin cargar el agregado. Se usa en el login.</summary>
    Task<int?> ObtenerIdPorUsuarioAsync(int idUsuario, CancellationToken ct = default);

    /// <summary>
    /// ¿Ya hay otro estudiante con este documento? excluirIdEstudiante
    /// permite que un estudiante actualice sus datos sin chocar consigo mismo.
    /// </summary>
    Task<bool> ExisteIdentificacionAsync(int idTipoIdentificacion, string numeroIdentificacion, int? excluirIdEstudiante = null, CancellationToken ct = default);

    void Agregar(Estudiante estudiante);
    void Eliminar(Estudiante estudiante);
}
