namespace RegistroEstudiantes.Application.Abstractions;

/// <summary>
/// Puerto para el hash de contraseñas. Application no sabe qué algoritmo
/// se usa; Infrastructure lo implementa (PBKDF2 con salt, vía ASP.NET).
/// </summary>
public interface IHasherContrasenas
{
    /// <summary>Registro: convierte la contraseña en hash para guardarla.</summary>
    string Hashear(string contrasena);

    /// <summary>Login: ¿esta contraseña corresponde a este hash?</summary>
    bool Verificar(string contrasenaHash, string contrasena);
}
