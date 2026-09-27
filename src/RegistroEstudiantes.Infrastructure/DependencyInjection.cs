using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RegistroEstudiantes.Application.Abstractions;
using RegistroEstudiantes.Application.Consultas;
using RegistroEstudiantes.Infrastructure.Consultas;
using RegistroEstudiantes.Infrastructure.Persistencia;
using RegistroEstudiantes.Infrastructure.Persistencia.Repositorios;
using RegistroEstudiantes.Infrastructure.Seguridad;

namespace RegistroEstudiantes.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var cadenaConexion = configuration.GetConnectionString("RegistroEstudiantes")
            ?? throw new InvalidOperationException(
                "Falta la cadena de conexión 'RegistroEstudiantes' en la configuración.");

        services.AddDbContext<RegistroEstudiantesDbContext>(o => o.UseSqlServer(cadenaConexion));

        services.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();
        services.AddScoped<IEstudianteRepository, EstudianteRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ICatalogoRepository, CatalogoRepository>();
        services.AddScoped<IConsultasRegistro, ConsultasRegistro>();

        services.AddSingleton<IHasherContrasenas, HasherContrasenas>();
        services.AddSingleton<IGeneradorToken, GeneradorTokenJwt>();

        // La API no arranca si la configuración del JWT es inválida.
        services.AddOptions<JwtOpciones>()
                .Bind(configuration.GetSection(JwtOpciones.Seccion))
                .Validate(o => !string.IsNullOrWhiteSpace(o.Emisor) && !string.IsNullOrWhiteSpace(o.Audiencia),
                          "Jwt:Emisor y Jwt:Audiencia son obligatorios.")
                .Validate(o => o.Clave is { Length: >= 32 },
                          "Jwt:Clave debe tener al menos 32 caracteres.")
                .ValidateOnStart();

        return services;
    }
}
