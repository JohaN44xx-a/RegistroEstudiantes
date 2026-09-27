namespace RegistroEstudiantes.Domain.Common;

public class ReglaDeNegocioException : Exception
{
    public ReglaDeNegocioException(string mensaje) : base(mensaje) { }
}