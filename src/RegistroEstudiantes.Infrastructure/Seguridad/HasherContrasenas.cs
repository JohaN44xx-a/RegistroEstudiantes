using Microsoft.AspNetCore.Identity;
using RegistroEstudiantes.Application.Abstractions;

namespace RegistroEstudiantes.Infrastructure.Seguridad;

/// <summary>
/// Adaptador del puerto IHasherContrasenas usando el PasswordHasher de
/// ASP.NET Core Identity: PBKDF2 con salt aleatorio y muchas iteraciones.
/// El salt queda guardado dentro del mismo texto del hash.
/// </summary>
public class HasherContrasenas : IHasherContrasenas
{
    // PasswordHasher pide un "usuario" genérico que su implementación no
    // usa; se le pasa un objeto vacío para no acoplarlo al dominio.
    private static readonly object SinUsuario = new();
    private readonly PasswordHasher<object> _hasher = new();

    public string Hashear(string contrasena)
        => _hasher.HashPassword(SinUsuario, contrasena);

    public bool Verificar(string contrasenaHash, string contrasena)
        => _hasher.VerifyHashedPassword(SinUsuario, contrasenaHash, contrasena)
           != PasswordVerificationResult.Failed;
}
