using RegistroEstudiantes.Application.Abstractions;
using RegistroEstudiantes.Application.Consultas;
using RegistroEstudiantes.Application.Estudiantes;
using RegistroEstudiantes.Domain.Usuarios;

namespace RegistroEstudiantes.Api.Configuracion;

/// <summary>
/// Crea el administrador y estudiantes de prueba SOLO en desarrollo.
///
/// - Los estudiantes se crean con los mismos casos de uso que usa la app
///   (RegistrarEstudiante e InscribirMateria), así que cumplen las mismas
///   reglas que cualquier registro real.
/// - Es idempotente: si el correo ya existe, no lo vuelve a crear.
/// - Los datos están pensados para demostrar las reglas (ver README).
/// </summary>
public static class DatosIniciales
{
    public const string CorreoAdministrador = "admin@registro.edu.co";
    public const string ContrasenaAdministrador = "Admin2026*";
    public const string ContrasenaEstudiantes = "Estudiante2026*";

    private const string CedulaCiudadania = "Cédula de ciudadanía";
    private const string Sistemas = "Ingeniería de Sistemas";
    private const string Administracion = "Administración de Empresas";

    private sealed record EstudianteDemo(string Nombre, string Correo, string Documento,
                                         string Programa, string[] Materias);

    private static readonly EstudianteDemo[] EstudiantesDemo =
    [
        // Cupo lleno: sirve para probar que no deja inscribir una cuarta.
        new("Ana Torres", "ana.torres@registro.edu.co", "1000000001", Sistemas,
            ["Cálculo Diferencial", "Programación I", "Bases de Datos"]),

        // Tiene Cálculo con Laura Gómez: sirve para probar que no deja
        // inscribir Álgebra Lineal (misma profesora).
        new("Juan Pérez", "juan.perez@registro.edu.co", "1000000002", Sistemas,
            ["Cálculo Diferencial", "Estructuras de Datos"]),

        // Comparte Cálculo y Bases de Datos con Ana: regla 9.
        new("María Rodríguez", "maria.rodriguez@registro.edu.co", "1000000003", Sistemas,
            ["Cálculo Diferencial", "Bases de Datos"]),

        // Otro programa, con una materia compartida con Sistemas.
        new("Pedro Gómez", "pedro.gomez@registro.edu.co", "1000000004", Administracion,
            ["Cálculo Diferencial", "Metodología de la Investigación"]),

        // Sin materias: para probar el flujo de inscripción desde cero.
        new("Camila Vargas", "camila.vargas@registro.edu.co", "1000000005", Administracion, [])
    ];

    public static async Task SembrarAsync(IServiceProvider servicios)
    {
        await using var alcance = servicios.CreateAsyncScope();
        var sp = alcance.ServiceProvider;
        var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(DatosIniciales));

        try
        {
            await SembrarAdministradorAsync(sp);
            await SembrarEstudiantesAsync(sp);
            logger.LogInformation("Datos iniciales de desarrollo listos.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "No se pudieron crear los datos iniciales. ¿Ejecutaste database/01_RegistroEstudiantes.sql " +
                "y la cadena de conexión apunta a tu SQL Server?");
            throw;
        }
    }

    private static async Task SembrarAdministradorAsync(IServiceProvider sp)
    {
        var usuarios = sp.GetRequiredService<IUsuarioRepository>();
        if (await usuarios.ExisteCorreoAsync(CorreoAdministrador))
            return;

        var hasher = sp.GetRequiredService<IHasherContrasenas>();
        var unidadDeTrabajo = sp.GetRequiredService<IUnidadDeTrabajo>();
        var idRol = await usuarios.ObtenerIdRolAsync(Roles.Administrador);

        usuarios.Agregar(new Usuario("Administrador", CorreoAdministrador,
                                     hasher.Hashear(ContrasenaAdministrador), idRol));
        await unidadDeTrabajo.GuardarCambiosAsync();
    }

    private static async Task SembrarEstudiantesAsync(IServiceProvider sp)
    {
        var usuarios = sp.GetRequiredService<IUsuarioRepository>();
        var consultas = sp.GetRequiredService<IConsultasRegistro>();
        var registrar = sp.GetRequiredService<RegistrarEstudiante>();
        var inscribir = sp.GetRequiredService<InscribirMateria>();

        var programas = await consultas.ListarProgramasAsync();
        var tipos = await consultas.ListarTiposIdentificacionAsync();
        var idCedula = tipos.First(t => t.Nombre == CedulaCiudadania).Id;

        foreach (var demo in EstudiantesDemo)
        {
            if (await usuarios.ExisteCorreoAsync(Usuario.NormalizarCorreo(demo.Correo)))
                continue;

            var idPrograma = programas.First(p => p.Nombre == demo.Programa).Id;

            var idEstudiante = await registrar.EjecutarAsync(new RegistrarEstudianteRequest(
                demo.Nombre, demo.Correo, ContrasenaEstudiantes, idCedula, demo.Documento, idPrograma));

            var disponibles = await consultas.ListarMateriasDisponiblesAsync(idEstudiante);
            foreach (var materia in demo.Materias)
                await inscribir.EjecutarAsync(idEstudiante, disponibles.First(m => m.Nombre == materia).IdMateria);
        }
    }
}
