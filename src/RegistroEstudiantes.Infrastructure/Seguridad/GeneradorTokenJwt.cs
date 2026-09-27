using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using RegistroEstudiantes.Application.Abstractions;
using RegistroEstudiantes.Application.Common;

namespace RegistroEstudiantes.Infrastructure.Seguridad;

/// <summary>
/// Adaptador del puerto IGeneradorToken. Firma el token con HMAC-SHA256
/// usando la clave secreta: quien no tenga la clave no puede fabricar ni
/// modificar un token válido.
/// </summary>
public class GeneradorTokenJwt(IOptions<JwtOpciones> opciones) : IGeneradorToken
{
    private readonly JwtOpciones _opciones = opciones.Value;
    private readonly JsonWebTokenHandler _handler = new();

    public TokenGenerado Generar(DatosToken datos)
    {
        var expira = DateTime.UtcNow.AddMinutes(_opciones.MinutosExpiracion);

        var claims = new List<Claim>
        {
            new(NombresClaims.IdUsuario, datos.IdUsuario.ToString()),
            new(NombresClaims.Nombre, datos.Nombre),
            new(NombresClaims.Correo, datos.CorreoElectronico),
            new(NombresClaims.Rol, datos.Rol)
        };

        if (datos.IdEstudiante is not null)
            claims.Add(new Claim(NombresClaims.IdEstudiante, datos.IdEstudiante.Value.ToString()));

        var credenciales = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opciones.Clave)),
            SecurityAlgorithms.HmacSha256);

        var token = _handler.CreateToken(new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _opciones.Emisor,
            Audience = _opciones.Audiencia,
            Expires = expira,
            SigningCredentials = credenciales
        });

        return new TokenGenerado(token, expira);
    }
}
