using RegistroEstudiantes.Domain.Usuarios;

namespace RegistroEstudiantes.Application.Abstractions;

/// <summary>Puerto de salida para las cuentas de usuario.</summary>
public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorIdAsync(int idUsuario, CancellationToken ct = default);
    Task<Usuario?> ObtenerPorCorreoAsync(string correoNormalizado, CancellationToken ct = default);
    Task<bool> ExisteCorreoAsync(string correoNormalizado, CancellationToken ct = default);

    Task<int> ObtenerIdRolAsync(string nombreRol, CancellationToken ct = default);
    Task<string> ObtenerNombreRolAsync(int idRol, CancellationToken ct = default);

    void Agregar(Usuario usuario);
    void Eliminar(Usuario usuario);
}
