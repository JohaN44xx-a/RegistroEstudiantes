namespace RegistroEstudiantes.Application.Abstractions;

public record DatosToken(int IdUsuario, string Nombre, string CorreoElectronico, string Rol, int? IdEstudiante);

public record TokenGenerado(string Token, DateTime ExpiraEnUtc);

public interface IGeneradorToken
{
    TokenGenerado Generar(DatosToken datos);
}
