import { ChangeDetectionStrategy, Component, inject, input, output, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { ActualizarEstudianteRequest, CatalogoItem, MiRegistro } from '../../../core/api/modelos';

@Component({
  selector: 'app-datos-estudiante',
  imports: [ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule],
  templateUrl: './datos-estudiante.html',
  styleUrl: './datos-estudiante.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DatosEstudiante {
  readonly registro = input.required<MiRegistro>();
  readonly tiposIdentificacion = input.required<CatalogoItem[]>();
  readonly deshabilitado = input(false);

  readonly guardar = output<ActualizarEstudianteRequest>();
  readonly eliminar = output<void>();

  protected readonly editando = signal(false);

  protected readonly formulario = inject(NonNullableFormBuilder).group({
    nombre: ['', [Validators.required, Validators.maxLength(100)]],
    idTipoIdentificacion: [0, Validators.min(1)],
    numeroIdentificacion: ['', [Validators.required, Validators.maxLength(20)]],
  });

  protected editar(): void {
    const registro = this.registro();
    this.formulario.setValue({
      nombre: registro.nombre,
      idTipoIdentificacion: registro.idTipoIdentificacion,
      numeroIdentificacion: registro.numeroIdentificacion,
    });
    this.editando.set(true);
  }

  protected cancelarEdicion(): void {
    this.editando.set(false);
  }

  protected enviar(): void {
    if (this.formulario.invalid) {
      this.formulario.markAllAsTouched();
      return;
    }

    this.guardar.emit(this.formulario.getRawValue());
    this.editando.set(false);
  }
}
