using System.Security.Claims;
using RegistroEstudiantes.Application.Common;

namespace RegistroEstudiantes.Api.Configuracion;

public static class ClaimsPrincipalExtensions
{
    // El Id sale del token, nunca de la URL: un estudiante no puede operar sobre el registro de otro.
    public static int ObtenerIdEstudiante(this ClaimsPrincipal usuario)
    {
        var valor = usuario.FindFirst(NombresClaims.IdEstudiante)?.Value;

        return int.TryParse(valor, out var idEstudiante)
            ? idEstudiante
            : throw new UnauthorizedAccessException("La sesión actual no corresponde a un estudiante.");
    }
}
