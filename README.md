# personapi-dotnet

Laboratorio 1 de Arquitectura de Software — Pontificia Universidad Javeriana.
Monolito ASP.NET Core MVC con patrones MVC y DAO que expone vistas web y una API REST para el CRUD de
`profesion`, `persona`, `estudios` y `telefono`, sobre EF Core y SQL Server, documentada con Swagger (OpenAPI 3.0).

## Integrantes

| Integrante | Bloque | Carpetas |
|---|---|---|
| Integrante 1 | Persistencia | `scripts/`, `Models/Entities/`, `Interfaces/`, `Repository/` |
| Integrante 2 | Servicios REST | `Controllers/Api/` |
| Integrante 3 (Samuel) | Interfaz MVC | `Controllers/`, `ViewModels/`, `Views/`, `wwwroot/` |

## Requisitos

| Herramienta | Versión |
|---|---|
| .NET SDK | 10.0.x (fijado en `global.json`) |
| SQL Server Express | 2025, instancia `localhost\SQLEXPRESS` |
| SQL Server Management Studio | 22 |
| Visual Studio Community | 2026 (opcional; basta la CLI `dotnet`) |
| EF Core | 10.0.x |
| Swashbuckle.AspNetCore | 10.x |

## Base de datos

1. Ejecutar `scripts/01_ddl.sql` en SSMS: crea `persona_db`, le asigna el dueño `sa` y crea las tablas.
2. Ejecutar `scripts/02_dml.sql`: inserta los datos de prueba.

Cadena de conexión (`personapi-dotnet/appsettings.json`, nombre `PersonaDb`):

```
Server=localhost\SQLEXPRESS;Database=persona_db;Trusted_Connection=True;TrustServerCertificate=true
```

## Compilar y ejecutar

```bash
dotnet build personapi-dotnet.sln
dotnet run --project personapi-dotnet
```

- Aplicación: http://localhost:5124
- Swagger UI: http://localhost:5124/swagger

## Endpoints REST

| Recurso | GET todos | GET uno | POST | PUT | DELETE |
|---|---|---|---|---|---|
| Personas | `/api/personas` | `/api/personas/{cc}` | `/api/personas` | `/api/personas/{cc}` | `/api/personas/{cc}` |
| Profesiones | `/api/profesiones` | `/api/profesiones/{id}` | `/api/profesiones` | `/api/profesiones/{id}` | `/api/profesiones/{id}` |
| Estudios | `/api/estudios` | `/api/estudios/{idProf}/{ccPer}` | `/api/estudios` | `/api/estudios/{idProf}/{ccPer}` | `/api/estudios/{idProf}/{ccPer}` |
| Teléfonos | `/api/telefonos` | `/api/telefonos/{num}` | `/api/telefonos` | `/api/telefonos/{num}` | `/api/telefonos/{num}` |

Códigos: 200 consulta, 201 creación, 204 actualización o eliminación, 400 datos inválidos o id distinto al del cuerpo,
404 no existe, 409 llave repetida.

## Estado

Rama `chore/estructura-inicial`: estructura base.

- [x] Solución, proyecto MVC .NET 10 y paquetes (EF Core 10.0.14, Swashbuckle 10.3.1).
- [x] Entidades y `PersonaDbContext` **provisionales**, con la forma que dará Scaffold-DbContext.
- [x] Interfaces DAO con las 5 operaciones de la sección 5.3 y registro en `Program.cs`.
- [ ] **Base de datos `persona_db`** (integrante 1).
- [ ] **Scripts `01_ddl.sql` y `02_dml.sql`**: hoy solo tienen encabezado (integrante 1).
- [ ] **Scaffolding real** que reemplace `Models/Entities/` (integrante 1).
- [ ] **Repositorios**: hoy lanzan `NotImplementedException("Pendiente: integrante 1")` (integrante 1).
- [ ] API REST y Swagger: rama `feature/api-rest` (integrante 2).
- [ ] Interfaz MVC: rama `feature/mvc` (integrante 3).
