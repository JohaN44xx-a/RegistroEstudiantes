using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegistroEstudiantes.Application.Consultas;

namespace RegistroEstudiantes.Api.Controllers;

[ApiController]
[Route("api/registros")]
[Authorize]
public class RegistrosController(IConsultasRegistro consultas) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RegistroPublicoDto>>> Listar(CancellationToken ct)
        => Ok(await consultas.ListarRegistrosAsync(ct));
}
