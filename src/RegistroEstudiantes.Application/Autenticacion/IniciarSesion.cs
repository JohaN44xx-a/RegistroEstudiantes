using RegistroEstudiantes.Application.Abstractions;
using RegistroEstudiantes.Application.Common;
using RegistroEstudiantes.Domain.Usuarios;

namespace RegistroEstudiantes.Application.Autenticacion;

public record IniciarSesionRequest(string CorreoElectronico, string Contrasena);

public record IniciarSesionResponse(string Token, DateTime ExpiraEnUtc, string Nombre, string Rol);

public class IniciarSesion(IUsuarioRepository usuarios, IEstudianteRepository estudiantes, IHasherContrasenas hasher, IGeneradorToken generadorToken)
{
    public async Task<IniciarSesionResponse> EjecutarAsync(IniciarSesionRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.CorreoElectronico) || string.IsNullOrWhiteSpace(request.Contrasena))
        {
            throw new CredencialesInvalidasException();
        }
        var correo = Usuario.NormalizarCorreo(request.CorreoElectronico);
        var usuario = await usuarios.ObtenerPorCorreoAsync(correo, ct);

        // Mismo error si no existe el correo o si la contraseña no coincide.
        if (usuario is null || !hasher.Verificar(usuario.ContrasenaHash, request.Contrasena))
        {
            throw new CredencialesInvalidasException();
        }

        var rol = await usuarios.ObtenerNombreRolAsync(usuario.IdRol, ct);
        var idEstudiante = await estudiantes.ObtenerIdPorUsuarioAsync(usuario.IdUsuario, ct);

        var token = generadorToken.Generar(new DatosToken(
            usuario.IdUsuario, usuario.Nombre, usuario.CorreoElectronico, rol, idEstudiante));

        return new IniciarSesionResponse(token.Token, token.ExpiraEnUtc, usuario.Nombre, rol);
    }
}
