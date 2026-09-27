namespace RegistroEstudiantes.Infrastructure.Seguridad;

public class JwtOpciones
{
    public const string Seccion = "Jwt";

    public string Emisor { get; set; } = null!;
    public string Audiencia { get; set; } = null!;
    // En producción, desde variables de entorno o un gestor de secretos.
    public string Clave { get; set; } = null!;
    public int MinutosExpiracion { get; set; } = 60;
}
