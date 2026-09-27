using Microsoft.AspNetCore.Identity;
using RegistroEstudiantes.Application.Abstractions;

namespace RegistroEstudiantes.Infrastructure.Seguridad;

// PBKDF2 con salt aleatorio; el salt queda dentro del mismo hash.
public class HasherContrasenas : IHasherContrasenas
{
    // PasswordHasher no usa el usuario; se pasa un objeto vacío para no acoplarlo al dominio.
    private static readonly object SinUsuario = new();
    private readonly PasswordHasher<object> _hasher = new();

    public string Hashear(string contrasena)
        => _hasher.HashPassword(SinUsuario, contrasena);

    public bool Verificar(string contrasenaHash, string contrasena)
        => _hasher.VerifyHashedPassword(SinUsuario, contrasenaHash, contrasena)
           != PasswordVerificationResult.Failed;
}
