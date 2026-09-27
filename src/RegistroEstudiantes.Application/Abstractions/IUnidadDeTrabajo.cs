namespace RegistroEstudiantes.Application.Abstractions;

/// <summary>
/// Puerto para confirmar cambios. Los repositorios solo "anotan" lo que
/// hay que hacer (Agregar, Eliminar); nada llega a la base hasta que
/// alguien llama a GuardarCambiosAsync. Así un caso de uso decide cuándo
/// se confirma todo junto.
/// </summary>
public interface IUnidadDeTrabajo
{
    Task GuardarCambiosAsync(CancellationToken ct = default);

    /// <summary>
    /// Para casos de uso que necesitan guardar en dos pasos y que ambos
    /// se confirmen o se deshagan juntos (por ejemplo, el registro).
    /// </summary>
    Task<ITransaccion> IniciarTransaccionAsync(CancellationToken ct = default);
}

/// <summary>
/// Si se libera (Dispose) sin haber llamado a ConfirmarAsync, se deshace.
/// </summary>
public interface ITransaccion : IAsyncDisposable
{
    Task ConfirmarAsync(CancellationToken ct = default);
}
