using Microsoft.Extensions.DependencyInjection;
using RegistroEstudiantes.Application.Autenticacion;
using RegistroEstudiantes.Application.Estudiantes;

namespace RegistroEstudiantes.Application;

/// <summary>
/// Registra los casos de uso. Program.cs solo llama a AddApplication().
/// Scoped: una instancia por petición HTTP.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IniciarSesion>();
        services.AddScoped<RegistrarEstudiante>();
        services.AddScoped<ActualizarEstudiante>();
        services.AddScoped<EliminarEstudiante>();
        services.AddScoped<InscribirMateria>();
        services.AddScoped<CancelarInscripcion>();

        return services;
    }
}
