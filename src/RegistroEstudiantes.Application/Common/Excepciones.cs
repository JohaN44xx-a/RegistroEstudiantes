namespace RegistroEstudiantes.Application.Common;

/// <summary>
/// Lo que se buscó no existe. La API la traduce a un 404.
/// </summary>
public class NoEncontradoException : Exception
{
    public NoEncontradoException(string mensaje) : base(mensaje) { }
}

/// <summary>
/// Login fallido. El mensaje es el mismo si el correo no existe o si la
/// contraseña está mal: así un atacante no puede averiguar qué correos
/// están registrados. La API la traduce a un 401.
/// </summary>
public class CredencialesInvalidasException : Exception
{
    public CredencialesInvalidasException() : base("Correo o contraseña incorrectos.") { }
}
