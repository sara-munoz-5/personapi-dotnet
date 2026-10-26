# Pruebas de persistencia — Repositorios DAO

## 1. Objetivo

Verificar la conexión entre la aplicación ASP.NET Core y SQL Server, así como el funcionamiento de las operaciones CRUD de los cuatro repositorios implementados mediante Entity Framework Core.

## 2. Entorno de pruebas

- Framework: .NET 10.
- Base de datos: SQL Server 2022 Express.
- ORM: Entity Framework Core 10.
- Base de datos utilizada: `persona_db`.
- Patrón de persistencia: DAO (Data Access Object).

## 3. Repositorios implementados

| Repositorio | Entidad | Identificador |
|---|---|---|
| PersonaDAO | Persona | `cc` |
| ProfesionDAO | Profesion | `id` |
| EstudiosDAO | Estudio | `id_prof`, `cc_per` |
| TelefonoDAO | Telefono | `num` |

Cada repositorio implementa una interfaz y utiliza `PersonaDbContext` mediante inyección de dependencias.

## 4. Resultados de las pruebas

| Repositorio | Crear | Consultar por ID | Consultar todos | Actualizar | Eliminar |
|---|---|---|---|---|---|
| PersonaDAO | OK | OK | OK | OK | OK |
| ProfesionDAO | OK | OK | OK | OK | OK |
| EstudiosDAO | OK | OK | OK | OK | OK |
| TelefonoDAO | OK | OK | OK | OK | OK |

**Resultado:** 20 operaciones ejecutadas correctamente.

## 5. Verificación de conexión

Se verificó la conexión mediante `PersonaDbContext.Database.CanConnectAsync()`, obteniendo el mensaje `CONEXION SQL EXITOSA`.

Entity Framework Core ejecutó consultas SQL reales, incluyendo operaciones `SELECT`, `INSERT`, `UPDATE` y `DELETE`.

## 6. Consideraciones

- La entidad Estudios tiene una clave primaria compuesta.
- Las entidades Estudios y Teléfono poseen relaciones mediante claves foráneas.
- Se utilizaron registros temporales para las pruebas de escritura.
- Los registros temporales se eliminaron después de las pruebas.
- Se retiró el código de prueba temporal de `Program.cs`.

## 7. Conclusión

Las pruebas realizadas permitieron verificar el funcionamiento básico de la capa de persistencia del monolito. Los cuatro repositorios pudieron consultar, crear, modificar y eliminar registros en SQL Server.

Quedan fuera del alcance de estas pruebas los escenarios de error, los registros duplicados y las operaciones sobre entidades relacionadas cuando existen restricciones de integridad referencial.