using Microsoft.AspNetCore.Mvc;
using RegistroEstudiantes.Application.Autenticacion;
using RegistroEstudiantes.Application.Estudiantes;

namespace RegistroEstudiantes.Api.Controllers;

public record RegistroCreadoResponse(int IdEstudiante);

/// <summary>
/// Adaptador de entrada HTTP: traduce peticiones a casos de uso. No tiene
/// lógica de negocio ni try/catch (los errores los maneja ManejadorExcepciones).
/// </summary>
[ApiController]
[Route("api/autenticacion")]
public class AutenticacionController : ControllerBase
{
    /// <summary>Registro en línea de un estudiante (la "C" del CRUD).</summary>
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

    /// <summary>Devuelve un JWT si el correo y la contraseña son correctos.</summary>
    [HttpPost("login")]
    [ProducesResponseType<IniciarSesionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IniciarSesionResponse>> IniciarSesion(
        IniciarSesionRequest request,
        [FromServices] IniciarSesion iniciarSesion,
        CancellationToken ct)
        => Ok(await iniciarSesion.EjecutarAsync(request, ct));
}
