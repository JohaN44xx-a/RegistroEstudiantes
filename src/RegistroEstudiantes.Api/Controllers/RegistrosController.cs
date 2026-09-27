using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegistroEstudiantes.Application.Consultas;

namespace RegistroEstudiantes.Api.Controllers;

/// <summary>
/// Regla 8: cualquier usuario con sesión puede ver los registros de los
/// demás estudiantes (solo nombre, programa y materias).
/// </summary>
[ApiController]
[Route("api/registros")]
[Authorize]
public class RegistrosController(IConsultasRegistro consultas) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RegistroPublicoDto>>> Listar(CancellationToken ct)
        => Ok(await consultas.ListarRegistrosAsync(ct));
}
