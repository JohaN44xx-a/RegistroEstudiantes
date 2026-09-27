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

export interface RegistroPublico {
  estudiante: string;
  programa: string;
  materias: string[];
}

export interface ProblemDetails {
  title?: string;
  detail?: string;
  status?: number;
}
