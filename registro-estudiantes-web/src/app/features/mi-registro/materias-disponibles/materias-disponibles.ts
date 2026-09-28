import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MateriaDisponible } from '../../../core/api/modelos';
import { MAXIMO_MATERIAS } from '../../../core/api/reglas';
import { EstadoVacio } from '../../../shared/estado-vacio';
import { InicialesPipe } from '../../../shared/iniciales.pipe';
import { PaletaProfesores } from '../../../shared/paleta-profesores';

interface OpcionMateria {
  materia: MateriaDisponible;
  profesorRepetido: boolean;
}

@Component({
  selector: 'app-materias-disponibles',
  imports: [MatButtonModule, EstadoVacio, InicialesPipe],
  templateUrl: './materias-disponibles.html',
  styleUrl: './materias-disponibles.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MateriasDisponibles {
  readonly materias = input.required<MateriaDisponible[]>();
  readonly paleta = input.required<PaletaProfesores>();
  readonly deshabilitado = input(false);

  readonly inscribir = output<number>();

  private readonly inscritas = computed(() => this.materias().filter((m) => m.inscrita));
  private readonly profesoresOcupados = computed(
    () => new Set(this.inscritas().map((m) => m.idProfesor)),
  );

  protected readonly maximoMaterias = MAXIMO_MATERIAS;
  protected readonly cupoLleno = computed(() => this.inscritas().length >= MAXIMO_MATERIAS);

  // Solo guía visual: deshabilita y explica. La regla la aplica la API.
  protected readonly opciones = computed<OpcionMateria[]>(() =>
    this.materias()
      .filter((materia) => !materia.inscrita)
      .map((materia) => ({
        materia,
        profesorRepetido: this.profesoresOcupados().has(materia.idProfesor),
      })),
  );
}
