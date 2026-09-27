namespace RegistroEstudiantes.Application.Abstractions;

/// <summary>Datos que viajan dentro del JWT (firmados, NO encriptados).</summary>
public record DatosToken(int IdUsuario, string Nombre, string CorreoElectronico, string Rol, int? IdEstudiante);

public record TokenGenerado(string Token, DateTime ExpiraEnUtc);

/// <summary>Puerto para emitir el JWT. Infrastructure lo implementa.</summary>
public interface IGeneradorToken
{
    TokenGenerado Generar(DatosToken datos);
}
