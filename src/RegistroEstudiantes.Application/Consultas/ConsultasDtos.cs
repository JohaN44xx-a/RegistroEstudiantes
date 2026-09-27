namespace RegistroEstudiantes.Application.Consultas;

public record CatalogoItemDto(int Id, string Nombre);

public record MateriaDisponibleDto(int IdMateria, string Nombre, int Creditos, int IdProfesor, string Profesor);

/// <summary>
/// Regla 9: de los compañeros solo se expone el nombre.
/// </summary>
public record MateriaInscritaDto(int IdMateria, string Nombre, int Creditos, string Profesor, IReadOnlyList<string> Companeros);

public record MiRegistroDto(int IdEstudiante, string Nombre, string TipoIdentificacion, string NumeroIdentificacion, string Programa, string CorreoElectronico, 
                            int TotalCreditos, IReadOnlyList<MateriaInscritaDto> Materias);

/// <summary>
/// Regla 8: lo que un estudiante puede ver del registro de otro.
/// Sin documento ni correo: solo nombre, programa y materias.
/// </summary>
public record RegistroPublicoDto(string Estudiante, string Programa, IReadOnlyList<string> Materias);
