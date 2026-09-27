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

/**
 * Dueño del estado de la sesión.
 * - Signals para el ESTADO (qué sesión hay ahora).
 * - Observables para las OPERACIONES (las peticiones HTTP).
 */
@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly url = `${environment.apiUrl}/autenticacion`;
  private readonly claveAlmacenamiento = 'registro-estudiantes.sesion';

  // Estado privado y escribible; hacia afuera, solo lectura. Mismo
  // principio que la lista privada de inscripciones en el backend.
  private readonly _sesion = signal<IniciarSesionResponse | null>(this.leerSesionGuardada());
  readonly sesion = this._sesion.asReadonly();

  // Estado derivado: se recalcula solo cuando cambia la sesión.
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

  /** Pantalla inicial según el rol: el administrador no tiene "mi registro". */
  rutaInicio(): string {
    return this.esEstudiante() ? '/mi-registro' : '/registros';
  }

  private guardarSesion(sesion: IniciarSesionResponse): void {
    localStorage.setItem(this.claveAlmacenamiento, JSON.stringify(sesion));
    this._sesion.set(sesion);
  }

  /**
   * Recupera la sesión al recargar la página. Solo revisa la expiración al
   * arrancar; si el token vence durante el uso, lo detecta el
   * errorInterceptor cuando la API responda 401.
   */
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
      // Valor dañado a mano o de una versión anterior: se descarta.
      localStorage.removeItem(this.claveAlmacenamiento);
      return null;
    }
  }
}
