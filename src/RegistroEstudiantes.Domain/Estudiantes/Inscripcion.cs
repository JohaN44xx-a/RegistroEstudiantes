using System;
using System.Collections.Generic;
using System.Text;

namespace RegistroEstudiantes.Domain.Estudiantes;

public class Inscripcion
{
    public int IdInscripcion { get; private set; }
    public int IdEstudiante { get; private set; }
    public int IdMateria { get; private set; }
    public int IdProfesor { get; private set; }
    public DateTime FechaInscripcion { get; private set; }

    private Inscripcion() { }

    internal Inscripcion(int idMateria, int idProfesor)
    {
        IdMateria = idMateria;
        IdProfesor = idProfesor;
        FechaInscripcion = DateTime.UtcNow;
    }
}
