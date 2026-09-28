import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MateriaInscrita } from '../../../core/api/modelos';
import { MAXIMO_MATERIAS } from '../../../core/api/reglas';
import { EstadoVacio } from '../../../shared/estado-vacio';
import { InicialesPipe } from '../../../shared/iniciales.pipe';
import { PaletaProfesores } from '../../../shared/paleta-profesores';

@Component({
  selector: 'app-materias-inscritas',
  imports: [MatButtonModule, MatTooltipModule, EstadoVacio, InicialesPipe],
  templateUrl: './materias-inscritas.html',
  styleUrl: './materias-inscritas.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MateriasInscritas {
  readonly materias = input.required<MateriaInscrita[]>();
  readonly paleta = input.required<PaletaProfesores>();
  readonly deshabilitado = input(false);

  readonly cancelar = output<number>();

  protected readonly maximoMaterias = MAXIMO_MATERIAS;
}
