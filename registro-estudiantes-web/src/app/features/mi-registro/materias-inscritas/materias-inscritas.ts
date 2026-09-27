import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MateriaInscrita } from '../../../core/api/modelos';
import { MAXIMO_MATERIAS } from '../../../core/api/reglas';

@Component({
  selector: 'app-materias-inscritas',
  imports: [MatCardModule, MatButtonModule, MatChipsModule, MatDividerModule],
  templateUrl: './materias-inscritas.html',
  styleUrl: './materias-inscritas.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MateriasInscritas {
  readonly materias = input.required<MateriaInscrita[]>();
  readonly totalCreditos = input.required<number>();
  readonly deshabilitado = input(false);

  readonly cancelar = output<number>();

  protected readonly maximoMaterias = MAXIMO_MATERIAS;
}
