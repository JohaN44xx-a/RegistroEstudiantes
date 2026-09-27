import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { shareReplay } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CatalogoItem } from './modelos';

@Injectable({ providedIn: 'root' })
export class CatalogoService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/catalogo`;

  // shareReplay(1): la primera suscripción hace la petición y las
  // siguientes reciben el mismo resultado guardado. El catálogo casi no
  // cambia, así que no tiene sentido pedirlo cada vez.
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
