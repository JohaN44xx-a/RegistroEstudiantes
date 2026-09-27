using RegistroEstudiantes.Application.Abstractions;
using RegistroEstudiantes.Application.Common;
using RegistroEstudiantes.Domain.Common;

namespace RegistroEstudiantes.Application.Estudiantes;

/// <summary>
/// Orquesta la inscripción: consigue los datos, le pide al agregado que
/// decida y guarda. Las reglas NO están aquí, están en Estudiante.
/// </summary>
public class InscribirMateria(
    IEstudianteRepository estudiantes,
    ICatalogoRepository catalogo,
    IUnidadDeTrabajo unidadDeTrabajo)
{
    public async Task EjecutarAsync(int idEstudiante, int idMateria, CancellationToken ct = default)
    {
        var estudiante = await estudiantes.ObtenerPorIdAsync(idEstudiante, ct)
            ?? throw new NoEncontradoException("El estudiante no existe.");

        // La regla del plan de estudios se valida aquí y no en el agregado
        // porque necesita datos que el estudiante no tiene (el plan vive
        // en el catálogo). Si la materia no es de su plan, no se encuentra.
        var materia = await catalogo.ObtenerMateriaDelPlanAsync(idMateria, estudiante.IdPrograma, ct)
            ?? throw new ReglaDeNegocioException("La materia no existe o no pertenece al plan de estudios de tu programa.");

        // Máximo 3 y profesor no repetido: los decide el dominio.
        estudiante.InscribirMateria(materia.IdMateria, materia.IdProfesor);

        await unidadDeTrabajo.GuardarCambiosAsync(ct);
    }
}
