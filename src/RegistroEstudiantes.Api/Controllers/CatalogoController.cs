using Microsoft.AspNetCore.Mvc;
using RegistroEstudiantes.Application.Consultas;

namespace RegistroEstudiantes.Api.Controllers;

/// <summary>
/// Datos públicos que necesita el formulario de registro antes de que
/// exista una sesión.
/// </summary>
[ApiController]
[Route("api/catalogo")]
public class CatalogoController(IConsultasRegistro consultas) : ControllerBase
{
    [HttpGet("programas")]
    public async Task<ActionResult<IReadOnlyList<CatalogoItemDto>>> ListarProgramas(CancellationToken ct)
        => Ok(await consultas.ListarProgramasAsync(ct));

    [HttpGet("tipos-identificacion")]
    public async Task<ActionResult<IReadOnlyList<CatalogoItemDto>>> ListarTiposIdentificacion(CancellationToken ct)
        => Ok(await consultas.ListarTiposIdentificacionAsync(ct));
}
