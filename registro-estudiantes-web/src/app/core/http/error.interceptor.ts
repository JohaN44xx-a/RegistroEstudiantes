import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { NotificacionService } from '../../shared/notificacion.service';
import { AuthService } from '../auth/auth.service';

export const errorInterceptor: HttpInterceptorFn = (request, next) => {
  const auth = inject(AuthService);
  const notificacion = inject(NotificacionService);

  return next(request).pipe(
    catchError((error: unknown) => {
      // Con sesión activa, un 401 significa token vencido o alterado.
      if (error instanceof HttpErrorResponse && error.status === 401 && auth.autenticado()) {
        auth.cerrarSesion();
        notificacion.aviso('Tu sesión expiró. Inicia sesión de nuevo.');
      }

      return throwError(() => error);
    }),
  );
};
