# Contratos de desarrollo — personapi-dotnet

**Proyecto:** Monolito MVC + DAO — CRUD de personas, profesiones, estudios y teléfonos  
**Tecnologías acordadas:** ASP.NET Core MVC (.NET 10 LTS), C#, Entity Framework Core 10, SQL Server 2022, API REST y OpenAPI 3 con Swagger UI.  
**Estado:** Versión 3 — estructura de controladores confirmada por el profesor; pendiente de aprobación por los tres integrantes y de decisiones de datos indicadas en este documento.  
**Objetivo:** Fijar nombres, tipos, operaciones, rutas, responsabilidades y criterios de prueba antes del desarrollo en paralelo.

> **Regla de equipo:** una vez aprobados, estos contratos no se modifican unilateralmente. Cualquier cambio que afecte entidades, firmas de interfaces, DTO, rutas o respuestas HTTP debe revisarse con los tres integrantes en un pull request.

## 1. Responsabilidades del equipo (actualizadas)

| Rol | Entregables técnicos | Documentación a cargo |
|---|---|---|
| Integrante 1 — Persistencia y Repo/DAO | DDL/DML, `persona_db`, entidades EF, `DbContext`, cuatro interfaces `I...DAO` y repositorios, pruebas de persistencia | ER y adaptación SQL Server, DAO/EF, scripts e instalación, evidencias de datos |
| Integrante 2 — Service y API | Cuatro interfaces y servicios de negocio, DTOs, mapeo, validación de reglas de negocio, API REST, Swagger y pruebas HTTP | Flujo Controller → Service → Repo, SOLID, decisiones arquitectónicas, endpoints y Swagger |
| Integrante 3 — Vistas ASP.NET y Controller View | ViewModels específicos por vista, vistas Razor, controladores de vista diferenciados según la consigna, navegación, validación de interfaz e integración con Service | Vistas, ViewModels, procedimientos y capturas de formularios, consolidación editorial del documento |

**Trabajo conjunto:** diseño inicial y contratos, revisión de pull requests, pruebas integrales, README, conclusiones, referencias y TAG. Ninguna persona queda como única responsable de toda la documentación.

**Dependencias:** Integrante 1 define entidades y `I...DAO`; el 2 define `I...Service` y DTOs en acuerdo con 1 y 3; el 3 define ViewModels y controladores de vista consumiendo servicios. Integrar primero interfaces y modelos compartidos para trabajar en paralelo.

## 2. Modelo de datos acordado

El esquema proviene del diagrama del laboratorio. Los nombres de **tablas y columnas SQL** se conservan para facilitar la comparación con el enunciado. Los nombres **C#** siguen PascalCase.

### 2.1 Tablas y columnas

| Tabla SQL | Columna | Tipo SQL Server propuesto | Restricción/función | Propiedad C# |
|---|---|---|---|---|
| `persona` | `cc` | `INT` | PK, NOT NULL | `Cc` (`int`) |
| `persona` | `nombre` | `VARCHAR(45)` | NOT NULL | `Nombre` (`string`) |
| `persona` | `apellido` | `VARCHAR(45)` | NOT NULL | `Apellido` (`string`) |
| `persona` | `genero` | `CHAR(1)` | NOT NULL; CHECK (`M`, `F`) | `Genero` (`string`) |
| `persona` | `edad` | `INT` | NOT NULL; rango a validar | `Edad` (`int`) |
| `profesion` | `id` | `INT` | PK, NOT NULL | `Id` (`int`) |
| `profesion` | `nom` | `VARCHAR(90)` | NOT NULL | `Nom` (`string`) |
| `profesion` | `des` | `VARCHAR(MAX)` | NULL | `Des` (`string?`) |
| `estudios` | `id_prof` | `INT` | PK (parte 1), FK a `profesion.id` | `IdProf` (`int`) |
| `estudios` | `cc_per` | `INT` | PK (parte 2), FK a `persona.cc` | `CcPer` (`int`) |
| `estudios` | `fecha` | `DATE` | NULL | `Fecha` (`DateOnly?`)* |
| `estudios` | `univer` | `VARCHAR(50)` | NULL | `Univer` (`string?`) |
| `telefono` | `num` | `VARCHAR(15)` | PK, NOT NULL | `Num` (`string`) |
| `telefono` | `oper` | `VARCHAR(45)` | NOT NULL | `Oper` (`string`) |
| `telefono` | `duenio` | `INT` | FK a `persona.cc`, NOT NULL | `Duenio` (`int`) |

\* Para `DATE`, Entity Framework Core 10 puede mapear `DateOnly`; usar el tipo efectivamente generado por **Scaffold-DbContext** y ajustar los contratos de forma coordinada si difiere.

**Decisiones necesarias antes de crear DDL:** confirmar con el profesor si `profesion.id` es `IDENTITY(1,1)` (propuesta: sí); si `persona.cc` se ingresa manualmente (propuesta: sí); y si los campos que el diagrama no marca explícitamente como obligatorios se permitirán NULL. La tabla anterior es la **propuesta de implementación**, no una afirmación de que el diagrama defina nulabilidad.

### 2.2 Claves y relaciones

- `persona`: PK `cc`.
- `profesion`: PK `id`.
- `telefono`: PK `num`; FK `duenio` → `persona.cc`. Una persona puede tener varios teléfonos.
- `estudios`: **PK compuesta (`id_prof`, `cc_per`)**; FK `id_prof` → `profesion.id` y FK `cc_per` → `persona.cc`.
- La PK de `estudios` **impide dos registros con la misma combinación persona–profesión**, aunque cambien la universidad o fecha. No agregar un campo `id` ni cambiar esta clave sin autorización.
- **Propuesta de borrado:** `ON DELETE NO ACTION` en las tres FK. No eliminar automáticamente estudios o teléfonos al borrar una persona, ni estudios al borrar una profesión.

**Adaptaciones de MySQL a SQL Server:** `ENUM('M','F')` → `CHAR(1)` + `CHECK`; `TEXT` → `VARCHAR(MAX)`; `INT(15)` del diagrama no implica 15 dígitos en SQL Server: se usa `INT`. Confirmar que las cédulas de prueba caben en el rango de `INT`; de no ser así, el equipo debe acordar cambiar `cc`, `cc_per` y `duenio` conjuntamente a `BIGINT`.

## 3. Arquitectura y convenciones del código (actualizadas)

**Decisión arquitectónica del equipo:** tanto los Controller View como los API Controller llaman **directamente a interfaces Service**, no a otros controladores ni directamente a Repo. Los Services dependen de interfaces DAO/Repo y los repositorios usan Entity Framework Core. Esto evita duplicar lógica y facilita aplicar SOLID y pruebas unitarias.

- Flujo web: Vista Razor → Controller View → Service → Repo/DAO → EF Core → SQL Server.
- Flujo externo: Cliente HTTP → API Controller → Service → Repo/DAO → EF Core → SQL Server.
- ViewModel = datos específicos de **una vista** (formularios, listas, mensajes, selectores). No usar entidades EF como ViewModel.
- DTO = datos de entrada/salida de la API o frontera de servicios; no exponer entidades EF directamente en JSON.
- Modelo/Entity = estructura de datos persistida, con relaciones; el modelo no ejecuta consultas.
- DAO y Repo se consideran equivalentes en este laboratorio; no crear dos capas redundantes.
- La BD y los archivos DDL/DML van en `database/` **fuera del proyecto web** (pero dentro de la raíz del repositorio para poder entregarlos).
- Aunque el profesor describió la lógica interna dentro de Controller, organizamos físicamente Services y Repositories en carpetas propias **dentro de la misma aplicación monolítica**. No significa microservicios ni proyectos separados.

Estructura propuesta del repositorio (un solo proyecto ASP.NET Core):

```text
personapi-dotnet/
├── database/                     # Fuera del proyecto web
│   ├── 01_ddl.sql
│   └── 02_dml.sql
├── docs/
│   ├── contratos.md
│   └── informe/                   # Documento y evidencias
├── personapi-dotnet/             # Proyecto web MVC + API, monolito
│   ├── Models/
│   │   ├── Entities/             # Entidades generadas o mapeadas por EF Core
│   │   ├── ViewModels/           # Uno por cada vista; ver sección 6
│   │   └── DTOs/                 # Request y Response para API
│   ├── Controllers/
│   │   ├── View/                 # Controladores por vista
│   │   └── Api/                  # Entrada REST (superficie API maestra)
│   ├── Services/
│   │   ├── Interfaces/
│   │   └── Implementations/
│   ├── Repositories/
│   │   ├── Interfaces/           # Contratos DAO
│   │   └── Implementations/      # DAO EF Core
│   ├── Data/                     # DbContext y configuración del ORM
│   ├── Views/                    # Archivos Razor .cshtml
│   ├── Program.cs
│   └── appsettings.json
├── README.md
└── contratos.md                  # Este archivo, si lo prefieren en la raíz
```

- Espacio de nombres de C#: `PersonaApiDotnet`; repositorio GitHub `personapi-dotnet`.
- Entidades: `Persona`, `Profesion`, `Estudios`, `Telefono`; mantener el tipo generado por scaffold y validar consistencia antes de codificar.
- Interfaces con prefijo `I`; métodos async de persistencia y negocio terminan en `Async`.
- `DbContext`, repositorios y servicios registrados con inyección de dependencias (`Scoped`).
- Los endpoints API retornan JSON y códigos HTTP; las acciones web retornan HTML/redirect.

## 4. Contratos DAO — firmas obligatorias

**Semántica compartida:** `ObtenerPorIdAsync` retorna `null` si no existe; `ActualizarAsync` y `EliminarAsync` retornan `false` cuando no existe y `true` cuando se completan; `CrearAsync` retorna la entidad creada. Las violaciones de restricciones de base de datos (PK/FK) no deben convertirse silenciosamente en `false`: deberán manejarse y traducirse a mensajes/HTTP apropiados en una capa superior.

> Estos ejemplos asumen el namespace de entidades `PersonaApiDotnet.Models.Entities`. Ajustar únicamente el `using` si el scaffold genera otro namespace; mantener las firmas tras acordarlas.

### 4.1 `IPersonaDAO`

```csharp
using PersonaApiDotnet.Models.Entities;

public interface IPersonaDAO
{
    Task<List<Persona>> ObtenerTodosAsync();
    Task<Persona?> ObtenerPorIdAsync(int cc);
    Task<Persona> CrearAsync(Persona persona);
    Task<bool> ActualizarAsync(Persona persona);
    Task<bool> EliminarAsync(int cc);
}
```

### 4.2 `IProfesionDAO`

```csharp
using PersonaApiDotnet.Models.Entities;

public interface IProfesionDAO
{
    Task<List<Profesion>> ObtenerTodosAsync();
    Task<Profesion?> ObtenerPorIdAsync(int id);
    Task<Profesion> CrearAsync(Profesion profesion);
    Task<bool> ActualizarAsync(Profesion profesion);
    Task<bool> EliminarAsync(int id);
}
```

### 4.3 `IEstudiosDAO`

```csharp
using PersonaApiDotnet.Models.Entities;

public interface IEstudiosDAO
{
    Task<List<Estudios>> ObtenerTodosAsync();
    Task<Estudios?> ObtenerPorIdAsync(int idProf, int ccPer);
    Task<Estudios> CrearAsync(Estudios estudio);
    Task<bool> ActualizarAsync(Estudios estudio);
    Task<bool> EliminarAsync(int idProf, int ccPer);
}
```

**Regla:** `ActualizarAsync` solo modifica `Fecha` y `Univer`; no cambia los componentes de la PK. Para cambiar la asociación persona–profesión se elimina el registro anterior y se crea otro, verificando restricciones.

### 4.4 `ITelefonoDAO`

```csharp
using PersonaApiDotnet.Models.Entities;

public interface ITelefonoDAO
{
    Task<List<Telefono>> ObtenerTodosAsync();
    Task<Telefono?> ObtenerPorIdAsync(string num);
    Task<Telefono> CrearAsync(Telefono telefono);
    Task<bool> ActualizarAsync(Telefono telefono);
    Task<bool> EliminarAsync(string num);
}
```

**Regla:** `Num` es `string` porque el número es un identificador, no un dato para operar matemáticamente. `ActualizarAsync` no cambia `Num`; sí puede modificar `Oper` o `Duenio`, siempre que la nueva persona exista.

## 4B. Contratos de Service — frontera obligatoria

Se mantienen las cuatro interfaces DAO de la sección 4; **los controladores ya no las inyectan directamente**. Los Services concentran validaciones y reglas de negocio compartidas por API y vistas. Propuesta de nombres y operaciones:

```csharp
public interface IPersonaService
{
    Task<List<PersonaDto>> ObtenerTodosAsync();
    Task<PersonaDto?> ObtenerPorIdAsync(int cc);
    Task<PersonaDto> CrearAsync(PersonaCrearDto dto);
    Task<bool> ActualizarAsync(int cc, PersonaActualizarDto dto);
    Task<bool> EliminarAsync(int cc);
}
```

Definir de forma análoga `IProfesionService`, `IEstudiosService` e `ITelefonoService`; para estudios, las operaciones por ID reciben **`(int idProf, int ccPer)`**. Especificar DTO de Crear, Actualizar y Respuesta para cada entidad. Las validaciones de negocio (existencia de persona/ profesión, claves compuestas, conflictos y restricciones de borrado) se aplican aquí. Los controladores traducen resultados/errores a respuestas HTTP o mensajes HTML.

**Pendiente de aprobación:** concretar las firmas DTO de las otras tres interfaces y el mecanismo uniforme de errores (resultado tipado o excepciones de negocio controladas) antes de implementar.

## 5. Contrato REST

- Base: `/api`.
- JSON en camelCase, por ejemplo `{ "cc": 123456, "nombre": "Sara" }`.
- Peticiones POST y PUT con `Content-Type: application/json`.
- Convención de controladores: `PersonasApiController`, `ProfesionesApiController`, `EstudiosApiController`, `TelefonosApiController`.
- Usar DTO de entrada para alta/edición y DTO de salida para lectura. Los DTO deben representar los campos de cada entidad; **no incluir objetos de navegación EF** en las respuestas.
- Cuando el recurso ya tiene ID conocido (como `persona.cc` y `telefono.num`), POST recibe el ID en el cuerpo. En `profesion`, si `id` es `IDENTITY`, el POST lo omite.

### 5.1 Endpoints

| Entidad | Método | Ruta | Acción |
|---|---|---|---|
| Persona | GET | `/api/personas` | Listar |
| Persona | GET | `/api/personas/{cc}` | Buscar por cédula |
| Persona | POST | `/api/personas` | Crear |
| Persona | PUT | `/api/personas/{cc}` | Actualizar (sin modificar cédula) |
| Persona | DELETE | `/api/personas/{cc}` | Eliminar |
| Profesión | GET | `/api/profesiones` | Listar |
| Profesión | GET | `/api/profesiones/{id}` | Buscar por ID |
| Profesión | POST | `/api/profesiones` | Crear |
| Profesión | PUT | `/api/profesiones/{id}` | Actualizar |
| Profesión | DELETE | `/api/profesiones/{id}` | Eliminar |
| Estudios | GET | `/api/estudios` | Listar |
| Estudios | GET | `/api/estudios/{idProf}/{ccPer}` | Buscar por PK compuesta |
| Estudios | POST | `/api/estudios` | Crear |
| Estudios | PUT | `/api/estudios/{idProf}/{ccPer}` | Actualizar fecha/universidad |
| Estudios | DELETE | `/api/estudios/{idProf}/{ccPer}` | Eliminar |
| Teléfono | GET | `/api/telefonos` | Listar |
| Teléfono | GET | `/api/telefonos/{num}` | Buscar por número |
| Teléfono | POST | `/api/telefonos` | Crear |
| Teléfono | PUT | `/api/telefonos/{num}` | Actualizar operador/propietario |
| Teléfono | DELETE | `/api/telefonos/{num}` | Eliminar |

### 5.2 Estructuras JSON propuestas

**POST `/api/personas`** (todos los campos obligatorios según las restricciones acordadas):

```json
{
  "cc": 123456,
  "nombre": "Sara",
  "apellido": "Muñoz",
  "genero": "F",
  "edad": 21
}
```

**POST `/api/profesiones`** (`id` generado por SQL Server si se aprueba `IDENTITY`):

```json
{ "nom": "Ingeniería de sistemas", "des": "Profesión de ejemplo" }
```

**POST `/api/estudios`**:

```json
{
  "idProf": 1,
  "ccPer": 123456,
  "fecha": "2026-10-20",
  "univer": "Universidad de ejemplo"
}
```

**POST `/api/telefonos`**:

```json
{ "num": "3001234567", "oper": "Operador ejemplo", "duenio": 123456 }
```

**Para PUT:** enviar únicamente los campos editables del recurso en el cuerpo y usar la clave identificadora de la ruta (Persona: `nombre`, `apellido`, `genero`, `edad`; Profesión: `nom`, `des`; Estudios: `fecha`, `univer`; Teléfono: `oper`, `duenio`). Esto debe quedar reflejado en los respectivos DTO de actualización.

### 5.3 Respuestas y errores

| Caso | Código | Respuesta |
|---|---|---|
| GET de colección exitoso | `200 OK` | Arreglo JSON (`[]` si está vacío) |
| GET individual existente | `200 OK` | Objeto JSON |
| POST exitoso | `201 Created` | Objeto creado y cabecera `Location` |
| PUT exitoso | `204 No Content` | Sin cuerpo |
| DELETE exitoso | `204 No Content` | Sin cuerpo |
| ID no encontrado en GET/PUT/DELETE | `404 Not Found` | `ProblemDetails` o `ValidationProblemDetails` |
| Datos inválidos | `400 Bad Request` | Detalles de validación |
| PK duplicada / conflicto de unicidad | `409 Conflict` | Detalle breve del conflicto |
| FK inexistente o borrado bloqueado por referencias | `409 Conflict` | Detalle breve, sin datos internos de SQL |
| Error inesperado | `500 Internal Server Error` | Respuesta genérica; detalle solo en logs |

Para la API, estandarizar errores con **ProblemDetails** y no devolver excepciones SQL completas. Los valores de claves de la ruta no se cambian en un PUT.

## 6. Contratos MVC — **un Controller View y un ViewModel por vista**

El profesor indicó expresamente **un controlador de vista por cada vista y un ViewModel por cada vista**. Se adopta esta interpretación literal para la entrega, aunque no es la organización MVC habitual de ASP.NET Core (que normalmente agrupa varias acciones en un controlador por entidad). El profesor confirmó que la correspondencia es literalmente 1:1 por archivo de vista; esta es una restricción obligatoria de la entrega.

Por entidad se proponen cinco vistas: `Index`, `Details`, `Create`, `Edit` y `Delete`. Para cada archivo `.cshtml` habrá su propio `...ViewModel` y `...ViewController` (o `...Controller`, con nombre coherente). Esto implica **20 vistas + 20 ViewModels + 20 Controller View** si se mantienen las cinco pantallas separadas. Los GET y POST correspondientes a una misma vista pueden quedar en su propio Controller View; no duplicar uno solo para cada método HTTP.

Ejemplo para Personas:

| Vista Razor | ViewModel | Controller View | Acciones |
|---|---|---|---|
| `Views/Personas/Index.cshtml` | `PersonaIndexViewModel` | `PersonaIndexController` | GET lista |
| `Views/Personas/Details.cshtml` | `PersonaDetailsViewModel` | `PersonaDetailsController` | GET detalle |
| `Views/Personas/Create.cshtml` | `PersonaCreateViewModel` | `PersonaCreateController` | GET formulario; POST crear |
| `Views/Personas/Edit.cshtml` | `PersonaEditViewModel` | `PersonaEditController` | GET formulario; POST actualizar |
| `Views/Personas/Delete.cshtml` | `PersonaDeleteViewModel` | `PersonaDeleteController` | GET confirmación; POST eliminar |

El mismo patrón aplica a `Profesion`, `Estudios` y `Telefono`. Se conservarán rutas públicas amigables, por ejemplo `/Personas`, `/Personas/Create`, `/Personas/Edit/123456`, mediante rutas explícitas/atributos si el nombre físico del controlador cambia. Para estudios, la clave de URL contiene `idProf` y `ccPer`.

**Reglas:**
- Todos los Controller View inyectan `I...Service`, **nunca directamente `I...DAO`**.
- Los ViewModels llevan solo los campos que requiere su vista; no son la misma clase que los DTO o las entidades EF.
- `Create` y `Edit` validan `ModelState`; los POST usan `[ValidateAntiForgeryToken]`.
- `Estudios/Create`: seleccionar persona y profesión existentes; en `Edit` no cambiar las dos partes de su PK.
- `Telefono/Create`: seleccionar dueño existente; en `Edit` mantener la clave `num` de solo lectura.
- Para eliminación con referencias, mostrar una validación comprensible (sin cascada destructiva).
- Un API Controller no es un paso intermedio de la navegación MVC: **ambos tipos de controller consumen Services**, no se invocan entre sí.

**API Controller maestro:** la aplicación tendrá un punto de entrada lógico REST bajo `/api/`. Para respetar responsabilidad única, se recomienda dividir físicamente en `PersonasApiController`, `ProfesionesApiController`, `EstudiosApiController` y `TelefonosApiController`, en vez de un controlador gigante. Según la aclaración disponible, se permite distribuir la superficie REST en cuatro controladores por entidad; esta es la decisión arquitectónica adoptada por el equipo.

## 7. Validaciones mínimas

| Entidad | Reglas |
|---|---|
| Persona | `cc` positivo y dentro de `INT`; nombre/apellido no vacíos y hasta 45 caracteres; `genero` `M` o `F`; `edad` entre 0 y 130 (rango propuesto) |
| Profesión | `nom` no vacío y hasta 90 caracteres; `des` opcional |
| Estudios | La profesión y la persona deben existir; combinación (`id_prof`, `cc_per`) única; universidad máximo 50 caracteres; fecha válida cuando se suministre |
| Teléfono | `num` no vacío, máximo 15 caracteres y único; `oper` no vacío, máximo 45 caracteres; `duenio` debe existir |

**Nota:** si se desea una regla más estricta para formato de teléfono, fecha o cédula, aprobarla entre todos antes de codificar. No alterar arbitrariamente el modelo que dio el profesor.

## 8. Pruebas de aceptación mínimas

Para **cada una** de las cuatro entidades, comprobar desde Swagger o cliente HTTP y, cuando aplique, desde vistas Razor:

1. GET colección inicialmente vacía o con datos de prueba.
2. POST válido → `201` y registro consultable.
3. GET por clave existente → `200` con valores correctos.
4. PUT válido → `204` y datos efectivamente modificados.
5. DELETE válido → `204` y GET posterior → `404`.
6. GET de clave inexistente → `404`.
7. POST con datos inválidos → `400`.
8. POST con PK repetida → `409`.
9. Inserción con FK inexistente y borrado con referencias → `409`.
10. Para `estudios`, comprobar lectura/edición/eliminación mediante **ambos componentes** de su clave.

Usar exclusivamente **datos ficticios** en DML, capturas, pruebas y documentación.

## 9. Git, integración y documentación

**Repositorio público:** `personapi-dotnet`.  
**Ramas recomendadas:** `feature/data-repo`, `feature/service-api`, `feature/view-mvc`.  
**Flujo:** implementar → probar → commit → push → pull request → revisión por otro integrante → merge a `main`. No trabajar directamente sobre `main` durante el desarrollo.

**Orden de integración recomendado:**

1. Aprobar este archivo y el modelo SQL.
2. Crear solución, dependencias y estructura base (PR compartido).
3. Integrante 1 sube entidades y **contratos DAO**; integrante 2 coordina la aprobación de los **contratos Service** y DTO.
4. Integrantes 2 y 3 trabajan en API y vistas, respectivamente, contra los contratos Service; se pueden usar implementaciones simuladas mientras se integra Repo.
5. Integrar servicios y repositorios reales, ejecutar pruebas completas y revisar los contratos.
6. Entregar `README.md` (prerrequisitos, instalación, conexión, compilación, ejecución, Swagger, vistas y pruebas), scripts `database/01_ddl.sql` y `database/02_dml.sql`, código fuente y documento académico (portada, marco conceptual, diseño, procedimiento, conclusiones/lecciones y referencias).
7. Crear TAG anotado **solo** cuando todo funcione, por ejemplo `v1.0.0`, y compartir la URL del TAG en GitHub.

**Seguridad de configuración:** no subir credenciales, cadenas de conexión con contraseña, `.env`, `appsettings.Development.json` con secretos ni archivos de base de datos al repositorio. Documentar conexión local mediante autenticación de Windows o secretos de desarrollo.

## 10. Lista de aprobación del equipo

- [ ] Confirmamos SQL Server 2022 y .NET 10 LTS.
- [ ] Confirmamos que `estudios` usa PK compuesta (`id_prof`, `cc_per`).
- [ ] Confirmamos la nulabilidad y el ID autogenerado de `profesion`.
- [ ] Confirmamos los tipos y los mapeos entre SQL Server y C#.
- [ ] Confirmamos las cuatro interfaces DAO y sus firmas.
- [ ] Confirmamos las 20 rutas REST y las convenciones JSON/HTTP.
- [ ] Confirmamos cuatro contratos Service y DTO para cada operación, y el manejo de errores.
- [x] Consigna confirmada: un Controller View y un ViewModel por cada archivo de vista Razor.
- [x] Consigna aclarada: superficie API bajo `/api/`, con cuatro controladores REST separados por entidad como decisión arquitectónica.
- [ ] Confirmamos las acciones MVC, los formularios y el tratamiento de claves.
- [ ] Confirmamos las validaciones y las reglas de eliminación (`NO ACTION`).
- [ ] Confirmamos responsables de código, pruebas y documentación.

**Aprobación:** Integrante 1: ______ · Integrante 2: ______ · Integrante 3: ______ · Fecha: ______
