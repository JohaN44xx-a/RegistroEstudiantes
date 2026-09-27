using RegistroEstudiantes.Application.Abstractions;
using RegistroEstudiantes.Application.Common;

namespace RegistroEstudiantes.Application.Estudiantes;

public class CancelarInscripcion(
    IEstudianteRepository estudiantes,
    IUnidadDeTrabajo unidadDeTrabajo)
{
    public async Task EjecutarAsync(int idEstudiante, int idMateria, CancellationToken ct = default)
    {
        var estudiante = await estudiantes.ObtenerPorIdAsync(idEstudiante, ct)
            ?? throw new NoEncontradoException("El estudiante no existe.");

        estudiante.CancelarInscripcion(idMateria);

        await unidadDeTrabajo.GuardarCambiosAsync(ct);
    }
}
