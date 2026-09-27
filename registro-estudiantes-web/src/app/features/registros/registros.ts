import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { catchError, debounceTime, distinctUntilChanged, map, of } from 'rxjs';
import { RegistroPublico } from '../../core/api/modelos';
import { RegistrosService } from '../../core/api/registros.service';
import { NotificacionService } from '../../shared/notificacion.service';

/** Regla 8: ver en línea los registros de los demás estudiantes. */
@Component({
  selector: 'app-registros',
  imports: [ReactiveFormsModule, MatCardModule, MatChipsModule, MatFormFieldModule, MatInputModule, MatProgressBarModule],
  templateUrl: './registros.html',
  styleUrl: './registros.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Registros {
  private readonly notificacion = inject(NotificacionService);

  protected readonly registros = toSignal(
    inject(RegistrosService)
      .listar()
      .pipe(
        catchError((error) => {
          this.notificacion.error(error);
          return of([] as RegistroPublico[]);
        }),
      ),
  );

  protected readonly busqueda = new FormControl('', { nonNullable: true });

  // Caso clásico de RxJS: esperar a que el usuario deje de escribir
  // (debounceTime) e ignorar valores repetidos (distinctUntilChanged).
  private readonly termino = toSignal(
    this.busqueda.valueChanges.pipe(
      debounceTime(300),
      map((texto) => texto.trim().toLowerCase()),
      distinctUntilChanged(),
    ),
    { initialValue: '' },
  );

  protected readonly filtrados = computed(() => {
    const termino = this.termino();
    const registros = this.registros() ?? [];

    if (!termino) {
      return registros;
    }

    return registros.filter(
      (registro) =>
        registro.estudiante.toLowerCase().includes(termino) ||
        registro.programa.toLowerCase().includes(termino) ||
        registro.materias.some((materia) => materia.toLowerCase().includes(termino)),
    );
  });
}
