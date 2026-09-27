import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed, toObservable, toSignal } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { catchError, filter, finalize, forkJoin, Observable, of, switchMap } from 'rxjs';
import { CatalogoService } from '../../core/api/catalogo.service';
import { MiRegistroService } from '../../core/api/mi-registro.service';
import { ActualizarEstudianteRequest } from '../../core/api/modelos';
import { AuthService } from '../../core/auth/auth.service';
import { ConfirmacionDialog, DatosConfirmacion } from '../../shared/confirmacion-dialog';
import { NotificacionService } from '../../shared/notificacion.service';
import { DatosEstudiante } from './datos-estudiante/datos-estudiante';
import { MateriasDisponibles } from './materias-disponibles/materias-disponibles';
import { MateriasInscritas } from './materias-inscritas/materias-inscritas';

@Component({
  selector: 'app-mi-registro',
  imports: [MatProgressBarModule, MatButtonModule, DatosEstudiante, MateriasInscritas, MateriasDisponibles],
  templateUrl: './mi-registro.html',
  styleUrl: './mi-registro.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MiRegistroPagina {
  private readonly api = inject(MiRegistroService);
  private readonly catalogo = inject(CatalogoService);
  private readonly auth = inject(AuthService);
  private readonly notificacion = inject(NotificacionService);
  private readonly dialogo = inject(MatDialog);
  private readonly destroyRef = inject(DestroyRef);

  private readonly recargar = signal(0);
  protected readonly procesando = signal(false);

  // switchMap descarta la carga anterior si llega una nueva. undefined = cargando, null = error.
  protected readonly datos = toSignal(
    toObservable(this.recargar).pipe(
      switchMap(() =>
        forkJoin({
          registro: this.api.obtener(),
          disponibles: this.api.materiasDisponibles(),
        }).pipe(
          catchError((error) => {
            this.notificacion.error(error);
            return of(null);
          }),
        ),
      ),
    ),
  );

  protected readonly tiposIdentificacion = toSignal(this.catalogo.tiposIdentificacion(), {
    initialValue: [],
  });

  protected reintentar(): void {
    this.recargar.update((n) => n + 1);
  }

  protected inscribir(idMateria: number): void {
    this.ejecutar(this.api.inscribir(idMateria), 'Materia inscrita.');
  }

  protected cancelar(idMateria: number): void {
    this.ejecutar(this.api.cancelar(idMateria), 'Inscripción cancelada.');
  }

  protected guardarDatos(request: ActualizarEstudianteRequest): void {
    this.ejecutar(this.api.actualizar(request), 'Datos actualizados.');
  }

  protected eliminarCuenta(): void {
    const datos: DatosConfirmacion = {
      titulo: 'Eliminar mi cuenta',
      mensaje: 'Se borrarán tu registro y todas tus inscripciones. Esta acción no se puede deshacer.',
      textoConfirmar: 'Eliminar',
    };

    this.dialogo
      .open(ConfirmacionDialog, { data: datos })
      .afterClosed()
      .pipe(
        filter((confirmado) => confirmado === true),
        switchMap(() => this.api.eliminar()),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: () => {
          this.notificacion.exito('Tu cuenta fue eliminada.');
          this.auth.cerrarSesion();
        },
        error: (error) => this.notificacion.error(error),
      });
  }

  private ejecutar(operacion: Observable<void>, mensajeExito: string): void {
    this.procesando.set(true);

    operacion
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => this.procesando.set(false)),
      )
      .subscribe({
        next: () => {
          this.notificacion.exito(mensajeExito);
          this.recargar.update((n) => n + 1);
        },
        error: (error) => {
          this.notificacion.error(error);
          this.recargar.update((n) => n + 1);
        },
      });
  }
}
