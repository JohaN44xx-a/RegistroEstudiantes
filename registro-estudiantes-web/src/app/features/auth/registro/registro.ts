import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSelectModule } from '@angular/material/select';
import { catchError, finalize, forkJoin, of, switchMap } from 'rxjs';
import { CatalogoService } from '../../../core/api/catalogo.service';
import { AuthService } from '../../../core/auth/auth.service';
import { NotificacionService } from '../../../shared/notificacion.service';

/** La "C" del CRUD: registro en línea de un estudiante. */
@Component({
  selector: 'app-registro',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatProgressBarModule,
  ],
  templateUrl: './registro.html',
  styleUrl: './registro.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Registro {
  private readonly auth = inject(AuthService);
  private readonly catalogo = inject(CatalogoService);
  private readonly router = inject(Router);
  private readonly notificacion = inject(NotificacionService);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly longitudMinimaContrasena = 8;
  protected readonly enviando = signal(false);

  // forkJoin: las dos peticiones del catálogo salen EN PARALELO y se
  // emite una sola vez cuando ambas terminan. toSignal lo convierte en
  // estado para la plantilla (undefined mientras carga).
  protected readonly catalogos = toSignal(
    forkJoin({
      programas: this.catalogo.programas(),
      tipos: this.catalogo.tiposIdentificacion(),
    }).pipe(
      catchError((error) => {
        this.notificacion.error(error);
        return of({ programas: [], tipos: [] });
      }),
    ),
  );

  protected readonly formulario = inject(NonNullableFormBuilder).group({
    nombre: ['', [Validators.required, Validators.maxLength(100)]],
    correoElectronico: ['', [Validators.required, Validators.email, Validators.maxLength(256)]],
    contrasena: ['', [Validators.required, Validators.minLength(this.longitudMinimaContrasena)]],
    idTipoIdentificacion: [0, Validators.min(1)],
    numeroIdentificacion: ['', [Validators.required, Validators.maxLength(20)]],
    idPrograma: [0, Validators.min(1)],
  });

  protected registrar(): void {
    if (this.formulario.invalid) {
      this.formulario.markAllAsTouched();
      return;
    }

    const datos = this.formulario.getRawValue();
    this.enviando.set(true);

    // switchMap encadena dos operaciones: primero registra y, cuando
    // termina, inicia sesión con las mismas credenciales.
    this.auth
      .registrar(datos)
      .pipe(
        switchMap(() =>
          this.auth.iniciarSesion({
            correoElectronico: datos.correoElectronico,
            contrasena: datos.contrasena,
          }),
        ),
        takeUntilDestroyed(this.destroyRef),
        finalize(() => this.enviando.set(false)),
      )
      .subscribe({
        next: () => {
          this.notificacion.exito('¡Registro exitoso! Ya puedes inscribir tus materias.');
          void this.router.navigateByUrl(this.auth.rutaInicio());
        },
        error: (error) => this.notificacion.error(error),
      });
  }
}
