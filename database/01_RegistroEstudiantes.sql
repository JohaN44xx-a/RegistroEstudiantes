/*
   PRUEBA TÉCNICA INTER RAPIDÍSIMO - REGISTRO DE ESTUDIANTES
   Motor: SQL Server 2022

   ¿Qué hace este script?
   Crea la base de datos RegistroEstudiantes, todas sus tablas con sus
   restricciones, y carga los datos fijos (roles, tipos de documento,
   programas, profesores, materias y planes de estudio).

   ¿Por qué el esquema se crea con este script y no con migraciones?
   Este archivo es el único lugar donde se define la estructura de la
   base. La API usa EF Core solo para leer y escribir en estas tablas,
   sin generar migraciones. Así se evita tener dos definiciones del
   mismo esquema que con el tiempo podrían dejar de coincidir.

   ¿Dónde se validan las reglas del enunciado?
   En dos lugares:
     1. En el código (dominio): el estudiante revisa las reglas antes
        de aceptar una inscripción. Es la validación principal y la que
        le devuelve un mensaje claro al usuario.
     2. En la base de datos: si algo se salta el código (un error, un
        INSERT manual, dos peticiones al mismo tiempo), la base rechaza
        el dato inválido.

   Reglas que la base de datos sí puede proteger por sí sola:
     - Cada materia tiene un solo profesor.
     - Cada materia vale exactamente 3 créditos.
     - Un estudiante no puede tener dos materias con el mismo profesor,
       y tampoco puede inscribir la misma materia dos veces.
     - Si una inscripción dice "Cálculo con el profesor X", X debe ser
       de verdad quien dicta Cálculo.

   Reglas que la base de datos NO puede proteger fácilmente, y por eso
   viven solo en el código:
     - Máximo 3 materias por estudiante: para saberlo hay que contar
       filas, y una restricción CHECK solo ve la fila que se inserta.
     - El estudiante solo puede inscribir materias del plan de estudios
       de su programa: requiere consultar otras tablas.
     - Cada profesor dicta exactamente 2 materias: se garantiza con los
       datos del catálogo que se cargan al final de este script.
*/


-- Crea la base solo si no existe, para no fallar al ejecutar el script
-- una segunda vez.
IF DB_ID(N'RegistroEstudiantes') IS NULL
    CREATE DATABASE RegistroEstudiantes;
GO

USE RegistroEstudiantes;
GO

/* ---------------------------------------------------------------------
   LIMPIEZA
   Borra las tablas si ya existen, para poder correr el script completo
   varias veces y siempre quedar con una base limpia.

   El orden importa: se borran primero las tablas que apuntan a otras.
   Por ejemplo, Inscripcion apunta a Estudiante; si intentáramos borrar
   Estudiante primero, SQL Server lo impediría porque hay inscripciones
   que dependen de él.
   --------------------------------------------------------------------- */
DROP TABLE IF EXISTS dbo.Inscripcion;
DROP TABLE IF EXISTS dbo.PlanEstudios;
DROP TABLE IF EXISTS dbo.Materia;
DROP TABLE IF EXISTS dbo.Estudiante;
DROP TABLE IF EXISTS dbo.Profesor;
DROP TABLE IF EXISTS dbo.Usuario;
DROP TABLE IF EXISTS dbo.Programa;
DROP TABLE IF EXISTS dbo.TipoIdentificacion;
DROP TABLE IF EXISTS dbo.Rol;
GO

/* ---------------------------------------------------------------------
   1. CATÁLOGOS
   Tablas con listas fijas de opciones que no dependen de ninguna otra.
   Se crean primero porque las demás tablas las referencian.

   En las tres se pone UNIQUE sobre el nombre para que no existan, por
   ejemplo, dos roles llamados "Estudiante".
   --------------------------------------------------------------------- */

-- Roles del sistema. Define qué puede hacer cada usuario al iniciar
-- sesión (por ejemplo, el administrador ve todo y el estudiante solo
-- gestiona su propio registro).
CREATE TABLE dbo.Rol
(
    IdRol   INT IDENTITY(1,1) NOT NULL,
    Nombre  NVARCHAR(50)      NOT NULL,
    CONSTRAINT PK_Rol PRIMARY KEY (IdRol),
    CONSTRAINT UQ_Rol_Nombre UNIQUE (Nombre)
);

-- Tipos de documento: cédula, tarjeta de identidad, pasaporte, etc.
-- Se maneja como tabla y no como texto libre para evitar que un mismo
-- tipo quede escrito de formas distintas ("CC", "Cédula", "cedula").
CREATE TABLE dbo.TipoIdentificacion
(
    IdTipoIdentificacion INT IDENTITY(1,1) NOT NULL,
    Nombre               NVARCHAR(50)      NOT NULL,
    CONSTRAINT PK_TipoIdentificacion PRIMARY KEY (IdTipoIdentificacion),
    CONSTRAINT UQ_TipoIdentificacion_Nombre UNIQUE (Nombre)
);

-- Programas académicos. El enunciado habla de "un programa de créditos",
-- pero se modelaron varios programas porque así funciona una
-- universidad real.
CREATE TABLE dbo.Programa
(
    IdPrograma INT IDENTITY(1,1) NOT NULL,
    Nombre     NVARCHAR(100)     NOT NULL,
    CONSTRAINT PK_Programa PRIMARY KEY (IdPrograma),
    CONSTRAINT UQ_Programa_Nombre UNIQUE (Nombre)
);
GO

/* ---------------------------------------------------------------------
   2. USUARIO
   Guarda solo lo necesario para iniciar sesión: correo, contraseña
   (como hash) y rol.

   Decisión de diseño: Usuario no tiene columnas que apunten a
   Estudiante ni a Profesor. Al revés: Estudiante y Profesor apuntan a
   Usuario. Así el administrador, que no es ni estudiante ni profesor,
   no necesita columnas vacías (NULL), y un usuario no puede quedar
   marcado como estudiante y profesor a la vez.
   --------------------------------------------------------------------- */
CREATE TABLE dbo.Usuario
(
    IdUsuario         INT IDENTITY(1,1) NOT NULL,
    Nombre            NVARCHAR(100)     NOT NULL,
    CorreoElectronico NVARCHAR(256)     NOT NULL,
    -- Nunca se guarda la contraseña real. Se guarda un hash: un valor que
    -- se calcula a partir de la contraseña y que no se puede revertir.
    -- En el login se calcula el hash de lo que escribió el usuario y se
    -- compara con este. La aplicación (.NET) le agrega un "salt"
    -- aleatorio antes de calcularlo, para que dos usuarios con la misma
    -- contraseña no tengan el mismo hash. El salt queda guardado dentro
    -- de este mismo texto, por eso no necesita una columna aparte.
    ContrasenaHash    NVARCHAR(500)     NOT NULL,
    IdRol             INT               NOT NULL,
    CONSTRAINT PK_Usuario PRIMARY KEY (IdUsuario),
    -- El correo es el dato con el que se inicia sesión, así que no se
    -- puede repetir: dos cuentas con el mismo correo harían imposible
    -- saber a quién pertenece el login.
    CONSTRAINT UQ_Usuario_CorreoElectronico UNIQUE (CorreoElectronico),
    -- Validación básica de formato: exige algo antes de la @, algo
    -- después, y un punto en el dominio. La validación completa la hace
    -- la aplicación; esta solo evita datos claramente inválidos.
    CONSTRAINT CK_Usuario_CorreoElectronico CHECK (CorreoElectronico LIKE N'%_@_%._%'),
    CONSTRAINT FK_Usuario_Rol FOREIGN KEY (IdRol) REFERENCES dbo.Rol (IdRol)
);
GO

/* ---------------------------------------------------------------------
   3. PROFESOR Y ESTUDIANTE
   --------------------------------------------------------------------- */

-- Profesores. Forman parte del catálogo: existen antes de que cualquier
-- estudiante se registre.
CREATE TABLE dbo.Profesor
(
    IdProfesor INT IDENTITY(1,1) NOT NULL,
    Nombre     NVARCHAR(100)     NOT NULL,
    Cargo      NVARCHAR(100)     NOT NULL,
    -- IdUsuario permite NULL porque el enunciado no pide que los
    -- profesores inicien sesión. Los profesores se cargan sin cuenta, y
    -- la columna queda lista por si en el futuro se les quiere dar acceso.
    IdUsuario  INT               NULL,
    CONSTRAINT PK_Profesor PRIMARY KEY (IdProfesor),
    CONSTRAINT FK_Profesor_Usuario FOREIGN KEY (IdUsuario) REFERENCES dbo.Usuario (IdUsuario)
);

-- Dos profesores no pueden compartir la misma cuenta de usuario.
-- El problema: en SQL Server, una restricción UNIQUE normal trata los
-- NULL como un valor más, y solo permitiría un profesor sin cuenta; el
-- segundo fallaría. Este índice filtrado aplica la unicidad solo a las
-- filas que sí tienen usuario, así que pueden existir muchos profesores
-- sin cuenta, pero nunca dos con la misma.
CREATE UNIQUE NONCLUSTERED INDEX UX_Profesor_IdUsuario
    ON dbo.Profesor (IdUsuario)
    WHERE IdUsuario IS NOT NULL;

-- Estudiantes. Se crean en el registro en línea (el CRUD del enunciado):
-- en ese momento la aplicación crea el Usuario y el Estudiante en una
-- misma transacción, por eso IdUsuario nunca queda vacío.
CREATE TABLE dbo.Estudiante
(
    IdEstudiante         INT IDENTITY(1,1) NOT NULL,
    Nombre               NVARCHAR(100)     NOT NULL,
    IdTipoIdentificacion INT               NOT NULL,
    -- Es texto y no número porque algunos documentos, como el pasaporte,
    -- llevan letras, y porque un número de documento nunca se usa para
    -- hacer cálculos.
    NumeroIdentificacion NVARCHAR(20)      NOT NULL,
    IdPrograma           INT               NOT NULL,
    IdUsuario            INT               NOT NULL,
    CONSTRAINT PK_Estudiante PRIMARY KEY (IdEstudiante),
    -- No pueden existir dos estudiantes con el mismo documento. Se valida
    -- la pareja tipo + número (y no solo el número) porque una cédula
    -- 123456 y un pasaporte 123456 son documentos distintos de personas
    -- distintas.
    CONSTRAINT UQ_Estudiante_Identificacion UNIQUE (IdTipoIdentificacion, NumeroIdentificacion),
    -- Relación 1 a 1 con Usuario: la FK por sí sola permitiría que dos
    -- estudiantes apunten a la misma cuenta; este UNIQUE lo impide.
    CONSTRAINT UQ_Estudiante_IdUsuario UNIQUE (IdUsuario),
    CONSTRAINT FK_Estudiante_TipoIdentificacion FOREIGN KEY (IdTipoIdentificacion)
        REFERENCES dbo.TipoIdentificacion (IdTipoIdentificacion),
    CONSTRAINT FK_Estudiante_Programa FOREIGN KEY (IdPrograma)
        REFERENCES dbo.Programa (IdPrograma),
    CONSTRAINT FK_Estudiante_Usuario FOREIGN KEY (IdUsuario)
        REFERENCES dbo.Usuario (IdUsuario)
);

-- SQL Server no crea índices automáticamente sobre las FK. Este acelera
-- las búsquedas de estudiantes por programa.
CREATE NONCLUSTERED INDEX IX_Estudiante_IdPrograma ON dbo.Estudiante (IdPrograma);
GO

/* ---------------------------------------------------------------------
   4. MATERIA
   Relación con Profesor: 1 a N. Un profesor dicta varias materias (2),
   pero cada materia tiene un solo profesor. Por eso la FK IdProfesor
   vive aquí, en el lado de "muchos".
   --------------------------------------------------------------------- */
CREATE TABLE dbo.Materia
(
    IdMateria  INT IDENTITY(1,1) NOT NULL,
    Nombre     NVARCHAR(100)     NOT NULL,
    Creditos   INT               NOT NULL,
    IdProfesor INT               NOT NULL,
    CONSTRAINT PK_Materia PRIMARY KEY (IdMateria),
    CONSTRAINT UQ_Materia_Nombre UNIQUE (Nombre),
    -- Regla 4 del enunciado: cada materia equivale a 3 créditos.
    CONSTRAINT CK_Materia_Creditos CHECK (Creditos = 3),
    -- A primera vista sobra: IdMateria ya es la llave primaria, así que
    -- la pareja (IdMateria, IdProfesor) ya es única de por sí.
    -- Se declara porque SQL Server exige que una FK apunte a columnas
    -- marcadas como PRIMARY KEY o UNIQUE. Sin esta línea no podríamos
    -- crear la FK compuesta de Inscripcion (ver más abajo).
    CONSTRAINT UQ_Materia_IdMateria_IdProfesor UNIQUE (IdMateria, IdProfesor),
    CONSTRAINT FK_Materia_Profesor FOREIGN KEY (IdProfesor) REFERENCES dbo.Profesor (IdProfesor)
);

-- Acelera la consulta "qué materias dicta este profesor".
CREATE NONCLUSTERED INDEX IX_Materia_IdProfesor ON dbo.Materia (IdProfesor);
GO

/* ---------------------------------------------------------------------
   5. PLAN DE ESTUDIOS
   Define qué materias pertenecen a cada programa.

   Relación muchos a muchos: un programa tiene varias materias, y una
   misma materia puede estar en varios programas (por ejemplo,
   Metodología de la Investigación la ven estudiantes de Sistemas y de
   Administración). Por eso necesita esta tabla intermedia.
   --------------------------------------------------------------------- */
CREATE TABLE dbo.PlanEstudios
(
    IdPrograma INT NOT NULL,
    IdMateria  INT NOT NULL,
    -- La llave primaria es la pareja de columnas: no hace falta un Id
    -- propio, y así una materia no puede aparecer dos veces en el mismo
    -- programa.
    CONSTRAINT PK_PlanEstudios PRIMARY KEY (IdPrograma, IdMateria),
    CONSTRAINT FK_PlanEstudios_Programa FOREIGN KEY (IdPrograma) REFERENCES dbo.Programa (IdPrograma),
    CONSTRAINT FK_PlanEstudios_Materia  FOREIGN KEY (IdMateria)  REFERENCES dbo.Materia (IdMateria)
);

-- La llave primaria ya sirve para buscar por programa (es su primera
-- columna). Este índice cubre la búsqueda contraria: en qué programas
-- está una materia.
CREATE NONCLUSTERED INDEX IX_PlanEstudios_IdMateria ON dbo.PlanEstudios (IdMateria);
GO

/* ---------------------------------------------------------------------
   6. INSCRIPCIÓN
   Cada fila significa "este estudiante eligió esta materia".

   En el dominio, Inscripcion no existe por sí sola: pertenece al
   Estudiante. Toda inscripción nueva pasa por el objeto Estudiante, que
   es quien revisa las reglas (máximo 3 materias, sin repetir profesor,
   solo materias de su plan) antes de aceptarla.
   --------------------------------------------------------------------- */
CREATE TABLE dbo.Inscripcion
(
    IdInscripcion    INT IDENTITY(1,1) NOT NULL,
    IdEstudiante     INT               NOT NULL,
    IdMateria        INT               NOT NULL,
    -- Este dato ya existe en Materia, pero se copia aquí a propósito:
    -- sin él, la base no tendría cómo impedir que un estudiante repita
    -- profesor, porque las restricciones solo ven columnas de su propia
    -- tabla.
    IdProfesor       INT               NOT NULL,
    -- Se llena sola con la fecha y hora (UTC) del momento de la
    -- inscripción.
    FechaInscripcion DATETIME2(0)      NOT NULL
        CONSTRAINT DF_Inscripcion_FechaInscripcion DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_Inscripcion PRIMARY KEY (IdInscripcion),

    -- Regla 7: el estudiante no puede tener clases con el mismo profesor.
    -- Ejemplo: si Juan ya tiene Cálculo con Laura Gómez, no puede
    -- inscribir Álgebra Lineal, que también dicta ella.
    -- Efecto adicional: como cada materia tiene un solo profesor,
    -- inscribir la misma materia dos veces también repetiría profesor,
    -- así que esta misma restricción lo impide. No hace falta otra.
    CONSTRAINT UQ_Inscripcion_Estudiante_Profesor UNIQUE (IdEstudiante, IdProfesor),

    -- Si se elimina un estudiante, sus inscripciones se eliminan con él:
    -- una inscripción sin estudiante no tiene sentido.
    CONSTRAINT FK_Inscripcion_Estudiante FOREIGN KEY (IdEstudiante)
        REFERENCES dbo.Estudiante (IdEstudiante) ON DELETE CASCADE,

    -- FK compuesta: revisa las dos columnas juntas contra Materia.
    -- Con dos FK separadas, la base aceptaría "Cálculo con el profesor
    -- Carlos" porque Cálculo existe y Carlos existe, aunque Cálculo lo
    -- dicte Laura. Con la FK compuesta, esa combinación se rechaza
    -- porque no existe en Materia.
    -- ON UPDATE CASCADE: si la universidad cambia el profesor de una
    -- materia, las inscripciones se actualizan solas. Si con ese cambio
    -- algún estudiante quedara con dos materias del mismo profesor, el
    -- UNIQUE de arriba rechaza la actualización completa.
    CONSTRAINT FK_Inscripcion_Materia_Profesor FOREIGN KEY (IdMateria, IdProfesor)
        REFERENCES dbo.Materia (IdMateria, IdProfesor) ON UPDATE CASCADE
);

-- Acelera la consulta de la regla 9: "con quiénes comparto esta clase",
-- que busca todas las inscripciones de una misma materia.
CREATE NONCLUSTERED INDEX IX_Inscripcion_IdMateria ON dbo.Inscripcion (IdMateria);
GO

/* ---------------------------------------------------------------------
   DATOS INICIALES DEL CATÁLOGO

   Aquí no se crean usuarios. La contraseña se guarda como hash con un
   salt aleatorio, y eso lo calcula la aplicación en .NET; un hash
   escrito a mano en SQL no funcionaría en el login. Por eso:
     - El usuario administrador lo crea la API la primera vez que arranca.
     - Los estudiantes se crean desde el formulario de registro.

   Todo va dentro de una transacción: si cualquier INSERT falla, no se
   guarda nada y la base no queda con datos a medias.
   SET XACT_ABORT ON hace que cualquier error cancele la transacción
   completa automáticamente.
   --------------------------------------------------------------------- */
SET XACT_ABORT ON;
BEGIN TRANSACTION;

INSERT INTO dbo.Rol (Nombre)
VALUES (N'Administrador'), (N'Estudiante');

INSERT INTO dbo.TipoIdentificacion (Nombre)
VALUES (N'Cédula de ciudadanía'),
       (N'Tarjeta de identidad'),
       (N'Cédula de extranjería'),
       (N'Pasaporte');

INSERT INTO dbo.Programa (Nombre)
VALUES (N'Ingeniería de Sistemas'),
       (N'Administración de Empresas');

-- Regla 6 del enunciado: 5 profesores.
INSERT INTO dbo.Profesor (Nombre, Cargo)
VALUES (N'Laura Gómez',      N'Docente de planta'),
       (N'Carlos Rodríguez', N'Docente de planta'),
       (N'Diana Martínez',   N'Docente de cátedra'),
       (N'Andrés López',     N'Docente de cátedra'),
       (N'Sofía Herrera',    N'Docente de planta');

-- Reglas 3, 4 y 6: 10 materias de 3 créditos, 2 por cada profesor.
-- En lugar de escribir los Id a mano (que dependen del orden en que se
-- insertaron los profesores), se busca cada profesor por su nombre.
-- Así el script no se rompe si cambia el orden de los INSERT.
INSERT INTO dbo.Materia (Nombre, Creditos, IdProfesor)
SELECT m.Nombre, 3, p.IdProfesor
FROM (VALUES
        (N'Cálculo Diferencial',             N'Laura Gómez'),
        (N'Álgebra Lineal',                  N'Laura Gómez'),
        (N'Programación I',                  N'Carlos Rodríguez'),
        (N'Estructuras de Datos',            N'Carlos Rodríguez'),
        (N'Bases de Datos',                  N'Diana Martínez'),
        (N'Ingeniería de Software',          N'Diana Martínez'),
        (N'Metodología de la Investigación', N'Andrés López'),
        (N'Proyecto de Vida',                N'Andrés López'),
        (N'Estadística',                     N'Sofía Herrera'),
        (N'Inglés I',                        N'Sofía Herrera')
     ) AS m (Nombre, Profesor)
JOIN dbo.Profesor p ON p.Nombre = m.Profesor;

-- Plan de Ingeniería de Sistemas: incluye las 10 materias.
-- CROSS JOIN combina el programa con todas las materias.
INSERT INTO dbo.PlanEstudios (IdPrograma, IdMateria)
SELECT pr.IdPrograma, m.IdMateria
FROM dbo.Programa pr
CROSS JOIN dbo.Materia m
WHERE pr.Nombre = N'Ingeniería de Sistemas';

-- Plan de Administración de Empresas: 6 materias.
-- Se eligieron de 4 profesores distintos para que un estudiante de este
-- programa siempre pueda armar sus 3 materias sin repetir profesor.
-- Varias se comparten con Sistemas, lo que demuestra la relación
-- muchos a muchos.
INSERT INTO dbo.PlanEstudios (IdPrograma, IdMateria)
SELECT pr.IdPrograma, m.IdMateria
FROM dbo.Programa pr
JOIN dbo.Materia m ON m.Nombre IN (
        N'Cálculo Diferencial',
        N'Bases de Datos',
        N'Metodología de la Investigación',
        N'Proyecto de Vida',
        N'Estadística',
        N'Inglés I')
WHERE pr.Nombre = N'Administración de Empresas';

COMMIT TRANSACTION;
GO
