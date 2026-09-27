import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ActualizarEstudianteRequest, MateriaDisponible, MiRegistro } from './modelos';

/**
 * Operaciones del estudiante sobre SU registro. Nunca se envía el Id del
 * estudiante: la API lo toma del token.
 */
@Injectable({ providedIn: 'root' })
export class MiRegistroService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/mi-registro`;

  obtener(): Observable<MiRegistro> {
    return this.http.get<MiRegistro>(this.url);
  }

  actualizar(request: ActualizarEstudianteRequest): Observable<void> {
    return this.http.put<void>(this.url, request);
  }

  eliminar(): Observable<void> {
    return this.http.delete<void>(this.url);
  }

  materiasDisponibles(): Observable<MateriaDisponible[]> {
    return this.http.get<MateriaDisponible[]>(`${this.url}/materias-disponibles`);
  }

  inscribir(idMateria: number): Observable<void> {
    return this.http.post<void>(`${this.url}/inscripciones`, { idMateria });
  }

  cancelar(idMateria: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/inscripciones/${idMateria}`);
  }
}
