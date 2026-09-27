namespace RegistroEstudiantes.Application.Consultas;

public record CatalogoItemDto(int Id, string Nombre);

public record MateriaDisponibleDto(int IdMateria, string Nombre, int Creditos, int IdProfesor,
                                   string Profesor, bool Inscrita);

// Regla 9: de los compañeros solo se expone el nombre.
public record MateriaInscritaDto(int IdMateria, string Nombre, int Creditos, string Profesor,
                                 IReadOnlyList<string> Companeros);

public record MiRegistroDto(int IdEstudiante, string Nombre, int IdTipoIdentificacion,
                            string TipoIdentificacion, string NumeroIdentificacion, string Programa,
                            string CorreoElectronico, int TotalCreditos,
                            IReadOnlyList<MateriaInscritaDto> Materias);

// Regla 8: sin documento ni correo.
public record RegistroPublicoDto(string Estudiante, string Programa, IReadOnlyList<string> Materias);
