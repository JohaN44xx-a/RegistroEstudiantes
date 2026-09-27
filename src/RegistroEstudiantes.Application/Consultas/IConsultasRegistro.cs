namespace RegistroEstudiantes.Application.Consultas;

/// <summary>
/// Puerto de LECTURA. Las consultas no modifican nada, así que no pasan
/// por el agregado: Infrastructure las resuelve directo con proyecciones
/// a DTO, que es más simple y más rápido. Las escrituras sí pasan
/// siempre por el dominio (separación ligera de comandos y consultas).
/// </summary>
public interface IConsultasRegistro
{
    Task<IReadOnlyList<CatalogoItemDto>> ListarProgramasAsync(CancellationToken ct = default);
    Task<IReadOnlyList<CatalogoItemDto>> ListarTiposIdentificacionAsync(CancellationToken ct = default);

    /// <summary>Materias del plan de estudios del programa del estudiante.</summary>
    Task<IReadOnlyList<MateriaDisponibleDto>> ListarMateriasDisponiblesAsync(int idEstudiante, CancellationToken ct = default);

    /// <summary>Datos del estudiante, sus materias y sus compañeros por clase.</summary>
    Task<MiRegistroDto?> ObtenerMiRegistroAsync(int idEstudiante, CancellationToken ct = default);

    /// <summary>Regla 8: registros de los demás estudiantes.</summary>
    Task<IReadOnlyList<RegistroPublicoDto>> ListarRegistrosAsync(CancellationToken ct = default);
}
