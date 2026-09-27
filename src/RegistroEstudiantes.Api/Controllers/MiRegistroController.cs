using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegistroEstudiantes.Api.Configuracion;
using RegistroEstudiantes.Application.Consultas;
using RegistroEstudiantes.Application.Estudiantes;
using RegistroEstudiantes.Domain.Usuarios;

namespace RegistroEstudiantes.Api.Controllers;

public record InscribirMateriaRequest(int IdMateria);

/// <summary>
/// Todo lo que un estudiante hace sobre SU propio registro. El Id del
/// estudiante sale siempre del token (User.ObtenerIdEstudiante()).
/// </summary>
[ApiController]
[Route("api/mi-registro")]
[Authorize(Roles = Roles.Estudiante)]
public class MiRegistroController : ControllerBase
{
    /// <summary>Datos, materias inscritas y compañeros de cada clase ("R" del CRUD).</summary>
    [HttpGet]
    [ProducesResponseType<MiRegistroDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<MiRegistroDto>> Obtener(
        [FromServices] IConsultasRegistro consultas, CancellationToken ct)
    {
        var registro = await consultas.ObtenerMiRegistroAsync(User.ObtenerIdEstudiante(), ct);
        return registro is null ? NotFound() : Ok(registro);
    }

    /// <summary>Actualiza nombre y documento ("U" del CRUD).</summary>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Actualizar(
        ActualizarEstudianteRequest request,
        [FromServices] ActualizarEstudiante actualizarEstudiante,
        CancellationToken ct)
    {
        await actualizarEstudiante.EjecutarAsync(User.ObtenerIdEstudiante(), request, ct);
        return NoContent();
    }

    /// <summary>Elimina el registro y la cuenta ("D" del CRUD).</summary>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Eliminar(
        [FromServices] EliminarEstudiante eliminarEstudiante, CancellationToken ct)
    {
        await eliminarEstudiante.EjecutarAsync(User.ObtenerIdEstudiante(), ct);
        return NoContent();
    }

    /// <summary>Materias del plan de estudios del estudiante.</summary>
    [HttpGet("materias-disponibles")]
    [ProducesResponseType<IReadOnlyList<MateriaDisponibleDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<MateriaDisponibleDto>>> ListarMateriasDisponibles(
        [FromServices] IConsultasRegistro consultas, CancellationToken ct)
        => Ok(await consultas.ListarMateriasDisponiblesAsync(User.ObtenerIdEstudiante(), ct));

    [HttpPost("inscripciones")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> InscribirMateria(
        InscribirMateriaRequest request,
        [FromServices] InscribirMateria inscribirMateria,
        CancellationToken ct)
    {
        await inscribirMateria.EjecutarAsync(User.ObtenerIdEstudiante(), request.IdMateria, ct);
        return NoContent();
    }

    [HttpDelete("inscripciones/{idMateria:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelarInscripcion(
        int idMateria,
        [FromServices] CancelarInscripcion cancelarInscripcion,
        CancellationToken ct)
    {
        await cancelarInscripcion.EjecutarAsync(User.ObtenerIdEstudiante(), idMateria, ct);
        return NoContent();
    }
}
