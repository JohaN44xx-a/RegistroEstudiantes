using Microsoft.EntityFrameworkCore;
using RegistroEstudiantes.Application.Consultas;
using RegistroEstudiantes.Domain.Estudiantes;
using RegistroEstudiantes.Infrastructure.Persistencia;

namespace RegistroEstudiantes.Infrastructure.Consultas;

public class ConsultasRegistro(RegistroEstudiantesDbContext contexto) : IConsultasRegistro
{
    public async Task<IReadOnlyList<CatalogoItemDto>> ListarProgramasAsync(CancellationToken ct = default)
        => await contexto.Programas.AsNoTracking()
                         .OrderBy(p => p.Nombre)
                         .Select(p => new CatalogoItemDto(p.IdPrograma, p.Nombre))
                         .ToListAsync(ct);

    public async Task<IReadOnlyList<CatalogoItemDto>> ListarTiposIdentificacionAsync(CancellationToken ct = default)
        => await contexto.TiposIdentificacion.AsNoTracking()
                         .OrderBy(t => t.IdTipoIdentificacion)
                         .Select(t => new CatalogoItemDto(t.IdTipoIdentificacion, t.Nombre))
                         .ToListAsync(ct);

    public async Task<IReadOnlyList<MateriaDisponibleDto>> ListarMateriasDisponiblesAsync(int idEstudiante, CancellationToken ct = default)
    {
        var idPrograma = await contexto.Estudiantes.AsNoTracking()
                                       .Where(e => e.IdEstudiante == idEstudiante)
                                       .Select(e => (int?)e.IdPrograma)
                                       .FirstOrDefaultAsync(ct);

        if (idPrograma is null)
            return [];

        var idsInscritas = await contexto.Set<Inscripcion>().AsNoTracking()
                                         .Where(i => i.IdEstudiante == idEstudiante)
                                         .Select(i => i.IdMateria)
                                         .ToListAsync(ct);

        return await (from pe in contexto.PlanesEstudios.AsNoTracking()
                      join m in contexto.Materias on pe.IdMateria equals m.IdMateria
                      join p in contexto.Profesores on m.IdProfesor equals p.IdProfesor
                      where pe.IdPrograma == idPrograma
                      orderby p.Nombre, m.Nombre
                      select new MateriaDisponibleDto(m.IdMateria, m.Nombre, m.Creditos, p.IdProfesor, p.Nombre,
                                                      idsInscritas.Contains(m.IdMateria)))
                     .ToListAsync(ct);
    }

    public async Task<MiRegistroDto?> ObtenerMiRegistroAsync(int idEstudiante, CancellationToken ct = default)
    {
        var datos = await (from e in contexto.Estudiantes.AsNoTracking()
                           join t in contexto.TiposIdentificacion on e.IdTipoIdentificacion equals t.IdTipoIdentificacion
                           join pr in contexto.Programas on e.IdPrograma equals pr.IdPrograma
                           join u in contexto.Usuarios on e.IdUsuario equals u.IdUsuario
                           where e.IdEstudiante == idEstudiante
                           select new
                           {
                               e.IdEstudiante,
                               e.Nombre,
                               e.IdTipoIdentificacion,
                               TipoIdentificacion = t.Nombre,
                               e.NumeroIdentificacion,
                               Programa = pr.Nombre,
                               u.CorreoElectronico
                           })
                          .FirstOrDefaultAsync(ct);

        if (datos is null)
            return null;

        var materias = await (from i in contexto.Set<Inscripcion>().AsNoTracking()
                              join m in contexto.Materias on i.IdMateria equals m.IdMateria
                              join p in contexto.Profesores on m.IdProfesor equals p.IdProfesor
                              where i.IdEstudiante == idEstudiante
                              orderby m.Nombre
                              select new { m.IdMateria, m.Nombre, m.Creditos, Profesor = p.Nombre })
                             .ToListAsync(ct);

        var idsMaterias = materias.Select(m => m.IdMateria).ToList();

        // Regla 9: de los compañeros solo se proyecta el nombre.
        var companeros = await (from i in contexto.Set<Inscripcion>().AsNoTracking()
                                join e in contexto.Estudiantes on i.IdEstudiante equals e.IdEstudiante
                                where idsMaterias.Contains(i.IdMateria) && i.IdEstudiante != idEstudiante
                                orderby e.Nombre
                                select new { i.IdMateria, e.Nombre })
                               .ToListAsync(ct);

        var materiasDto = materias
            .Select(m => new MateriaInscritaDto(
                m.IdMateria, m.Nombre, m.Creditos, m.Profesor,
                companeros.Where(c => c.IdMateria == m.IdMateria).Select(c => c.Nombre).ToList()))
            .ToList();

        return new MiRegistroDto(datos.IdEstudiante, datos.Nombre, datos.IdTipoIdentificacion,
                                 datos.TipoIdentificacion, datos.NumeroIdentificacion, datos.Programa,
                                 datos.CorreoElectronico, materiasDto.Sum(m => m.Creditos), materiasDto);
    }

    public async Task<IReadOnlyList<RegistroPublicoDto>> ListarRegistrosAsync(CancellationToken ct = default)
    {
        var estudiantes = await (from e in contexto.Estudiantes.AsNoTracking()
                                 join pr in contexto.Programas on e.IdPrograma equals pr.IdPrograma
                                 orderby e.Nombre
                                 select new { e.IdEstudiante, e.Nombre, Programa = pr.Nombre })
                                .ToListAsync(ct);

        var inscripciones = await (from i in contexto.Set<Inscripcion>().AsNoTracking()
                                   join m in contexto.Materias on i.IdMateria equals m.IdMateria
                                   orderby m.Nombre
                                   select new { i.IdEstudiante, Materia = m.Nombre })
                                  .ToListAsync(ct);

        return estudiantes
            .Select(e => new RegistroPublicoDto(
                e.Nombre, e.Programa,
                inscripciones.Where(i => i.IdEstudiante == e.IdEstudiante).Select(i => i.Materia).ToList()))
            .ToList();
    }
}
