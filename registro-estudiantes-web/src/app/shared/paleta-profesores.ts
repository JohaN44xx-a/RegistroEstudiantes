import { MateriaDisponible } from '../core/api/modelos';

export const TONOS_PROFESOR = 5;

// Nombre del profesor -> tono (0 a 4). Se usa en el atributo data-tono.
export type PaletaProfesores = ReadonlyMap<string, number>;

// Ordena por Id para que cada profesor conserve su color aunque cambie el orden de la lista.
export function crearPaleta(materias: readonly MateriaDisponible[]): PaletaProfesores {
  const profesores = [...new Map(materias.map((m) => [m.idProfesor, m.profesor])).entries()].sort(
    ([a], [b]) => a - b,
  );

  return new Map(profesores.map(([, nombre], indice) => [nombre, indice % TONOS_PROFESOR]));
}
