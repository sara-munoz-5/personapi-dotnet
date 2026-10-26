USE persona_db;
GO

IF NOT EXISTS (SELECT 1 FROM persona WHERE cc = 123456)
BEGIN
    INSERT INTO persona (cc, nombre, apellido, genero, edad)
    VALUES (123456, 'Sara', 'Muñoz', 'F', 21);
END;

IF NOT EXISTS (SELECT 1 FROM profesion WHERE id = 1)
BEGIN
    INSERT INTO profesion (id, nom, des)
    VALUES (1, 'Ingeniería de Sistemas',
            'Desarrollo y gestión de sistemas informáticos');
END;
GO