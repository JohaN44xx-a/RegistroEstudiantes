import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';

/** Solo usuarios con sesión. */
export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  return auth.autenticado() ? true : inject(Router).createUrlTree(['/login']);
};

/** Solo estudiantes (el administrador no tiene registro propio). */
export const estudianteGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  return auth.esEstudiante() ? true : inject(Router).createUrlTree(['/registros']);
};

/** Login y registro: si ya hay sesión, no tiene sentido mostrarlos. */
export const invitadoGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  return auth.autenticado() ? inject(Router).createUrlTree([auth.rutaInicio()]) : true;
};
