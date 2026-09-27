import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { AuthService } from './auth.service';

/**
 * Agrega el JWT a cada petición hacia NUESTRA API. Se valida la URL para
 * no filtrar el token si algún día se llama a un servicio externo.
 */
export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const token = inject(AuthService).token();

  if (!token || !request.url.startsWith(environment.apiUrl)) {
    return next(request);
  }

  return next(request.clone({ setHeaders: { Authorization: `Bearer ${token}` } }));
};
