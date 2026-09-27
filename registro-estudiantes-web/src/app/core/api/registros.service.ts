import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { RegistroPublico } from './modelos';

@Injectable({ providedIn: 'root' })
export class RegistrosService {
  private readonly http = inject(HttpClient);

  listar(): Observable<RegistroPublico[]> {
    return this.http.get<RegistroPublico[]>(`${environment.apiUrl}/registros`);
  }
}
