import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';

export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  return auth.autenticado() ? true : inject(Router).createUrlTree(['/login']);
};

export const estudianteGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  return auth.esEstudiante() ? true : inject(Router).createUrlTree(['/registros']);
};

export const invitadoGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  return auth.autenticado() ? inject(Router).createUrlTree([auth.rutaInicio()]) : true;
};
