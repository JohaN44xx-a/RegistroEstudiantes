import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  IniciarSesionRequest,
  IniciarSesionResponse,
  RegistrarEstudianteRequest,
  RegistroCreado,
} from '../api/modelos';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly url = `${environment.apiUrl}/autenticacion`;
  private readonly claveAlmacenamiento = 'registro-estudiantes.sesion';

  private readonly _sesion = signal<IniciarSesionResponse | null>(this.leerSesionGuardada());
  readonly sesion = this._sesion.asReadonly();

  readonly autenticado = computed(() => this._sesion() !== null);
  readonly token = computed(() => this._sesion()?.token ?? null);
  readonly nombre = computed(() => this._sesion()?.nombre ?? '');
  readonly rol = computed(() => this._sesion()?.rol ?? null);
  readonly esEstudiante = computed(() => this.rol() === 'Estudiante');

  registrar(request: RegistrarEstudianteRequest): Observable<RegistroCreado> {
    return this.http.post<RegistroCreado>(`${this.url}/registro`, request);
  }

  iniciarSesion(request: IniciarSesionRequest): Observable<IniciarSesionResponse> {
    return this.http
      .post<IniciarSesionResponse>(`${this.url}/login`, request)
      .pipe(tap((sesion) => this.guardarSesion(sesion)));
  }

  cerrarSesion(): void {
    localStorage.removeItem(this.claveAlmacenamiento);
    this._sesion.set(null);
    void this.router.navigate(['/login']);
  }

  rutaInicio(): string {
    return this.esEstudiante() ? '/mi-registro' : '/registros';
  }

  private guardarSesion(sesion: IniciarSesionResponse): void {
    localStorage.setItem(this.claveAlmacenamiento, JSON.stringify(sesion));
    this._sesion.set(sesion);
  }

  // Solo valida la expiración al cargar; durante el uso la detecta el errorInterceptor (401).
  private leerSesionGuardada(): IniciarSesionResponse | null {
    try {
      const texto = localStorage.getItem(this.claveAlmacenamiento);
      if (!texto) {
        return null;
      }

      const sesion = JSON.parse(texto) as IniciarSesionResponse;
      if (new Date(sesion.expiraEnUtc) <= new Date()) {
        localStorage.removeItem(this.claveAlmacenamiento);
        return null;
      }

      return sesion;
    } catch {
      localStorage.removeItem(this.claveAlmacenamiento);
      return null;
    }
  }
}
