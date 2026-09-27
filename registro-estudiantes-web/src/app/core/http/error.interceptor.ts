import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { NotificacionService } from '../../shared/notificacion.service';
import { AuthService } from '../auth/auth.service';

/**
 * Manejo centralizado de la sesión vencida: si con sesión activa la API
 * responde 401, el token ya no sirve (venció o fue alterado). Se cierra
 * la sesión y se avisa. El error se relanza para que quien hizo la
 * petición también se entere.
 */
export const errorInterceptor: HttpInterceptorFn = (request, next) => {
  const auth = inject(AuthService);
  const notificacion = inject(NotificacionService);

  return next(request).pipe(
    catchError((error: unknown) => {
      if (error instanceof HttpErrorResponse && error.status === 401 && auth.autenticado()) {
        auth.cerrarSesion();
        notificacion.aviso('Tu sesión expiró. Inicia sesión de nuevo.');
      }

      return throwError(() => error);
    }),
  );
};
