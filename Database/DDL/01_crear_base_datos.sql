-- Crea la base de datos del laboratorio si no existe.
-- Ejecutar antes de 02_crear_tablas.sql.
IF DB_ID(N'persona_db') IS NULL
    CREATE DATABASE persona_db;
GO
