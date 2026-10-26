# Decisiones del integrante 3 — Vistas ASP.NET y Controller View

Rama: `feature/view-mvc`, creada desde `main` en `eeb637a` (merge del PR #1 `feature/database-dao`).
Fuente de verdad: `docs/contratos (1).md`. Cuando el contrato y la estructura real de `main` difieren en nombres
o carpetas, manda la estructura de `main`; para reglas, validaciones, flujo y borrado manda el contrato.

## 1. Estado de `main` al empezar

### Archivos con código

| Área | Archivos | Dueño |
|---|---|---|
| Entidades EF (scaffold) | `Models/Entities/Estudio.cs`, `Persona.cs`, `Profesion.cs`, `Telefono.cs`, `PersonaDbContext.cs` (namespace `personapi_dotnet.Models.EntitiesGeneradas`) | Integrante 1 |
| Interfaces DAO | `Controllers/Repositories/Interfaces/I{Persona,Profesion,Estudios,Telefono}DAO.cs` | Integrante 1 |
| DAO con EF Core | `Controllers/Repositories/Implementations/{Persona,Profesion,Estudios,Telefono}DAO.cs` | Integrante 1 |
| Scripts SQL | `Database/DDL/02_crear_tablas.sql` (UTF-16), `Database/DML/01_insertar_datos.sql`, `02_insertar_datos.sql` | Integrante 1 |
| Configuración | `Program.cs` (DbContext + 4 DAO en `Scoped`), `appsettings.json` (`PersonaConnection`), `personapi-dotnet.csproj` (EF Core 10.0.14) | Compartido |
| Plantilla MVC | `Controllers/HomeController.cs`, `Views/Home/*`, `Views/Shared/*`, `wwwroot/*`, `Models/ErrorViewModel.cs` | Compartido |
| Documentación | `docs/contratos (1).md`, `docs/prueba-dao.md` | Equipo / integrante 1 |

### Archivos vacíos (0 bytes)

- `Database/DDL/01_crear_base_datos.sql` (integrante 1).
- `Controllers/ApiControllers/*ApiController.cs` (4), `Controllers/Services/Implementations/*Service.cs` (4),
  `Controllers/Services/Interfaces/I*Service.cs` (4) y `Models/DTOs/*Dto.cs` (4) — integrante 2.
- `Controllers/ViewControllers/{Entidad}/*Controller.cs` (20), `Models/ViewModels/{Entidad}/*ViewModel.cs` (20)
  y `Views/{Persona,Profesion,Estudios,Telefono}/{Listar,Detalle,Crear,Editar,Eliminar}.cshtml` (20) — integrante 3.
- `docs/arquitectura.md`, `docs/conclusiones.md`, `docs/procedimiento.md`, `docs/pruebas.md`,
  `docs/diagramas/README.md`, `docs/evidencias/README.md`.

### Lo que ya entregó el integrante 1

- Tablas `persona`, `profesion`, `estudios` (PK compuesta `id_prof, cc_per`) y `telefono`; FK **sin cascada**
  (NO ACTION, como pide el contrato §2.2); CHECK de género `M`/`F` y de edad `>= 0`.
- `profesion.id` **no es IDENTITY** (`ValueGeneratedNever`): el id se escribe a mano al crear.
- `telefono.oper` admite NULL en la base (`string?` en la entidad), aunque el contrato §2.1 lo pide NOT NULL.
- Los métodos DAO se llaman `ObtenerTodos`, `ObtenerPorId`, `Crear`, `Actualizar` y `Eliminar` (sin sufijo `Async`,
  a diferencia del contrato §4). La clase de la entidad es `Estudio` (el contrato dice `Estudios`).
- Los DAO devuelven entidades sin navegaciones (`AsNoTracking`, sin `Include`).

### Lo que falta del integrante 2

- No hay rama del integrante 2 en `origin` (solo `main`, `feature/database-dao` y las ramas viejas obsoletas).
- Faltan: interfaces y DTO con contenido, implementaciones de los 4 Services, su registro en `Program.cs`,
  los 4 API Controller y Swagger (el paquete Swashbuckle aún no está en el `.csproj`).
- Por eso propuse las interfaces y los DTO (sección 2), marcados como PROPUESTA en cada archivo.

## 2. Propuesta de interfaces de Service y DTO (contratos §4B)

- `Controllers/Services/Interfaces/I{Persona,Profesion,Estudios,Telefono}Service.cs`: cinco operaciones
  `ObtenerTodosAsync`, `ObtenerPorIdAsync`, `CrearAsync`, `ActualizarAsync(llave, dto)` y `EliminarAsync`,
  con la firma exacta de `IPersonaService` del contrato. Estudios recibe `(int idProf, int ccPer)`; Teléfono, `string num`.
- `Models/DTOs/{Entidad}Dto.cs`: en cada archivo, el DTO de respuesta, el de crear y el de actualizar
  (`PersonaDto`, `PersonaCrearDto`, `PersonaActualizarDto`, etc.). Se respetó un archivo por entidad, como en `main`.
  Los de actualizar llevan solo los campos editables del contrato §5.2.
- Los DTO tienen solo los campos de la entidad, sin navegaciones (contrato §5).
- `ProfesionCrearDto` incluye `Id` porque la tabla real no tiene IDENTITY.
- **Mecanismo de errores:** `Controllers/Services/Interfaces/ConflictoNegocioException.cs`, una sola excepción con
  `TipoConflicto` (`LlaveDuplicada`, `ReferenciaInexistente`, `BorradoConReferencias`). Se eligió la excepción
  (y no un resultado tipado) porque conserva las firmas del contrato §4B sin cambios. "No existe" no es excepción:
  `null` en consultas y `false` en actualizar y eliminar, como la semántica del contrato §4.
