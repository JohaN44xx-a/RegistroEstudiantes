import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MateriaDisponible } from '../../../core/api/modelos';
import { MAXIMO_MATERIAS } from '../../../core/api/reglas';

interface OpcionMateria {
  materia: MateriaDisponible;
  /** null = se puede inscribir; texto = por qué no. */
  motivoBloqueo: string | null;
}

/**
 * Lista del plan de estudios. Todo lo que se muestra es ESTADO DERIVADO
 * con computed(): no hay código que "actualice" nada a mano.
 */
@Component({
  selector: 'app-materias-disponibles',
  imports: [MatCardModule, MatButtonModule, MatDividerModule],
  templateUrl: './materias-disponibles.html',
  styleUrl: './materias-disponibles.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MateriasDisponibles {
  readonly materias = input.required<MateriaDisponible[]>();
  readonly deshabilitado = input(false);

  readonly inscribir = output<number>();

  private readonly inscritas = computed(() => this.materias().filter((m) => m.inscrita));
  private readonly cupoLleno = computed(() => this.inscritas().length >= MAXIMO_MATERIAS);
  private readonly profesoresOcupados = computed(() => new Set(this.inscritas().map((m) => m.idProfesor)));

  // Solo es ayuda visual: deshabilita y explica. La regla real la aplica la API.
  protected readonly opciones = computed<OpcionMateria[]>(() =>
    this.materias()
      .filter((materia) => !materia.inscrita)
      .map((materia) => ({
        materia,
        motivoBloqueo: this.cupoLleno()
          ? `Ya tienes ${MAXIMO_MATERIAS} materias inscritas.`
          : this.profesoresOcupados().has(materia.idProfesor)
            ? `Ya tienes una materia con ${materia.profesor}.`
            : null,
      })),
  );
}
