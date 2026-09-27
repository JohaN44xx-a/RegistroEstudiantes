using RegistroEstudiantes.Application.Abstractions;
using RegistroEstudiantes.Application.Common;

namespace RegistroEstudiantes.Application.Estudiantes;

public class EliminarEstudiante(
    IEstudianteRepository estudiantes,
    IUsuarioRepository usuarios,
    IUnidadDeTrabajo unidadDeTrabajo)
{
    public async Task EjecutarAsync(int idEstudiante, CancellationToken ct = default)
    {
        var estudiante = await estudiantes.ObtenerPorIdAsync(idEstudiante, ct)
            ?? throw new NoEncontradoException("El estudiante no existe.");

        var usuario = await usuarios.ObtenerPorIdAsync(estudiante.IdUsuario, ct);

        estudiantes.Eliminar(estudiante);
        if (usuario is not null)
            usuarios.Eliminar(usuario);

        await unidadDeTrabajo.GuardarCambiosAsync(ct);
    }
}
