namespace RegistroEstudiantes.Application.Abstractions;

public interface IHasherContrasenas
{
    string Hashear(string contrasena);

    bool Verificar(string contrasenaHash, string contrasena);
}
