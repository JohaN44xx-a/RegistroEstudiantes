using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RegistroEstudiantes.Api.Configuracion;
using RegistroEstudiantes.Application;
using RegistroEstudiantes.Application.Common;
using RegistroEstudiantes.Infrastructure;
using RegistroEstudiantes.Infrastructure.Seguridad;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ---------- Capas (composition root: el único lugar que conoce todo) ----------
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ---------- API ----------
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ManejadorExcepciones>();

// ---------- Autenticación JWT ----------
var jwt = builder.Configuration.GetSection(JwtOpciones.Seccion).Get<JwtOpciones>();
if (jwt is null || string.IsNullOrWhiteSpace(jwt.Clave))
    throw new InvalidOperationException("Falta la configuración 'Jwt' (Emisor, Audiencia y Clave).");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opciones =>
    {
        // Conserva los nombres cortos de los claims ("role", "sub"...).
        opciones.MapInboundClaims = false;

        opciones.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Emisor,
            ValidateAudience = true,
            ValidAudience = jwt.Audiencia,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Clave)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            NameClaimType = NombresClaims.Nombre,
            RoleClaimType = NombresClaims.Rol
        };
    });

builder.Services.AddAuthorization();

// ---------- CORS para el frontend de Angular ----------
const string PoliticaFrontend = "Frontend";
var origenesPermitidos = builder.Configuration.GetSection("Cors:OrigenesPermitidos").Get<string[]>() ?? [];

builder.Services.AddCors(opciones => opciones.AddPolicy(PoliticaFrontend, politica =>
    politica.WithOrigins(origenesPermitidos)
            .AllowAnyHeader()
            .AllowAnyMethod()));

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();              // /openapi/v1.json
    app.MapScalarApiReference();   // /scalar/v1 (documentación interactiva)
    await DatosIniciales.SembrarAsync(app.Services);
}

app.UseHttpsRedirection();
app.UseCors(PoliticaFrontend);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
