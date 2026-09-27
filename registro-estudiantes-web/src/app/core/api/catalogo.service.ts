import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { shareReplay } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CatalogoItem } from './modelos';

@Injectable({ providedIn: 'root' })
export class CatalogoService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/catalogo`;

  // El catálogo casi no cambia: se pide una vez y se comparte.
  private readonly programas$ = this.http
    .get<CatalogoItem[]>(`${this.url}/programas`)
    .pipe(shareReplay(1));

  private readonly tiposIdentificacion$ = this.http
    .get<CatalogoItem[]>(`${this.url}/tipos-identificacion`)
    .pipe(shareReplay(1));

  programas() {
    return this.programas$;
  }

  tiposIdentificacion() {
    return this.tiposIdentificacion$;
  }
}
