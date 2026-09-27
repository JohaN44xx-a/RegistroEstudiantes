using RegistroEstudiantes.Application.Abstractions;
using RegistroEstudiantes.Domain.Common;
using RegistroEstudiantes.Domain.Estudiantes;
using RegistroEstudiantes.Domain.Usuarios;

namespace RegistroEstudiantes.Application.Estudiantes;

public record RegistrarEstudianteRequest(
    string Nombre,
    string CorreoElectronico,
    string Contrasena,
    int IdTipoIdentificacion,
    string NumeroIdentificacion,
    int IdPrograma);

/// <summary>
/// "C" del CRUD: registro en línea. Crea la cuenta (Usuario) y el
/// Estudiante en una sola transacción: o quedan los dos, o ninguno.
/// </summary>
public class RegistrarEstudiante(IUsuarioRepository usuarios, IEstudianteRepository estudiantes, ICatalogoRepository catalogo, IHasherContrasenas hasher, IUnidadDeTrabajo unidadDeTrabajo)
{
    public const int LongitudMinimaContrasena = 8;

    public async Task<int> EjecutarAsync(RegistrarEstudianteRequest request, CancellationToken ct = default)
    {
        // 1. Validaciones que necesitan consultar la base o que son
        //    políticas de la aplicación (no reglas del estudiante).
        if (string.IsNullOrWhiteSpace(request.Contrasena) || request.Contrasena.Length < LongitudMinimaContrasena)
            throw new ReglaDeNegocioException(
                $"La contraseña debe tener al menos {LongitudMinimaContrasena} caracteres.");

        if (string.IsNullOrWhiteSpace(request.CorreoElectronico))
            throw new ReglaDeNegocioException("El correo electrónico es obligatorio.");

        if (!await catalogo.ExisteProgramaAsync(request.IdPrograma, ct))
            throw new ReglaDeNegocioException("El programa seleccionado no existe.");

        if (!await catalogo.ExisteTipoIdentificacionAsync(request.IdTipoIdentificacion, ct))
            throw new ReglaDeNegocioException("El tipo de identificación seleccionado no existe.");

        var correo = Usuario.NormalizarCorreo(request.CorreoElectronico);
        if (await usuarios.ExisteCorreoAsync(correo, ct))
            throw new ReglaDeNegocioException("Ya existe una cuenta registrada con este correo.");

        if (!string.IsNullOrWhiteSpace(request.NumeroIdentificacion) &&
            await estudiantes.ExisteIdentificacionAsync(
                request.IdTipoIdentificacion, request.NumeroIdentificacion.Trim(), ct: ct))
            throw new ReglaDeNegocioException("Ya existe un estudiante registrado con este documento.");

        var idRol = await usuarios.ObtenerIdRolAsync(Roles.Estudiante, ct);

        // 2. Dos guardados porque el Estudiante necesita el IdUsuario que
        //    genera la base. Si algo falla antes de Confirmar, la
        //    transacción se deshace al salir del using.
        await using var transaccion = await unidadDeTrabajo.IniciarTransaccionAsync(ct);

        var usuario = new Usuario(request.Nombre, correo, hasher.Hashear(request.Contrasena), idRol);
        usuarios.Agregar(usuario);
        await unidadDeTrabajo.GuardarCambiosAsync(ct);

        var estudiante = new Estudiante(request.Nombre, request.IdTipoIdentificacion,
                                        request.NumeroIdentificacion, request.IdPrograma, usuario.IdUsuario);
        estudiantes.Agregar(estudiante);
        await unidadDeTrabajo.GuardarCambiosAsync(ct);

        await transaccion.ConfirmarAsync(ct);
        return estudiante.IdEstudiante;
    }
}
