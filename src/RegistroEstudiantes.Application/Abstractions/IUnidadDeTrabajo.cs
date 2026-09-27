namespace RegistroEstudiantes.Application.Abstractions;

public interface IUnidadDeTrabajo
{
    Task GuardarCambiosAsync(CancellationToken ct = default);

    Task<ITransaccion> IniciarTransaccionAsync(CancellationToken ct = default);
}

// Si se libera sin ConfirmarAsync, se hace rollback.
public interface ITransaccion : IAsyncDisposable
{
    Task ConfirmarAsync(CancellationToken ct = default);
}
