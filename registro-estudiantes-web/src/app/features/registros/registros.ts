import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { catchError, debounceTime, distinctUntilChanged, map, of } from 'rxjs';
import { RegistroPublico } from '../../core/api/modelos';
import { MAXIMO_MATERIAS } from '../../core/api/reglas';
import { RegistrosService } from '../../core/api/registros.service';
import { EstadoVacio } from '../../shared/estado-vacio';
import { InicialesPipe } from '../../shared/iniciales.pipe';
import { NotificacionService } from '../../shared/notificacion.service';

@Component({
  selector: 'app-registros',
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    EstadoVacio,
    InicialesPipe,
  ],
  templateUrl: './registros.html',
  styleUrl: './registros.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Registros {
  private readonly notificacion = inject(NotificacionService);

  protected readonly maximoMaterias = MAXIMO_MATERIAS;

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

  protected limpiar(): void {
    this.busqueda.setValue('');
  }
}
