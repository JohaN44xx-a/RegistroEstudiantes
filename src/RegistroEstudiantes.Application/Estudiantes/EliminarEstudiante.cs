using RegistroEstudiantes.Application.Abstractions;
using RegistroEstudiantes.Application.Common;

namespace RegistroEstudiantes.Application.Estudiantes;

/// <summary>
/// "D" del CRUD. Borra el estudiante (sus inscripciones se van con él,
/// porque son parte del agregado) y su cuenta de usuario.
/// </summary>
public class EliminarEstudiante(IEstudianteRepository estudiantes, IUsuarioRepository usuarios, IUnidadDeTrabajo unidadDeTrabajo)
{
    public async Task EjecutarAsync(int idEstudiante, CancellationToken ct = default)
    {
        var estudiante = await estudiantes.ObtenerPorIdAsync(idEstudiante, ct)
            ?? throw new NoEncontradoException("El estudiante no existe.");

        var usuario = await usuarios.ObtenerPorIdAsync(estudiante.IdUsuario, ct);

        estudiantes.Eliminar(estudiante);
        if (usuario is not null)
            usuarios.Eliminar(usuario);

        // Un solo guardado = una sola transacción. EF Core ordena los
        // DELETE según las relaciones: inscripciones, estudiante, usuario.
        await unidadDeTrabajo.GuardarCambiosAsync(ct);
    }
}
