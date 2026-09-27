using System.Security.Claims;
using RegistroEstudiantes.Application.Common;

namespace RegistroEstudiantes.Api.Configuracion;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// El Id del estudiante sale del token, NUNCA de la URL ni del body.
    /// Así un estudiante no puede modificar el registro de otro cambiando
    /// un número en la petición.
    /// </summary>
    public static int ObtenerIdEstudiante(this ClaimsPrincipal usuario)
    {
        var valor = usuario.FindFirst(NombresClaims.IdEstudiante)?.Value;

        return int.TryParse(valor, out var idEstudiante)
            ? idEstudiante
            : throw new UnauthorizedAccessException("La sesión actual no corresponde a un estudiante.");
    }
}
