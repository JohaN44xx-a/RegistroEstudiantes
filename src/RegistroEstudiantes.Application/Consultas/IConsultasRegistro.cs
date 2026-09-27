namespace RegistroEstudiantes.Application.Consultas;

// Lecturas: proyectan directo a DTO sin pasar por el agregado, porque no modifican estado.
public interface IConsultasRegistro
{
    Task<IReadOnlyList<CatalogoItemDto>> ListarProgramasAsync(CancellationToken ct = default);
    Task<IReadOnlyList<CatalogoItemDto>> ListarTiposIdentificacionAsync(CancellationToken ct = default);

    Task<IReadOnlyList<MateriaDisponibleDto>> ListarMateriasDisponiblesAsync(int idEstudiante, CancellationToken ct = default);

    Task<MiRegistroDto?> ObtenerMiRegistroAsync(int idEstudiante, CancellationToken ct = default);

    Task<IReadOnlyList<RegistroPublicoDto>> ListarRegistrosAsync(CancellationToken ct = default);
}
