using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegistroEstudiantes.Application.Estudiantes;
using RegistroEstudiantes.Domain.Usuarios;

namespace RegistroEstudiantes.Api.Controllers;

/// <summary>Operaciones de administración sobre cualquier estudiante.</summary>
[ApiController]
[Route("api/estudiantes")]
[Authorize(Roles = Roles.Administrador)]
public class EstudiantesController : ControllerBase
{
    [HttpDelete("{idEstudiante:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(
        int idEstudiante,
        [FromServices] EliminarEstudiante eliminarEstudiante,
        CancellationToken ct)
    {
        await eliminarEstudiante.EjecutarAsync(idEstudiante, ct);
        return NoContent();
    }
}
