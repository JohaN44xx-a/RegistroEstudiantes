using RegistroEstudiantes.Application.Abstractions;
using RegistroEstudiantes.Application.Common;
using RegistroEstudiantes.Domain.Common;

namespace RegistroEstudiantes.Application.Estudiantes;

public record ActualizarEstudianteRequest(string Nombre, int IdTipoIdentificacion, string NumeroIdentificacion);

/// <summary>
/// Actualización: El programa no se puede cambiar aquí: si cambiara, las
/// materias inscritas podrían dejar de pertenecer a su plan de estudios.
/// </summary>
public class ActualizarEstudiante(IEstudianteRepository estudiantes, IUsuarioRepository usuarios, ICatalogoRepository catalogo, IUnidadDeTrabajo unidadDeTrabajo)
{
    public async Task EjecutarAsync(int idEstudiante, ActualizarEstudianteRequest request, CancellationToken ct = default)
    {
        var estudiante = await estudiantes.ObtenerPorIdAsync(idEstudiante, ct)
            ?? throw new NoEncontradoException("El estudiante no existe.");

        if (!await catalogo.ExisteTipoIdentificacionAsync(request.IdTipoIdentificacion, ct))
            throw new ReglaDeNegocioException("El tipo de identificación seleccionado no existe.");

        if (!string.IsNullOrWhiteSpace(request.NumeroIdentificacion) && await estudiantes.ExisteIdentificacionAsync(request.IdTipoIdentificacion, request.NumeroIdentificacion.Trim(), idEstudiante, ct))
            throw new ReglaDeNegocioException("Ya existe otro estudiante registrado con este documento.");

        estudiante.ActualizarDatos(request.Nombre, request.IdTipoIdentificacion, request.NumeroIdentificacion);

        // El nombre de la cuenta se mantiene igual al del estudiante.
        var usuario = await usuarios.ObtenerPorIdAsync(estudiante.IdUsuario, ct);
        usuario?.ActualizarNombre(estudiante.Nombre);

        await unidadDeTrabajo.GuardarCambiosAsync(ct);
    }
}
