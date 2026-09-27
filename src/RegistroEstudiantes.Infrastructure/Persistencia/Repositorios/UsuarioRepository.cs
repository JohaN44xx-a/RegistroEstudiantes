using Microsoft.EntityFrameworkCore;
using RegistroEstudiantes.Application.Abstractions;
using RegistroEstudiantes.Domain.Usuarios;

namespace RegistroEstudiantes.Infrastructure.Persistencia.Repositorios;

public class UsuarioRepository(RegistroEstudiantesDbContext contexto) : IUsuarioRepository
{
    public Task<Usuario?> ObtenerPorIdAsync(int idUsuario, CancellationToken ct = default)
        => contexto.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == idUsuario, ct);

    public Task<Usuario?> ObtenerPorCorreoAsync(string correoNormalizado, CancellationToken ct = default)
        => contexto.Usuarios.FirstOrDefaultAsync(u => u.CorreoElectronico == correoNormalizado, ct);

    public Task<bool> ExisteCorreoAsync(string correoNormalizado, CancellationToken ct = default)
        => contexto.Usuarios.AnyAsync(u => u.CorreoElectronico == correoNormalizado, ct);

    // First y no FirstOrDefault: si falta un rol, es un error de configuración.
    public Task<int> ObtenerIdRolAsync(string nombreRol, CancellationToken ct = default)
        => contexto.Roles.Where(r => r.Nombre == nombreRol).Select(r => r.IdRol).FirstAsync(ct);

    public Task<string> ObtenerNombreRolAsync(int idRol, CancellationToken ct = default)
        => contexto.Roles.Where(r => r.IdRol == idRol).Select(r => r.Nombre).FirstAsync(ct);

    public void Agregar(Usuario usuario) => contexto.Usuarios.Add(usuario);

    public void Eliminar(Usuario usuario) => contexto.Usuarios.Remove(usuario);
}
