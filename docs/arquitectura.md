# Arquitectura del sistema

## Tipo de aplicación

`personapi-dotnet` es un monolito web ASP.NET Core. La interfaz MVC y la API REST se ejecutan en el mismo proceso y comparten Services, DAO, entidades y conexión a SQL Server.

## Capas

### Presentación MVC

Los controladores de `Controllers/ViewControllers` reciben solicitudes de los formularios Razor. Cada vista tiene su propio controlador y ViewModel. Los controladores MVC no reciben entidades EF directamente: convierten datos entre ViewModels y DTO.

### API REST

Los controladores de `Controllers/ApiControllers` exponen recursos JSON bajo `/api`. Usan `[ApiController]`, validación automática de modelos y respuestas HTTP convencionales. No contienen consultas SQL ni reglas de persistencia.

### Services

Los Services de `Controllers/Services/Implementations` implementan las interfaces de `Controllers/Services/Interfaces`. Sus responsabilidades son:

- validar claves y referencias antes de guardar;
- evitar duplicados;
- impedir eliminaciones con dependencias;
- convertir entidades en DTO;
- traducir errores de integridad a `ConflictoNegocioException`.

### DAO y Entity Framework Core

Los DAO implementan operaciones CRUD mediante `PersonaDbContext`. Usan inyección de dependencias, consultas asíncronas y `AsNoTracking` en las consultas de lectura.

### Base de datos

SQL Server contiene las tablas `persona`, `profesion`, `estudios` y `telefono`. `estudios` tiene una clave primaria compuesta por `id_prof` y `cc_per`. Las claves foráneas usan borrado sin cascada (`NO ACTION`).

## Flujo de una operación

```text
Cliente HTTP / navegador
		|
API Controller / View Controller
		|
Interface Service
		|
Service: validación y mapeo
		|
Interface DAO
		|
Entity Framework Core
		|
SQL Server
```

## Inyección de dependencias

`Program.cs` registra cada DAO y Service con ciclo de vida `Scoped`. También registra `PersonaDbContext`, los controladores MVC/API y Swagger.

## Decisiones de errores

- Consulta de un registro inexistente: `null` en el Service y `404 Not Found` en la API.
- Actualización o eliminación inexistente: `false` en el Service y `404 Not Found`.
- Duplicado, referencia inexistente o borrado con dependencias: `ConflictoNegocioException` y `409 Conflict`.
- DTO inválido: respuesta automática `400 Bad Request` de `[ApiController]`.
