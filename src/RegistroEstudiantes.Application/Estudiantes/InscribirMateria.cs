using RegistroEstudiantes.Application.Abstractions;
using RegistroEstudiantes.Application.Common;
using RegistroEstudiantes.Domain.Common;

namespace RegistroEstudiantes.Application.Estudiantes;

public class InscribirMateria(
    IEstudianteRepository estudiantes,
    ICatalogoRepository catalogo,
    IUnidadDeTrabajo unidadDeTrabajo)
{
    public async Task EjecutarAsync(int idEstudiante, int idMateria, CancellationToken ct = default)
    {
        var estudiante = await estudiantes.ObtenerPorIdAsync(idEstudiante, ct)
            ?? throw new NoEncontradoException("El estudiante no existe.");

        // El plan de estudios vive en el catálogo, fuera del agregado; por eso esta regla se valida aquí.
        var materia = await catalogo.ObtenerMateriaDelPlanAsync(idMateria, estudiante.IdPrograma, ct)
            ?? throw new ReglaDeNegocioException(
                "La materia no existe o no pertenece al plan de estudios de tu programa.");

        estudiante.InscribirMateria(materia.IdMateria, materia.IdProfesor);

        await unidadDeTrabajo.GuardarCambiosAsync(ct);
    }
}
