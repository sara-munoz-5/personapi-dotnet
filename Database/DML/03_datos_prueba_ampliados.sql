
USE persona_db;
GO

SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    -- 1. PERSONAS
    INSERT INTO dbo.persona (cc, nombre, apellido, genero, edad)
    SELECT v.cc, v.nombre, v.apellido, v.genero, v.edad
    FROM (VALUES
        (100001, 'Laura', 'Gomez', 'F', 25),
        (100002, 'Carlos', 'Ramirez', 'M', 30),
        (100003, 'Daniela', 'Torres', 'F', 23),
        (100004, 'Andres', 'Martinez', 'M', 28),
        (100005, 'Valentina', 'Rojas', 'F', 22),
        (100006, 'Juan', 'Herrera', 'M', 35),
        (100007, 'Camila', 'Castro', 'F', 27),
        (100008, 'Santiago', 'Moreno', 'M', 24),
        (100009, 'Natalia', 'Vargas', 'F', 32)
    ) AS v(cc, nombre, apellido, genero, edad)
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.persona p WHERE p.cc = v.cc
    );

    -- 2. PROFESIONES
    INSERT INTO dbo.profesion (id, nom, des)
    SELECT v.id, v.nom, v.des
    FROM (VALUES
        (2, 'Medicina', 'Atencion y cuidado de pacientes'),
        (3, 'Derecho', 'Asesoria y representacion juridica'),
        (4, 'Administracion de Empresas', 'Gestion de organizaciones'),
        (5, 'Psicologia', 'Estudio del comportamiento humano'),
        (6, 'Arquitectura', 'Diseno de espacios y edificaciones'),
        (7, 'Contaduria Publica', 'Gestion contable y financiera'),
        (8, 'Ingenieria Industrial', 'Optimizacion de procesos')
    ) AS v(id, nom, des)
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.profesion p WHERE p.id = v.id
    );

    -- 3. ESTUDIOS
    INSERT INTO dbo.estudios (id_prof, cc_per, fecha, univer)
    SELECT v.id_prof, v.cc_per, v.fecha, v.univer
    FROM (VALUES
        (1, 100001, CONVERT(date, '2023-06-15'), 'Universidad Javeriana'),
        (2, 100002, CONVERT(date, '2020-12-10'), 'Universidad Nacional'),
        (3, 100003, CONVERT(date, '2025-06-20'), 'Universidad del Rosario'),
        (4, 100004, CONVERT(date, '2022-11-15'), 'Universidad de los Andes'),
        (5, 100005, CONVERT(date, '2026-06-10'), 'Universidad Javeriana'),
        (6, 100006, CONVERT(date, '2018-12-05'), 'Universidad Nacional'),
        (7, 100007, CONVERT(date, '2021-06-20'), 'Universidad Externado'),
        (8, 100008, CONVERT(date, '2024-11-22'), 'Universidad de La Sabana'),
        (1, 100009, CONVERT(date, '2019-06-15'), 'Universidad Javeriana'),
        (4, 100001, CONVERT(date, '2025-11-20'), 'Universidad EAN'),
        (8, 100002, CONVERT(date, '2022-06-20'), 'Universidad de los Andes')
    ) AS v(id_prof, cc_per, fecha, univer)
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.estudios e
        WHERE e.id_prof = v.id_prof AND e.cc_per = v.cc_per
    )
    AND EXISTS (SELECT 1 FROM dbo.profesion p WHERE p.id = v.id_prof)
    AND EXISTS (SELECT 1 FROM dbo.persona p WHERE p.cc = v.cc_per);

    -- 4. TELEFONOS
    INSERT INTO dbo.telefono (num, oper, duenio)
    SELECT v.num, v.oper, v.duenio
    FROM (VALUES
        ('3100000001', 'Claro', 100001),
        ('3100000002', 'Movistar', 100002),
        ('3100000003', 'Tigo', 100003),
        ('3100000004', 'Claro', 100004),
        ('3100000005', 'WOM', 100005),
        ('3100000006', 'Movistar', 100006),
        ('3100000007', 'Tigo', 100007),
        ('3100000008', 'Claro', 100008),
        ('3100000009', 'WOM', 100009),
        ('3200000001', 'Movistar', 100001),
        ('3200000002', 'Claro', 100002)
    ) AS v(num, oper, duenio)
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.telefono t WHERE t.num = v.num
    )
    AND EXISTS (SELECT 1 FROM dbo.persona p WHERE p.cc = v.duenio);

    COMMIT TRANSACTION;
    PRINT 'DATOS DE PRUEBA INSERTADOS CORRECTAMENTE';

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO

-- Verificar cantidades finales
SELECT 'Personas' AS Tabla, COUNT(*) AS Total FROM dbo.persona
UNION ALL
SELECT 'Profesiones', COUNT(*) FROM dbo.profesion
UNION ALL
SELECT 'Estudios', COUNT(*) FROM dbo.estudios
UNION ALL
SELECT 'Telefonos', COUNT(*) FROM dbo.telefono;
