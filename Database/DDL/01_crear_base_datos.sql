-- Crea la base de datos del laboratorio si no existe.
-- Ejecutar antes de 02_crear_tablas.sql.

IF DB_ID(N'persona_db') IS NULL
    CREATE DATABASE persona_db;
GO

-- Asigna el usuario sa como propietario de la base de datos.
ALTER AUTHORIZATION ON DATABASE::persona_db TO sa;
GO

-- Verifica quién es el propietario.
SELECT
    name,
    SUSER_SNAME(owner_sid) AS propietario
FROM sys.databases
WHERE name = 'persona_db';
GO