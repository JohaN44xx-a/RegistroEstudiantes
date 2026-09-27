namespace RegistroEstudiantes.Infrastructure.Seguridad;

/// <summary>
/// Se llena desde la sección "Jwt" de la configuración. La clave NUNCA
/// va en el código: en desarrollo, user-secrets; en producción,
/// variables de entorno o un gestor de secretos.
/// </summary>
public class JwtOpciones
{
    public const string Seccion = "Jwt";

    public string Emisor { get; set; } = null!;
    public string Audiencia { get; set; } = null!;
    public string Clave { get; set; } = null!;
    public int MinutosExpiracion { get; set; } = 60;
}
