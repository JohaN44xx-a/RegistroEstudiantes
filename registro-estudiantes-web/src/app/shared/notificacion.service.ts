import { inject, Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { mensajeDeError } from '../core/http/mensaje-error';

@Injectable({ providedIn: 'root' })
export class NotificacionService {
  private readonly snackBar = inject(MatSnackBar);

  exito(mensaje: string): void {
    this.snackBar.open(mensaje, 'Cerrar', { duration: 3500 });
  }

  aviso(mensaje: string): void {
    this.snackBar.open(mensaje, 'Cerrar', { duration: 5000 });
  }

  error(error: unknown): void {
    this.snackBar.open(mensajeDeError(error), 'Cerrar', { duration: 6000 });
  }
}
