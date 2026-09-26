using RegistroEstudiantes.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace RegistroEstudiantes.Domain.Estudiantes
{
    public class Estudiante
    {
        public const int MaximoMaterias = 3;

        private readonly List<Inscripcion> _inscripciones = new();

        public int IdEstudiante { get; private set; }
        public string Nombre { get; private set; } = null!;
        public int IdTipoIdentificacion { get; private set; }
        public string NumeroIdentificacion { get; private set; } = null!;
        public int IdPrograma { get; private set; }
        public int IdUsuario { get; private set; }

        public IReadOnlyCollection<Inscripcion> Inscripciones => _inscripciones.AsReadOnly();

        private Estudiante() 
        { 
        }

        public Estudiante(string nombre, int idTipoIdentificacion, string numeroIdentificacion, int idPrograma, int idUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ReglaDeNegocioException("El nombre del estudiante es obligatorio.");

            if (string.IsNullOrWhiteSpace(numeroIdentificacion))
                throw new ReglaDeNegocioException("El número de identificación es obligatorio.");

            Nombre = nombre.Trim();
            IdTipoIdentificacion = idTipoIdentificacion;
            NumeroIdentificacion = numeroIdentificacion.Trim();
            IdPrograma = idPrograma;
            IdUsuario = idUsuario;
        }

        public void InscribirMateria(int idMateria, int idProfesor)
        {
            if (_inscripciones.Count >= MaximoMaterias)
            {
                throw new ReglaDeNegocioException($"Solo puedes inscribir {MaximoMaterias} materias.");
            }

            if (_inscripciones.Any(i => i.IdProfesor == idProfesor))
            {
                throw new ReglaDeNegocioException("Ya tienes una materia con este profesor. Debes elegir materias de profesores diferentes.");
            }

            _inscripciones.Add(new Inscripcion(idMateria, idProfesor));
        }

        public void CancelarInscripcion(int idMateria)
        {
            var inscripcion = _inscripciones.FirstOrDefault(i => i.IdMateria == idMateria);
            if (inscripcion is null)
                throw new ReglaDeNegocioException("No tienes inscrita esta materia.");

            _inscripciones.Remove(inscripcion);
        }
    }
}
