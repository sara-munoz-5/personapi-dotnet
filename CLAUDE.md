# CLAUDE.md — personapi-dotnet

Laboratorio 1 de Arquitectura de Software (PUJ): monolito ASP.NET Core MVC con patrones MVC y DAO.
Fuente de verdad: `docs/Laboratorio1_personapi-dotnet_Investigacion_completa.pdf` (manda sobre
`docs/Integrante3_Interfaz_MVC_personapi-dotnet.pdf` si difieren). Las secciones citadas abajo son de ese documento.

## Stack (sección 1.4)
- .NET 10 LTS (`net10.0`), plantilla MVC sin autenticación y sin HTTPS.
- EF Core 10.0.x: `Microsoft.EntityFrameworkCore`, `.SqlServer`, `.Tools`.
- Swashbuckle.AspNetCore 10.x, documento **OpenAPI 3.0** (no 3.1), Swagger UI en `/swagger`.
- SQL Server 2025 Express, instancia `localhost\SQLEXPRESS`, base `persona_db` con dueño `sa`.
- Bootstrap 5 y jQuery Validation (vienen en la plantilla).

## Estructura (sección 4.2) — un solo proyecto, capas por carpeta
- `Models/Entities/`: entidades y `PersonaDbContext` generados por Scaffold-DbContext. **No editar a mano**; hoy son provisionales.
- `Interfaces/` (`I<Entidad>Repository`) y `Repository/` (`<Entidad>Repository`): DAO.
- `Controllers/` (`<Entidad>Controller`, MVC) y `Controllers/Api/` (`<Entidad>ApiController`, REST).
- `ViewModels/` (`EstudioFormViewModel`, `TelefonoFormViewModel`) y `Views/<Entidad>/` (Index, Details, Create, Edit, Delete).
- Un controlador MVC y un controlador API **por entidad**; nunca un controlador único.
- Los controladores reciben solo interfaces DAO, nunca el DbContext. Los MVC no llaman a la API.

### Agregados aprobados que no están en el documento
- `.editorconfig`, `global.json` (SDK 10.0.100, `rollForward: latestFeature`), este `CLAUDE.md`.
- `Models/Metadata/`: clases `[ModelMetadataType]` con las validaciones de la Tabla 19, para no tocar las entidades generadas.

## Modelo de datos (sección 4.4)
- `profesion(id PK, nom VARCHAR(90), des VARCHAR(MAX) NULL)`
- `persona(cc PK, nombre VARCHAR(45), apellido VARCHAR(45), genero CHAR(1) CHECK M/F, edad INT NULL)`
- `estudios(id_prof, cc_per PK compuesta y FK, fecha DATE NULL, univer VARCHAR(50) NULL)`
- `telefono(num VARCHAR(15) PK, oper VARCHAR(45), duenio INT FK a persona.cc)`
- Llaves sin IDENTITY; las 3 FK con `ON DELETE CASCADE`. El DML inserta en orden profesion, persona, estudios, telefono.

## Contrato DAO (sección 5.3) — exactamente 5 operaciones asíncronas
`GetAllAsync`, `GetByIdAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`. **No existe `ExistsAsync`.**
- Estudio usa la llave `(int idProf, int ccPer)`; Teléfono, `string num`.
- `GetAllAsync` de Estudio y Teléfono incluye sus relaciones; `GetByIdAsync` de Persona incluye estudios y teléfonos, y el de Profesión incluye sus estudios.
- Llave repetida: se revisa con `GetByIdAsync` (no nulo = existe) y además se captura la excepción de `AddAsync` (MVC: `ModelState.AddModelError`; API: 409).

## API REST (secciones 4.5 y 5.1)
- Rutas: `api/personas/{cc}`, `api/profesiones/{id}`, `api/estudios/{idProf}/{ccPer}`, `api/telefonos/{num}`.
- GET todos 200 · GET uno 200/404 · POST 201 (CreatedAtAction)/409 · PUT 204/400 (id distinto)/404 · DELETE 204/404.
- JSON con `ReferenceHandler.IgnoreCycles`; comentarios XML y `[ProducesResponseType]` en cada endpoint.

## Interfaz MVC (secciones 6.3 a 6.7)
- 8 acciones async por controlador; `[ValidateAntiForgeryToken]` en los POST; PRG con mensaje en `TempData`.
- `[Bind]` en Create; en Edit se carga la entidad y se actualizan solo los campos permitidos.
- Estudio: enlaces con `asp-route-idProf` y `asp-route-ccPer`; persona y profesión de solo lectura en Edit.

## Reglas de git (sección 5.4)
- Ramas: `chore/estructura-inicial` (base), `feature/persistencia` (integrante 1), `feature/api-rest` (integrante 2), `feature/mvc` (integrante 3).
- `main` siempre compila; se integra por pull request revisado. Nunca hacer push sin que lo pidan.
- Conventional Commits en español (`feat(mvc): agrega CRUD de personas`); no subir `bin`, `obj` ni `.vs`.
- Cambiar una interfaz DAO exige avisar a los otros dos integrantes.

## Convenciones
- Comentarios y textos visibles en español.
- Compilar: `dotnet build personapi-dotnet.sln`.
