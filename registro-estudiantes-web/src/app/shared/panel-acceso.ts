import { ChangeDetectionStrategy, Component, input } from '@angular/core';

// Pantalla dividida para login y registro: panel de marca a la izquierda y formulario a la derecha.
@Component({
  selector: 'app-panel-acceso',
  template: `
    <aside class="lateral">
      <div class="marca">
        <img src="logo.svg" alt="" width="36" height="36" />
        <span>Registro de Estudiantes</span>
      </div>
      <img class="ilustracion" [src]="ilustracion()" alt="" width="400" height="300" />
      <h1 class="lema">{{ lema() }}</h1>
      <ng-content select="[lateral]" />
    </aside>

    <div class="principal">
      <div class="contenedor">
        <ng-content />
      </div>
    </div>
  `,
  styleUrl: './panel-acceso.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PanelAcceso {
  readonly ilustracion = input.required<string>();
  readonly lema = input.required<string>();
}
