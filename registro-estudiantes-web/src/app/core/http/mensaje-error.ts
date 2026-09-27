import { HttpErrorResponse } from '@angular/common/http';
import { ProblemDetails } from '../api/modelos';

/**
 * Convierte cualquier error en un mensaje para el usuario. La API siempre
 * responde con ProblemDetails, así que el mensaje útil viene en "detail"
 * (por ejemplo: "Ya tienes una materia con este profesor...").
 */
export function mensajeDeError(error: unknown): string {
  if (error instanceof HttpErrorResponse) {
    if (error.status === 0) {
      return 'No se pudo conectar con el servidor. Verifica que la API esté en ejecución.';
    }

    const problema = error.error as ProblemDetails | null;
    return problema?.detail ?? problema?.title ?? 'Ocurrió un error inesperado.';
  }

  return 'Ocurrió un error inesperado.';
}
