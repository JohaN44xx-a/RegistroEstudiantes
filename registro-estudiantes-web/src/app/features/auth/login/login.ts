import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { finalize } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { AuthService } from '../../../core/auth/auth.service';
import { InicialesPipe } from '../../../shared/iniciales.pipe';
import { NotificacionService } from '../../../shared/notificacion.service';
import { PanelAcceso } from '../../../shared/panel-acceso';

interface UsuarioPrueba {
  nombre: string;
  correo: string;
  contrasena: string;
  descripcion: string;
}

@Component({
  selector: 'app-login',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    PanelAcceso,
    InicialesPipe,
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Login {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly notificacion = inject(NotificacionService);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly enviando = signal(false);
  protected readonly mostrarContrasena = signal(false);
  protected readonly mostrarUsuariosPrueba = !environment.production;

  // Los mismos usuarios que crea la API al arrancar en desarrollo.
  protected readonly usuariosPrueba: UsuarioPrueba[] = [
    {
      nombre: 'Juan Pérez',
      correo: 'juan.perez@registro.edu.co',
      contrasena: 'Estudiante2026*',
      descripcion: 'Profesor repetido',
    },
    {
      nombre: 'Ana Torres',
      correo: 'ana.torres@registro.edu.co',
      contrasena: 'Estudiante2026*',
      descripcion: 'Cupo lleno',
    },
    {
      nombre: 'María Rodríguez',
      correo: 'maria.rodriguez@registro.edu.co',
      contrasena: 'Estudiante2026*',
      descripcion: 'Comparte clases',
    },
    {
      nombre: 'Camila Vargas',
      correo: 'camila.vargas@registro.edu.co',
      contrasena: 'Estudiante2026*',
      descripcion: 'Sin materias',
    },
    {
      nombre: 'Administrador',
      correo: 'admin@registro.edu.co',
      contrasena: 'Admin2026*',
      descripcion: 'Consulta registros',
    },
  ];

  protected readonly formulario = inject(NonNullableFormBuilder).group({
    correoElectronico: ['', [Validators.required, Validators.email]],
    contrasena: ['', Validators.required],
  });

  protected usarUsuarioPrueba(usuario: UsuarioPrueba): void {
    this.formulario.setValue({ correoElectronico: usuario.correo, contrasena: usuario.contrasena });
  }

  protected iniciarSesion(): void {
    if (this.formulario.invalid) {
      this.formulario.markAllAsTouched();
      return;
    }

    this.enviando.set(true);

    this.auth
      .iniciarSesion(this.formulario.getRawValue())
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => this.enviando.set(false)),
      )
      .subscribe({
        next: () => void this.router.navigateByUrl(this.auth.rutaInicio()),
        error: (error) => this.notificacion.error(error),
      });
  }
}
