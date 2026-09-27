// Contratos con la API. Cada interfaz es el espejo de un DTO de C#,
// con las propiedades en camelCase (así las serializa ASP.NET Core).

export interface CatalogoItem {
  id: number;
  nombre: string;
}

export type Rol = 'Administrador' | 'Estudiante';

export interface IniciarSesionRequest {
  correoElectronico: string;
  contrasena: string;
}

export interface IniciarSesionResponse {
  token: string;
  expiraEnUtc: string;
  nombre: string;
  rol: Rol;
}

export interface RegistrarEstudianteRequest {
  nombre: string;
  correoElectronico: string;
  contrasena: string;
  idTipoIdentificacion: number;
  numeroIdentificacion: string;
  idPrograma: number;
}

export interface RegistroCreado {
  idEstudiante: number;
}

export interface ActualizarEstudianteRequest {
  nombre: string;
  idTipoIdentificacion: number;
  numeroIdentificacion: string;
}

export interface MateriaDisponible {
  idMateria: number;
  nombre: string;
  creditos: number;
  idProfesor: number;
  profesor: string;
  inscrita: boolean;
}

export interface MateriaInscrita {
  idMateria: number;
  nombre: string;
  creditos: number;
  profesor: string;
  /** Regla 9: la API solo expone el nombre de los compañeros. */
  companeros: string[];
}

export interface MiRegistro {
  idEstudiante: number;
  nombre: string;
  idTipoIdentificacion: number;
  tipoIdentificacion: string;
  numeroIdentificacion: string;
  programa: string;
  correoElectronico: string;
  totalCreditos: number;
  materias: MateriaInscrita[];
}

/** Regla 8: lo que se puede ver del registro de otro estudiante. */
export interface RegistroPublico {
  estudiante: string;
  programa: string;
  materias: string[];
}

/** Formato de error que devuelve la API (RFC 9457). */
export interface ProblemDetails {
  title?: string;
  detail?: string;
  status?: number;
}
