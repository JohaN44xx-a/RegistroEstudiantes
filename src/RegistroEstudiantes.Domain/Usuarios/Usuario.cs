using RegistroEstudiantes.Domain.Common;

namespace RegistroEstudiantes.Domain.Usuarios;

/// <summary>
/// Cuenta con la que alguien inicia sesión. Pertenece al contexto de
/// Identidad: no sabe nada de estudiantes ni de inscripciones.
/// </summary>
public class Usuario
{
    public int IdUsuario { get; private set; }
    public string Nombre { get; private set; } = null!;
    public string CorreoElectronico { get; private set; } = null!;
    public string ContrasenaHash { get; private set; } = null!;
    public int IdRol { get; private set; }

    private Usuario() { }

    public Usuario(string nombre, string correoElectronico, string contrasenaHash, int idRol)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ReglaDeNegocioException("El nombre es obligatorio.");

        if (string.IsNullOrWhiteSpace(correoElectronico))
            throw new ReglaDeNegocioException("El correo electrónico es obligatorio.");

        if (string.IsNullOrWhiteSpace(contrasenaHash))
            throw new ReglaDeNegocioException("La contraseña es obligatoria.");

        Nombre = nombre.Trim();
        CorreoElectronico = NormalizarCorreo(correoElectronico);
        ContrasenaHash = contrasenaHash;
        IdRol = idRol;
    }

    public void ActualizarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ReglaDeNegocioException("El nombre es obligatorio.");

        Nombre = nombre.Trim();
    }

    /// <summary>
    /// "Juan@Correo.com " y "juan@correo.com" son la misma cuenta.
    /// Se normaliza siempre igual al guardar y al buscar en el login.
    /// </summary>
    public static string NormalizarCorreo(string correoElectronico)
        => correoElectronico.Trim().ToLowerInvariant();
}
