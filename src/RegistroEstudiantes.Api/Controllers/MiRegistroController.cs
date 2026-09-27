using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegistroEstudiantes.Api.Configuracion;
using RegistroEstudiantes.Application.Consultas;
using RegistroEstudiantes.Application.Estudiantes;
using RegistroEstudiantes.Domain.Usuarios;

namespace RegistroEstudiantes.Api.Controllers;

public record InscribirMateriaRequest(int IdMateria);

[ApiController]
[Route("api/mi-registro")]
[Authorize(Roles = Roles.Estudiante)]
public class MiRegistroController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<MiRegistroDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<MiRegistroDto>> Obtener(
        [FromServices] IConsultasRegistro consultas, CancellationToken ct)
    {
        var registro = await consultas.ObtenerMiRegistroAsync(User.ObtenerIdEstudiante(), ct);
        return registro is null ? NotFound() : Ok(registro);
    }

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

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Eliminar(
        [FromServices] EliminarEstudiante eliminarEstudiante, CancellationToken ct)
    {
        await eliminarEstudiante.EjecutarAsync(User.ObtenerIdEstudiante(), ct);
        return NoContent();
    }

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
