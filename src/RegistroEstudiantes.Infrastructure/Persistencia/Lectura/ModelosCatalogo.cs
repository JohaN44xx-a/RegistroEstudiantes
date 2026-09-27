namespace RegistroEstudiantes.Infrastructure.Persistencia.Lectura;

// Clases simples que representan las tablas del catálogo.
// NO son entidades del dominio: el catálogo se carga con el script SQL
// y la aplicación solo lo consulta. Viven en Infrastructure porque son
// un detalle de cómo se lee la base.

public class RolDb
{
    public int IdRol { get; set; }
    public string Nombre { get; set; } = null!;
}

public class TipoIdentificacionDb
{
    public int IdTipoIdentificacion { get; set; }
    public string Nombre { get; set; } = null!;
}

public class ProgramaDb
{
    public int IdPrograma { get; set; }
    public string Nombre { get; set; } = null!;
}

public class ProfesorDb
{
    public int IdProfesor { get; set; }
    public string Nombre { get; set; } = null!;
    public string Cargo { get; set; } = null!;
    public int? IdUsuario { get; set; }
}

public class MateriaDb
{
    public int IdMateria { get; set; }
    public string Nombre { get; set; } = null!;
    public int Creditos { get; set; }
    public int IdProfesor { get; set; }
}

public class PlanEstudiosDb
{
    public int IdPrograma { get; set; }
    public int IdMateria { get; set; }
}
