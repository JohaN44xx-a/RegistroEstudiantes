using RegistroEstudiantes.Domain.Common;
using RegistroEstudiantes.Domain.Estudiantes;

namespace RegistroEstudiantes.Domain.Tests;

public class EstudianteTests
{
    private static Estudiante CrearEstudiante() => new("Juan Pérez", 1, "123456", 1, 1);

    [Fact]
    public void InscribirMateria_SinInscripciones_AgregaLaInscripcion()
    {
        var estudiante = CrearEstudiante();
        estudiante.InscribirMateria(idMateria: 1, idProfesor: 1);
        Assert.Single(estudiante.Inscripciones);
    }

    [Fact]
    public void InscribirMateria_ConTresMaterias_LanzaExcepcion()
    {
        var estudiante = CrearEstudiante();
        estudiante.InscribirMateria(idMateria: 1, idProfesor: 1);
        estudiante.InscribirMateria(idMateria: 3, idProfesor: 2);
        estudiante.InscribirMateria(idMateria: 5, idProfesor: 3);

        Assert.Throws<ReglaDeNegocioException>(() => estudiante.InscribirMateria(idMateria: 7, idProfesor: 4));

        Assert.Equal(Estudiante.MaximoMaterias, estudiante.Inscripciones.Count);
    }

    [Fact]
    public void InscribirMateria_ConProfesorRepetido_LanzaExcepcion()
    {
        var estudiante = CrearEstudiante();
        estudiante.InscribirMateria(idMateria: 1, idProfesor: 1);

        Assert.Throws<ReglaDeNegocioException>(() => estudiante.InscribirMateria(idMateria: 2, idProfesor: 1));

        Assert.Single(estudiante.Inscripciones);
    }

    [Fact]
    public void CancelarInscripcion_MateriaInscrita_LaQuita()
    {
        var estudiante = CrearEstudiante();
        estudiante.InscribirMateria(idMateria: 1, idProfesor: 1);

        estudiante.CancelarInscripcion(idMateria: 1);

        Assert.Empty(estudiante.Inscripciones);
    }

    [Fact]
    public void CancelarInscripcion_MateriaNoInscrita_LanzaExcepcion()
    {
        var estudiante = CrearEstudiante();

        Assert.Throws<ReglaDeNegocioException>(() => estudiante.CancelarInscripcion(idMateria: 1));
    }
}