namespace RegistroEstudiantes.Application.Consultas;

public record CatalogoItemDto(int Id, string Nombre);

/// <summary>
/// Materia del plan del estudiante. Inscrita indica si ya la tiene; así
/// la interfaz puede mostrarla marcada. La decisión final de si se puede
/// inscribir SIEMPRE la toma el dominio.
/// </summary>
public record MateriaDisponibleDto(int IdMateria, string Nombre, int Creditos, int IdProfesor,
                                   string Profesor, bool Inscrita);

/// <summary>
/// Regla 9: de los compañeros solo se expone el nombre.
/// </summary>
public record MateriaInscritaDto(int IdMateria, string Nombre, int Creditos, string Profesor,
                                 IReadOnlyList<string> Companeros);

public record MiRegistroDto(int IdEstudiante, string Nombre, int IdTipoIdentificacion,
                            string TipoIdentificacion, string NumeroIdentificacion, string Programa,
                            string CorreoElectronico, int TotalCreditos,
                            IReadOnlyList<MateriaInscritaDto> Materias);

/// <summary>
/// Regla 8: lo que un estudiante puede ver del registro de otro.
/// Sin documento ni correo: solo nombre, programa y materias.
/// </summary>
public record RegistroPublicoDto(string Estudiante, string Programa, IReadOnlyList<string> Materias);
