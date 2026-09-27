namespace RegistroEstudiantes.Application.Common;

/// <summary>
/// Nombres de los datos que viajan dentro del JWT. Los usa
/// Infrastructure para escribir el token y la Api para leerlo, así que
/// viven aquí, en un lugar que ambos conocen.
/// </summary>
public static class NombresClaims
{
    public const string IdUsuario = "sub";
    public const string Nombre = "name";
    public const string Correo = "email";
    public const string Rol = "role";
    public const string IdEstudiante = "id_estudiante";
}
