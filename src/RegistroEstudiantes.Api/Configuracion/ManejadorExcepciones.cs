using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RegistroEstudiantes.Application.Common;
using RegistroEstudiantes.Domain.Common;

namespace RegistroEstudiantes.Api.Configuracion;

public sealed class ManejadorExcepciones(
    IProblemDetailsService problemDetails,
    ILogger<ManejadorExcepciones> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext contexto, Exception excepcion, CancellationToken ct)
    {
        var (estado, titulo) = excepcion switch
        {
            ReglaDeNegocioException => (StatusCodes.Status400BadRequest, "Regla de negocio"),
            NoEncontradoException => (StatusCodes.Status404NotFound, "No encontrado"),
            CredencialesInvalidasException => (StatusCodes.Status401Unauthorized, "Credenciales inválidas"),
            UnauthorizedAccessException => (StatusCodes.Status403Forbidden, "Acceso denegado"),
            _ => (StatusCodes.Status500InternalServerError, "Error interno")
        };

        // Los errores inesperados se registran completos, pero al cliente no se le exponen detalles.
        if (estado == StatusCodes.Status500InternalServerError)
            logger.LogError(excepcion, "Error no controlado");

        contexto.Response.StatusCode = estado;

        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = contexto,
            Exception = excepcion,
            ProblemDetails = new ProblemDetails
            {
                Status = estado,
                Title = titulo,
                Detail = estado == StatusCodes.Status500InternalServerError
                    ? "Ocurrió un error inesperado. Intenta de nuevo más tarde."
                    : excepcion.Message
            }
        });
    }
}
