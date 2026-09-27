using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RegistroEstudiantes.Application.Abstractions;
using RegistroEstudiantes.Domain.Common;

namespace RegistroEstudiantes.Infrastructure.Persistencia;

public class UnidadDeTrabajo(RegistroEstudiantesDbContext contexto) : IUnidadDeTrabajo
{
    private const int ViolacionUnique = 2627;
    private const int ViolacionIndiceUnico = 2601;

    public async Task GuardarCambiosAsync(CancellationToken ct = default)
    {
        try
        {
            await contexto.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sql &&
                                           sql.Number is ViolacionUnique or ViolacionIndiceUnico)
        {
            // Peticiones simultáneas que el dominio no alcanzó a ver: la base las rechaza por UNIQUE.
            throw new ReglaDeNegocioException(TraducirViolacionUnique(sql.Message));
        }
    }

    public async Task<ITransaccion> IniciarTransaccionAsync(CancellationToken ct = default)
        => new TransaccionEf(await contexto.Database.BeginTransactionAsync(ct));

    private static string TraducirViolacionUnique(string mensajeSql) => mensajeSql switch
    {
        _ when mensajeSql.Contains("UQ_Inscripcion_Estudiante_Profesor")
            => "Ya tienes una materia con este profesor. Debes elegir materias de profesores diferentes.",
        _ when mensajeSql.Contains("UQ_Usuario_CorreoElectronico")
            => "Ya existe una cuenta registrada con este correo.",
        _ when mensajeSql.Contains("UQ_Estudiante_Identificacion")
            => "Ya existe un estudiante registrado con este documento.",
        _ => "Ya existe un registro con esos datos."
    };

    private sealed class TransaccionEf(IDbContextTransaction transaccion) : ITransaccion
    {
        public Task ConfirmarAsync(CancellationToken ct = default) => transaccion.CommitAsync(ct);

        public ValueTask DisposeAsync() => transaccion.DisposeAsync();
    }
}
