/*
   PRUEBA TÉCNICA INTER RAPIDÍSIMO - VERIFICACIÓN DEL CATÁLOGO
   Motor: SQL Server 2022

   Ejecutar después de 01_RegistroEstudiantes.sql.
   Estas consultas solo leen datos: confirman que el catálogo quedó
   cargado según las reglas del enunciado. No modifican nada.
*/

USE RegistroEstudiantes;
GO

-- Reglas 3, 4 y 6: cada profesor dicta exactamente 2 materias.
-- Resultado esperado: 5 filas, todas con Materias = 2 y Creditos = 6.
SELECT p.Nombre        AS Profesor,
       COUNT(*)        AS Materias,
       SUM(m.Creditos) AS Creditos
FROM dbo.Profesor p
JOIN dbo.Materia  m ON m.IdProfesor = p.IdProfesor
GROUP BY p.Nombre
ORDER BY p.Nombre;

-- Plan de estudios de cada programa, con el profesor de cada materia.
-- Resultado esperado: 10 filas de Ingeniería de Sistemas y 6 de
-- Administración de Empresas.
SELECT pr.Nombre AS Programa,
       m.Nombre  AS Materia,
       p.Nombre  AS Profesor
FROM dbo.PlanEstudios pe
JOIN dbo.Programa pr ON pr.IdPrograma = pe.IdPrograma
JOIN dbo.Materia  m  ON m.IdMateria   = pe.IdMateria
JOIN dbo.Profesor p  ON p.IdProfesor  = m.IdProfesor
ORDER BY pr.Nombre, p.Nombre, m.Nombre;
