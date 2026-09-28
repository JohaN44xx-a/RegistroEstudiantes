import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { MateriaInscrita } from '../../../core/api/modelos';
import { CREDITOS_POR_MATERIA, MAXIMO_MATERIAS } from '../../../core/api/reglas';
import { InicialesPipe } from '../../../shared/iniciales.pipe';
import { PaletaProfesores } from '../../../shared/paleta-profesores';

// Las tres casillas del semestre y el avance en créditos. Solo presenta datos.
@Component({
  selector: 'app-cupo-semestre',
  imports: [InicialesPipe],
  templateUrl: './cupo-semestre.html',
  styleUrl: './cupo-semestre.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CupoSemestre {
  readonly materias = input.required<MateriaInscrita[]>();
  readonly totalCreditos = input.required<number>();
  readonly paleta = input.required<PaletaProfesores>();

  protected readonly maximoCreditos = MAXIMO_MATERIAS * CREDITOS_POR_MATERIA;

  protected readonly casillas = computed<(MateriaInscrita | null)[]>(() =>
    Array.from({ length: MAXIMO_MATERIAS }, (_, i) => this.materias()[i] ?? null),
  );

  protected readonly porcentaje = computed(() =>
    Math.min(100, Math.round((this.totalCreditos() / this.maximoCreditos) * 100)),
  );

  protected readonly mensaje = computed(() => {
    const libres = MAXIMO_MATERIAS - this.materias().length;

    if (libres <= 0) {
      return 'Semestre completo. Cancela una materia si quieres cambiarla.';
    }

    if (libres === MAXIMO_MATERIAS) {
      return 'Aún no tienes materias. Tienes 3 cupos libres.';
    }

    return libres === 1 ? 'Te queda 1 cupo libre.' : `Te quedan ${libres} cupos libres.`;
  });
}
