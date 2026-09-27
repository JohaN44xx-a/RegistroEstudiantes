import { Routes } from '@angular/router';
import { authGuard, estudianteGuard, invitadoGuard } from './core/auth/auth.guards';

// loadComponent: cada pantalla se descarga solo cuando se visita
// (lazy loading), así la carga inicial es más liviana.
export const routes: Routes = [
  {
    path: 'login',
    title: 'Iniciar sesión',
    canActivate: [invitadoGuard],
    loadComponent: () => import('./features/auth/login/login').then((m) => m.Login),
  },
  {
    path: 'registro',
    title: 'Crear cuenta',
    canActivate: [invitadoGuard],
    loadComponent: () => import('./features/auth/registro/registro').then((m) => m.Registro),
  },
  {
    path: 'mi-registro',
    title: 'Mi registro',
    canActivate: [authGuard, estudianteGuard],
    loadComponent: () => import('./features/mi-registro/mi-registro').then((m) => m.MiRegistroPagina),
  },
  {
    path: 'registros',
    title: 'Registros de estudiantes',
    canActivate: [authGuard],
    loadComponent: () => import('./features/registros/registros').then((m) => m.Registros),
  },
  { path: '', pathMatch: 'full', redirectTo: 'mi-registro' },
  { path: '**', redirectTo: 'mi-registro' },
];
