import { HttpErrorResponse } from '@angular/common/http';
import { ProblemDetails } from '../api/modelos';

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
