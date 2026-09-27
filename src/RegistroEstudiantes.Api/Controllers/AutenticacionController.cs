using Microsoft.AspNetCore.Mvc;
using RegistroEstudiantes.Application.Autenticacion;
using RegistroEstudiantes.Application.Estudiantes;

namespace RegistroEstudiantes.Api.Controllers;

public record RegistroCreadoResponse(int IdEstudiante);

[ApiController]
[Route("api/autenticacion")]
public class AutenticacionController : ControllerBase
{
    [HttpPost("registro")]
    [ProducesResponseType<RegistroCreadoResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Registrar(
        RegistrarEstudianteRequest request,
        [FromServices] RegistrarEstudiante registrarEstudiante,
        CancellationToken ct)
    {
        var idEstudiante = await registrarEstudiante.EjecutarAsync(request, ct);
        return Created("/api/mi-registro", new RegistroCreadoResponse(idEstudiante));
    }

    [HttpPost("login")]
    [ProducesResponseType<IniciarSesionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IniciarSesionResponse>> IniciarSesion(
        IniciarSesionRequest request,
        [FromServices] IniciarSesion iniciarSesion,
        CancellationToken ct)
        => Ok(await iniciarSesion.EjecutarAsync(request, ct));
}
