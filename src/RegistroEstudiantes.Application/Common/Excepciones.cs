namespace RegistroEstudiantes.Application.Common;

public class NoEncontradoException : Exception
{
    public NoEncontradoException(string mensaje) : base(mensaje) { }
}

public class CredencialesInvalidasException : Exception
{
    public CredencialesInvalidasException() : base("Correo o contraseña incorrectos.") { }
}
