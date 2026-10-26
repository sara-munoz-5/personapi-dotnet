USE persona_db;
GO

-- Registrar un estudio
IF NOT EXISTS (
    SELECT 1 FROM dbo.estudios
    WHERE id_prof = 1 AND cc_per = 123456
)
BEGIN
    INSERT INTO dbo.estudios
        (id_prof, cc_per, fecha, univer)
    VALUES
        (1, 123456, '2025-01-15', 'Javeriana');
END;

-- Registrar un teléfono
IF NOT EXISTS (
    SELECT 1 FROM dbo.telefono
    WHERE num = '3001234567'
)
BEGIN
    INSERT INTO dbo.telefono
        (num, oper, duenio)
    VALUES
        ('3001234567', 'Claro', 123456);
END;
GO