import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'app-estado-vacio',
  template: `
    <img [src]="imagen()" alt="" width="200" height="150" />
    <h3>{{ titulo() }}</h3>
    <p>{{ texto() }}</p>
    <ng-content />
  `,
  styles: `
    :host {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 0.5rem;
      padding: 1.5rem 1rem;
      text-align: center;
    }

    img {
      width: 160px;
      height: auto;
      margin-bottom: 0.5rem;
    }

    p {
      max-width: 34ch;
      color: var(--rel-tinta-suave);
      font-size: 0.95rem;
      margin-bottom: 0.5rem;
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EstadoVacio {
  readonly imagen = input.required<string>();
  readonly titulo = input.required<string>();
  readonly texto = input('');
}
