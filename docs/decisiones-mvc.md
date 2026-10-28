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

### Estado actual de los archivos

- Los archivos DAO, Services, DTO, controladores API, controladores MVC y vistas están implementados.
- La documentación consolidada se encuentra en `README.md`, `docs/arquitectura.md`, `docs/servicios.md` y `docs/api.md`.

### Lo que ya entregó el integrante 1

- Tablas `persona`, `profesion`, `estudios` (PK compuesta `id_prof, cc_per`) y `telefono`; FK **sin cascada**
  (NO ACTION, como pide el contrato §2.2); CHECK de género `M`/`F` y de edad `>= 0`.
- `profesion.id` **no es IDENTITY** (`ValueGeneratedNever`): el id se escribe a mano al crear.
- `telefono.oper` admite NULL en la base (`string?` en la entidad), aunque el contrato §2.1 lo pide NOT NULL.
- Los métodos DAO siguen las firmas contractuales `ObtenerTodosAsync`, `ObtenerPorIdAsync`, `CrearAsync`,
  `ActualizarAsync` y `EliminarAsync`. La clase generada es `Estudio` aunque el contrato conceptual use `Estudios`.
- Los DAO devuelven entidades sin navegaciones (`AsNoTracking`, sin `Include`).

### Implementación actual del integrante 2

- Los cuatro Services, DTO, controladores API y Swagger están implementados y registrados en `Program.cs`.
- Las respuestas de error API usan `ProblemDetails` y `ValidationProblemDetails` según la sección 5.3.

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

## 3. Decisiones tomadas (opción más conservadora en cada caso)

1. **Nombres y carpetas de `main`** sobre los del contrato: vistas `Listar`, `Detalle`, `Crear`, `Editar` y
   `Eliminar`; controladores `{Entidad}{Vista}Controller` en `Controllers/ViewControllers/{Entidad}/`; ViewModels
   `{Entidad}{Vista}ViewModel` en `Models/ViewModels/{Entidad}/`; namespace `personapi_dotnet` siguiendo las carpetas.
2. **Rutas por atributo en español**, iguales en GET y POST de la misma vista: `/Personas`, `/Personas/Detalle/{cc}`,
   `/Personas/Crear`, `/Personas/Editar/{cc}`, `/Personas/Eliminar/{cc}` (y lo mismo para `/Profesiones/…/{id}`,
   `/Telefonos/…/{num}` y `/Estudios/…/{idProf}/{ccPer}`). Con rutas por atributo no hizo falta tocar `Program.cs`:
   `MapControllerRoute` ya publica los controladores con atributos.
3. **Vista con ruta explícita** (`View("~/Views/Persona/Crear.cshtml", vm)`), porque el controlador no se llama como la carpeta.
4. **Los Controller View dependen solo de `I...Service` y de `ConflictoNegocioException`.** Para mostrar nombres en
   lugar de llaves (persona, profesión, dueño) inyectan además el servicio de esa entidad; no se agregaron
   navegaciones a los DTO (contratos §5).
5. **El Detalle de Persona** muestra estudios y teléfonos filtrando `ObtenerTodosAsync` de esos servicios. Funciona
   sin cambiar el contrato; un método filtrado por persona sería más eficiente (pedido opcional al integrante 2).
6. **La llave de Editar y Eliminar** sale de la URL (`[FromRoute]`) y la propiedad de la llave en el ViewModel de
   Editar tiene `[BindNever]`, para que un campo del formulario no pueda cambiar qué registro se modifica.
7. **Campos numéricos obligatorios como `int?` con `[Required]`**: el campo vacío muestra un mensaje en español en
   lugar del texto en inglés del model binder.
8. **Profesión Crear pide el id**, porque la tabla real no es IDENTITY (contrato §5: si no es IDENTITY, el id viaja en el POST).
9. **Teléfono: el operador es obligatorio** en la interfaz (contrato §2.1 y §7), aunque la columna admita NULL.
10. **Borrado sin cascada:** la vista Eliminar no consulta dependientes por su cuenta; confía en el servicio
    (`BorradoConReferencias`) y entonces muestra el error y oculta el botón Eliminar.
11. **Errores no previstos no se capturan** en los controladores: llegan a la página de error (500) en lugar de
    disfrazarse de error de validación.
12. **Género:** helper `Models/ViewModels/Persona/Generos.cs` con las opciones M/F y su texto, compartido por
    Crear, Editar, Listar, Detalle y Eliminar. No es un ViewModel; es el único archivo que no está en la relación 1:1.
13. **GET de Crear de Persona y Profesión síncronos:** no llaman a ningún servicio; marcarlos `async` sin `await`
    daría una advertencia del compilador.
14. **`Privacy` se conserva** (solo se tradujo el texto del enlace a "Privacidad").
15. **No se tocó `Program.cs`** ni nada de los integrantes 1 y 2 fuera de la propuesta de interfaces y DTO.

## 4. Diferencias con el contrato

| Tema | Contrato | Implementado | Motivo |
|---|---|---|---|
| Nombre de las vistas | `Index`, `Details`, `Create`, `Edit`, `Delete` (§6) | `Listar`, `Detalle`, `Crear`, `Editar`, `Eliminar` | Estructura real de `main` |
| Carpetas | `Controllers/View/`, `Services/`, `Repositories/` (§3) | `Controllers/ViewControllers/`, `Controllers/Services/`, `Controllers/Repositories/` | Estructura real de `main` |
| Namespace | `PersonaApiDotnet` (§3) | `personapi_dotnet` | Estructura real de `main` |
| Rutas de ejemplo | `/Personas/Create`, `/Personas/Edit/123456` (§6) | `/Personas/Crear`, `/Personas/Editar/123456` | Coherentes con los nombres de las vistas |
| Id de profesión | IDENTITY propuesto (§2.1) | Se escribe a mano | La tabla real no es IDENTITY |
| Mecanismo de errores | Pendiente de aprobación (§4B) | `ConflictoNegocioException` + `TipoConflicto` | Propuesta; mantiene las firmas del §4B |
| Interfaces y DTO de Service | Las define el integrante 2 (§1) | Propuesta del integrante 3 | No existía rama del integrante 2 |
| Helper de género | 1 ViewModel por vista (§6) | Además `Generos.cs` | Evita repetir las opciones y el texto en 5 ViewModels |

## 5. Lo que necesito del integrante 2

1. **Implementar los 4 servicios** (`Controllers/Services/Implementations/`) y **registrarlos en `Program.cs`**
   (`AddScoped<IPersonaService, PersonaService>()`, etc.). Sin ese registro `main` compila, pero las vistas
   responden 500 porque la inyección de dependencias no encuentra los servicios.
2. **Confirmar o ajustar las interfaces y los DTO propuestos.** Si cambian nombres, tipos o firmas, avisar para
   adaptar los Controller View; lo ideal es aprobar los archivos tal cual para que no haya conflictos.
3. **Traducir los errores de la base a `ConflictoNegocioException`** y no dejar escapar `DbUpdateException`:
   - `LlaveDuplicada`: cédula, id de profesión, número de teléfono o combinación (idProf, ccPer) ya existentes.
   - `ReferenciaInexistente`: persona o profesión de un estudio, dueño de un teléfono (crear y editar).
   - `BorradoConReferencias`: persona con estudios o teléfonos, profesión con estudios (las FK son NO ACTION).
4. Convertir `telefono.oper` nulo de la base en `""` en `TelefonoDto.Oper`.
5. Swagger en `/swagger`: el menú ya tiene el enlace.
6. **Opcional:** `ObtenerPorPersonaAsync(int cc)` en estudios y teléfonos, para no filtrar listas completas en el
   Detalle de Persona.
7. **Propuesta para `Program.cs` (decidir entre todos):** mensajes del model binder en español, para cuando llega
   un número o una fecha mal formados sin pasar por el navegador:

   ```csharp
   builder.Services.AddControllersWithViews(o =>
   {
       o.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((v, c) => $"El valor '{v}' no es válido para {c}.");
       o.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(c => $"El campo {c} debe ser un número.");
       o.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(v => $"El valor '{v}' no es válido.");
       o.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(c => $"Falta un valor para {c}.");
       o.ModelBindingMessageProvider.SetValueIsInvalidAccessor(v => $"El valor '{v}' no es válido.");
   });
   ```

## 6. Lo que necesito del integrante 1

1. Confirmar que `profesion.id` **no** es IDENTITY. La interfaz pide el id al crear; si pasa a IDENTITY, hay que
   quitar el campo de `ProfesionCrearViewModel` y de `ProfesionCrearDto`.
2. Alinear `telefono.oper` con el contrato (NOT NULL) o confirmar que puede ser nulo.
3. Opcional: `CHECK (edad <= 130)` para igualar la validación de la interfaz (hoy solo `edad >= 0`).
4. `Database/DDL/01_crear_base_datos.sql` está vacío y `02_crear_tablas.sql` está en UTF-16 (git lo trata como
   binario en los diffs); en UTF-8 se podría revisar en los pull requests.
5. Más datos ficticios en el DML (hoy 1 persona, 1 profesión, 1 estudio y 1 teléfono) para probar listas y borrados bloqueados.

## 7. Cierre

### Commits (`git log --oneline main..feature/view-mvc`)

Los contratos se separaron en su propia rama (sección 8): `feature/contratos-service` tiene solo el primer commit
y `feature/view-mvc` se rebasó encima, así que las dos comparten exactamente el mismo commit `8a22a41`.

```text
docs(mvc): agrega los textos de los pull requests                    ← este commit
9bbc8e8 docs(mvc): documenta vistas, ViewModels y pruebas del integrante 3
3e51d1c feat(mvc): agrega vistas Razor del CRUD y navegación
570205d feat(mvc): agrega controladores de vista por pantalla
da9f4c9 feat(mvc): agrega ViewModels por vista
8a22a41 feat(contratos): agrega interfaces de Service y DTO para las vistas   ← también en feature/contratos-service
```

No hubo commits `fix(mvc)`: la prueba de humo no encontró fallas en la aplicación (la única comprobación que falló
en la primera corrida era un error del propio script, que buscaba `<option value=…>` cuando la opción elegida sale
como `<option selected="selected" value=…>`). Todos los commits compilan con 0 advertencias y 0 errores.

### Archivos creados y modificados frente a `main`

| Archivo o carpeta | Cambio | ¿Es de mi área? |
|---|---|---|
| `Controllers/ViewControllers/{Persona,Profesion,Estudios,Telefono}/*Controller.cs` (20) | Llenados (estaban vacíos) | Sí |
| `Models/ViewModels/{Persona,Profesion,Estudios,Telefono}/*ViewModel.cs` (20) | Llenados (estaban vacíos) | Sí |
| `Models/ViewModels/Persona/Generos.cs` | Nuevo | Sí (helper de vistas) |
| `Views/{Persona,Profesion,Estudios,Telefono}/*.cshtml` (20) | Llenados (estaban vacíos) | Sí |
| `Views/Shared/_Layout.cshtml` | Menú y textos en español | Excepción permitida |
| `Views/Home/Index.cshtml` | Accesos a las cuatro listas | Excepción permitida |
| `Controllers/Services/Interfaces/I*Service.cs` (4) | Llenados: PROPUESTA | Excepción permitida (propuesta) |
| `Controllers/Services/Interfaces/ConflictoNegocioException.cs` | Nuevo: PROPUESTA | Excepción permitida (propuesta) |
| `Models/DTOs/*Dto.cs` (4) | Llenados: PROPUESTA | Excepción permitida (propuesta) |
| `docs/decisiones-mvc.md`, `docs/vistas-mvc.md` | Nuevos | Sí (mis documentos) |
| `docs/procedimiento.md`, `docs/pruebas.md` | Sección "Integrante 3 — Vistas" (estaban vacíos) | Sí (solo mi sección) |
| `docs/evidencias/mvc/*.png` (27) | Nuevos | Sí (capturas) |

Sin cambios: `Program.cs`, `.csproj`, `appsettings*.json`, `Models/Entities/`, `Controllers/Repositories/`,
`Controllers/Services/Implementations/`, `Controllers/ApiControllers/`, `Database/`, `Views/Home/Privacy.cshtml`,
`Views/_ViewImports.cshtml`, `docs/contratos (1).md` y `docs/prueba-dao.md`.

### Verificación de las reglas de los contratos §6 y §7

| Regla | Cumple | Dónde |
|---|---|---|
| Un Controller View por vista (20) | Sí | `Controllers/ViewControllers/*/` |
| Un ViewModel por vista (20) | Sí | `Models/ViewModels/*/` (más el helper `Generos.cs`) |
| GET y POST de una vista en el mismo controlador | Sí | Crear, Editar y Eliminar |
| Controller View inyectan `I...Service`, nunca `I...DAO` ni `DbContext` | Sí | Constructores de los 20 controladores |
| ViewModels distintos de DTO y entidades EF | Sí | Ninguna vista usa `Models.DTOs` ni `Models.EntitiesGeneradas` |
| Rutas amigables; Estudios con `idProf` y `ccPer` | Sí | Atributos `[HttpGet]`/`[HttpPost]`; `docs/vistas-mvc.md` §2 |
| Create y Edit validan `ModelState` | Sí | POST de Crear y Editar |
| `[ValidateAntiForgeryToken]` en los POST | Sí | 12 POST; caso 47 de la prueba (400 sin token) |
| Estudios/Create: persona y profesión existentes desde listas | Sí | `EstudiosCrearController`, `Views/Estudios/Crear.cshtml` |
| Estudios/Edit: no cambia las dos partes de la PK | Sí | `[BindNever]` + `[FromRoute]` + selects `disabled`; caso 83 |
| Telefono/Create: dueño existente desde lista | Sí | `TelefonoCrearController`, `Views/Telefono/Crear.cshtml` |
| Telefono/Edit: `num` de solo lectura | Sí | `readonly` + `[BindNever]` + `[FromRoute]`; caso 94 |
| Eliminación con referencias: mensaje comprensible, sin cascada | Sí | `PersonaEliminarController`, `ProfesionEliminarController`; casos 65, 66 y 75 |
| Controller View y API Controller no se llaman entre sí | Sí | Ningún controlador de vista usa `HttpClient` ni `Controllers/ApiControllers` |
| Persona: `cc` positivo dentro de `INT` | Sí | `[Range(1, int.MaxValue)]`; caso 54 |
| Persona: nombre y apellido no vacíos, máximo 45 | Sí | `[Required]`, `[StringLength(45)]`; casos 53 y 58 |
| Persona: género `M` o `F` | Sí | Lista + `[RegularExpression("^[MF]$")]`; caso 55 |
| Persona: edad 0 a 130 | Sí | `[Range(0, 130)]`; caso 52 |
| Profesión: `nom` no vacío, máximo 90; `des` opcional | Sí | `ProfesionCrear/EditarViewModel`; casos 71 y 72 |
| Estudios: persona y profesión existentes | Sí (servicio) | `ReferenciaInexistente`; caso 80 |
| Estudios: combinación única | Sí (servicio) | `LlaveDuplicada`; caso 78 |
| Estudios: universidad máximo 50 | Sí | `[StringLength(50)]`; caso 82 |
| Estudios: fecha válida cuando se suministre | Sí, con matiz | `DateOnly?` + `type="date"`; caso 86. El mensaje del servidor sale en inglés (§5, punto 7) |
| Teléfono: `num` no vacío, máximo 15 y único | Sí | `[Required]`, `[StringLength(15)]`, `LlaveDuplicada`; casos 89 y 93 |
| Teléfono: `oper` no vacío, máximo 45 | Sí | `[Required]`, `[StringLength(45)]`; caso 91 |
| Teléfono: `duenio` debe existir | Sí (servicio) | `ReferenciaInexistente`; casos 90 y 96 |

### Riesgos de conflicto al integrar

| Con | Archivos | Riesgo | Cómo resolverlo |
|---|---|---|---|
| Integrante 2 | `Controllers/Services/Interfaces/I*Service.cs`, `Models/DTOs/*Dto.cs` | **Alto** si el integrante 2 escribe su propia versión: los dos partimos del archivo vacío | Que adopte estos archivos tal cual (no tocarlos) o que yo adapte los controladores a su versión antes del merge |
| Integrante 2 | `ConflictoNegocioException.cs` | Bajo (archivo nuevo); medio si elige otro mecanismo | Si elige un resultado tipado, cambian los `catch` de 7 controladores |
| Integrante 2 | `Views/Shared/_Layout.cshtml` | Bajo: el enlace a `/swagger` ya está | Conservar el menú de esta rama |
| Integrante 2 | `docs/procedimiento.md`, `docs/pruebas.md` | Seguro pero trivial: todos parten del archivo vacío | Juntar las secciones de cada integrante |
| Integrante 2 | `Program.cs` | Ninguno: esta rama no lo toca | — |
| Integrante 1 | Entidades, DAO, `Database/` | Ninguno: esta rama no los toca y los Controller View no usan entidades | Si cambia un tipo (por ejemplo `edad` a nulo), lo absorbe el DTO del integrante 2 |
| Ambos | Orden de merge | Si esta rama entra antes que los servicios, `main` compila pero las vistas dan 500 | Fusionar después (o junto) con la rama del integrante 2 |

### Push y pull requests

Ya se hizo push de las dos ramas (`git push -u origin feature/contratos-service` y
`git push -u origin feature/view-mvc`). Los pull requests **no** se abrieron: se crean desde GitHub (o con `gh pr create`)
copiando el título y la descripción de la sección 8. Antes de fusionar, otro integrante revisa cada pull request
(contratos §9).

## 8. Textos de los pull requests

**Orden de fusión:** PR 1 → servicios del integrante 2 (registrados en `Program.cs`) → PR 2.

`feature/view-mvc` contiene el mismo commit `8a22a41` de `feature/contratos-service`. Mientras el PR 1 no entre,
el PR 2 también muestra ese commit; cuando el PR 1 se fusione con *Create a merge commit*, el PR 2 queda solo con
sus commits. Si el PR 1 se fusiona con *Squash* o *Rebase*, `git rebase origin/main` en `feature/view-mvc` descarta
el commit repetido sin conflictos, porque el contenido es idéntico.

### PR 1 — `feature/contratos-service` → `main`

**Título:** `feat(contratos): interfaces de Service y DTO (propuesta del integrante 3)`

**Revisa y aprueba:** integrante 2, **antes** de implementar sus Services.

**Descripción:**

> **Qué es.** Propuesta de las interfaces de Service y los DTO del contrato §4B, para que el integrante 2 implemente
> sus servicios y el integrante 3 construya las vistas sobre las mismas firmas. Solo trae contratos: no hay
> implementaciones, no toca `Program.cs` y compila con 0 advertencias y 0 errores. Todos los archivos empiezan con
> `// PROPUESTA del integrante 3 según contratos §4B; pendiente de aprobación del integrante 2`.
>
> **Archivos (10).**
> - `Controllers/Services/Interfaces/I{Persona,Profesion,Estudios,Telefono}Service.cs`: estaban vacíos.
> - `Controllers/Services/Interfaces/ConflictoNegocioException.cs`: nuevo.
> - `Models/DTOs/{Persona,Profesion,Estudios,Telefono}Dto.cs`: estaban vacíos.
> - `docs/decisiones-mvc.md`: estado de `main` y explicación de la propuesta.
>
> **Firmas.** Las cinco operaciones de `IPersonaService` del contrato, iguales en las cuatro entidades:
>
> ```csharp
> Task<List<PersonaDto>> ObtenerTodosAsync();
> Task<PersonaDto?> ObtenerPorIdAsync(int cc);                       // null si no existe
> Task<PersonaDto> CrearAsync(PersonaCrearDto dto);                  // ConflictoNegocioException
> Task<bool> ActualizarAsync(int cc, PersonaActualizarDto dto);      // false si no existe
> Task<bool> EliminarAsync(int cc);                                  // false si no existe
> ```
>
> | Servicio | Llave en Obtener, Actualizar y Eliminar |
> |---|---|
> | `IPersonaService` | `int cc` |
> | `IProfesionService` | `int id` |
> | `IEstudiosService` | `int idProf, int ccPer` (PK compuesta) |
> | `ITelefonoService` | `string num` |
>
> **DTO.** Un archivo por entidad con tres clases: respuesta, crear y actualizar. Solo campos de la tabla, sin
> navegaciones (§5). Los de actualizar llevan solo los campos editables (§5.2); la llave viaja en la ruta.
>
> | Entidad | Respuesta y crear | Actualizar |
> |---|---|---|
> | Persona | `Cc`, `Nombre`, `Apellido`, `Genero` (`"M"`/`"F"`), `Edad` | `Nombre`, `Apellido`, `Genero`, `Edad` |
> | Profesión | `Id`, `Nom`, `Des?` | `Nom`, `Des?` |
> | Estudios | `IdProf`, `CcPer`, `Fecha?` (`DateOnly`), `Univer?` | `Fecha?`, `Univer?` |
> | Teléfono | `Num`, `Oper`, `Duenio` | `Oper`, `Duenio` |
>
> `ProfesionCrearDto` incluye `Id` porque `profesion.id` no es IDENTITY en la tabla real.
>
> **Mecanismo de errores.**
> - **No existe:** no es un error. Las consultas devuelven `null`, y actualizar o eliminar devuelven `false`.
> - **Errores de negocio:** se lanza una sola excepción, `ConflictoNegocioException`, con una propiedad `Tipo`:
>
>   | `TipoConflicto` | Cuándo |
>   |---|---|
>   | `LlaveDuplicada` | La cédula, el id de profesión, el número o la combinación `(idProf, ccPer)` ya existen |
>   | `ReferenciaInexistente` | No existen la persona o la profesión de un estudio, o el dueño de un teléfono (crear y editar) |
>   | `BorradoConReferencias` | Persona con estudios o teléfonos, o profesión con estudios (las FK son NO ACTION) |
>
> - **Conversión en los controladores:** los Controller View la convierten en un mensaje de `ModelState` y los API
>   Controller, en 409.
> - **Por qué una excepción:** se eligió una excepción y no un resultado tipado porque conserva sin cambios las
>   firmas `Task<Dto>` y `Task<bool>` del contrato.
> - **Lo que no debe salir del servicio:** `DbUpdateException` ni excepciones de SQL.
>
> **Qué pido al integrante 2.**
> 1. Aprobar estos archivos tal cual o pedir cambios en este PR, no en su rama. Así evitamos conflictos, porque los
>    dos partíamos de archivos vacíos.
> 2. Al implementar:
>    - traducir los errores de la base a `ConflictoNegocioException`;
>    - convertir `telefono.oper` nulo en `""`;
>    - registrar los cuatro servicios en `Program.cs` con `AddScoped`.
> 3. Opcional: agregar `ObtenerPorPersonaAsync(int cc)` en estudios y teléfonos.

### PR 2 — `feature/view-mvc` → `main`

**Título:** `feat(mvc): vistas, ViewModels y Controller View del integrante 3`

**Se fusiona:** **después** del PR 1 y de los Services del integrante 2.

**Descripción:**

> **Qué es.** La interfaz web del contrato §6:
> - 20 ViewModels, 20 Controller View y 20 vistas Razor (Listar, Detalle, Crear, Editar y Eliminar de las cuatro
>   entidades);
> - menú en español en `_Layout.cshtml`, con enlace a `/swagger`;
> - accesos a las cuatro listas en el inicio.
>
> Está construida sobre las interfaces del PR 1, y su primer commit es ese mismo PR.
>
> **⚠️ No fusionar todavía.** Hasta que el integrante 2 implemente los servicios y los registre en `Program.cs`,
> `main` compila pero **todas las vistas de las cuatro entidades responden 500**. La inyección de dependencias no
> encuentra `IPersonaService`, `IProfesionService`, `IEstudiosService` ni `ITelefonoService`. El inicio y Privacidad
> sí funcionan. Esta rama **no toca `Program.cs`**.
>
> **Cómo está hecho.**
> - **Un controlador por vista** en `Controllers/ViewControllers/{Entidad}/`. GET y POST de la misma vista van en el
>   mismo controlador.
> - **Rutas por atributo en español:** `/Personas`, `/Personas/Crear`, `/Personas/Editar/{cc}`,
>   `/Estudios/Detalle/{idProf}/{ccPer}`, `/Telefonos/Eliminar/{num}`, …
> - **Dependencias:** los controladores inyectan solo `I...Service`, nunca DAO ni `DbContext`.
> - **Seguridad y flujo de los formularios:**
>   - los 12 POST tienen `[ValidateAntiForgeryToken]`;
>   - se valida `ModelState`;
>   - al guardar se usa PRG con un mensaje en `TempData`;
>   - si el registro no existe, se responde 404;
>   - la llave de Editar y Eliminar sale siempre de la URL (`[FromRoute]` y `[BindNever]`).
> - **Validaciones del §7** con mensajes en español en los ViewModels. Los ViewModels son distintos de los DTO y de
>   las entidades.
> - **Errores del servicio:** `ConflictoNegocioException` se muestra como mensaje en el formulario. En Eliminar, si
>   la persona o la profesión tienen dependientes, se explica el motivo y no se borra.
>
> **Pruebas.**
> - **Prueba de humo:** 100 de 100 casos OK con servicios falsos en memoria, usados solo durante la prueba (ningún
>   commit los contiene). Detalle en `docs/vistas-mvc.md` §7 y `docs/pruebas.md`.
> - **Capturas:** 27, en `docs/evidencias/mvc/`.
> - **Compilación:** cada commit compila con 0 advertencias y 0 errores.
> - **Pendiente:** repetir la prueba con los servicios reales y `persona_db`.
>
> **Sugerencia para el equipo: mensajes del enlace de modelos en español.** Si alguien envía sin navegador un
> número o una fecha mal formados (por ejemplo `Cc=abc`), el mensaje sale del model binder y está en inglés. Toca
> `Program.cs`, que es compartido, así que no está en este PR. Lo propongo para decidirlo entre todos:
>
> ```csharp
> builder.Services.AddControllersWithViews(o =>
> {
>     o.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((v, c) => $"El valor '{v}' no es válido para {c}.");
>     o.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(c => $"El campo {c} debe ser un número.");
>     o.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(v => $"El valor '{v}' no es válido.");
>     o.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(c => $"Falta un valor para {c}.");
>     o.ModelBindingMessageProvider.SetValueIsInvalidAccessor(v => $"El valor '{v}' no es válido.");
> });
> ```
>
> **Documentación.**
> - `docs/decisiones-mvc.md`: decisiones, diferencias con el contrato, pedidos a los integrantes 1 y 2, y riesgos.
> - `docs/vistas-mvc.md`: vistas, rutas y resultados de las pruebas.
> - `docs/procedimiento.md`: sección "Integrante 3 — Vistas".
