# Vistas, ViewModels y Controller View — Integrante 3

Interfaz web del monolito personapi-dotnet: 20 vistas Razor, 20 ViewModels y 20 Controller View, uno por vista
(contratos §6). Rama `feature/view-mvc`.

## 1. Flujo

```text
Navegador ──HTTP──▶ Controller View ──▶ I...Service ──▶ I...DAO ──▶ EF Core ──▶ SQL Server
   ▲                  │  (ViewModel)        (DTO)          (entidad)
   └──── Vista Razor ◀┘
```

- Cada Controller View inyecta **solo interfaces `I...Service`**; nunca un `I...DAO` ni el `DbContext` (contratos §3 y §6).
- El controlador convierte el ViewModel en un DTO de crear o actualizar, llama al servicio y convierte el DTO de
  respuesta en el ViewModel de la vista. Las vistas nunca reciben entidades EF ni DTO.
- Un Controller View no llama a un API Controller: los dos consumen los mismos servicios.

## 2. Las 20 vistas

Todas las rutas son por atributo (`[HttpGet]` y `[HttpPost]` con plantilla). Como el nombre del controlador no
coincide con la carpeta de la vista, cada acción devuelve la vista con ruta explícita (`View("~/Views/Persona/Crear.cshtml", vm)`).

| Vista | ViewModel | Controller View | Ruta | Servicios que usa |
|---|---|---|---|---|
| `Views/Persona/Listar.cshtml` | `PersonaListarViewModel` | `PersonaListarController` | `GET /Personas` | `IPersonaService` |
| `Views/Persona/Detalle.cshtml` | `PersonaDetalleViewModel` | `PersonaDetalleController` | `GET /Personas/Detalle/{cc}` | `IPersonaService`, `IEstudiosService`, `ITelefonoService`, `IProfesionService` |
| `Views/Persona/Crear.cshtml` | `PersonaCrearViewModel` | `PersonaCrearController` | `GET/POST /Personas/Crear` | `IPersonaService` |
| `Views/Persona/Editar.cshtml` | `PersonaEditarViewModel` | `PersonaEditarController` | `GET/POST /Personas/Editar/{cc}` | `IPersonaService` |
| `Views/Persona/Eliminar.cshtml` | `PersonaEliminarViewModel` | `PersonaEliminarController` | `GET/POST /Personas/Eliminar/{cc}` | `IPersonaService` |
| `Views/Profesion/Listar.cshtml` | `ProfesionListarViewModel` | `ProfesionListarController` | `GET /Profesiones` | `IProfesionService` |
| `Views/Profesion/Detalle.cshtml` | `ProfesionDetalleViewModel` | `ProfesionDetalleController` | `GET /Profesiones/Detalle/{id}` | `IProfesionService` |
| `Views/Profesion/Crear.cshtml` | `ProfesionCrearViewModel` | `ProfesionCrearController` | `GET/POST /Profesiones/Crear` | `IProfesionService` |
| `Views/Profesion/Editar.cshtml` | `ProfesionEditarViewModel` | `ProfesionEditarController` | `GET/POST /Profesiones/Editar/{id}` | `IProfesionService` |
| `Views/Profesion/Eliminar.cshtml` | `ProfesionEliminarViewModel` | `ProfesionEliminarController` | `GET/POST /Profesiones/Eliminar/{id}` | `IProfesionService` |
| `Views/Estudios/Listar.cshtml` | `EstudiosListarViewModel` | `EstudiosListarController` | `GET /Estudios` | `IEstudiosService`, `IPersonaService`, `IProfesionService` |
| `Views/Estudios/Detalle.cshtml` | `EstudiosDetalleViewModel` | `EstudiosDetalleController` | `GET /Estudios/Detalle/{idProf}/{ccPer}` | `IEstudiosService`, `IPersonaService`, `IProfesionService` |
| `Views/Estudios/Crear.cshtml` | `EstudiosCrearViewModel` | `EstudiosCrearController` | `GET/POST /Estudios/Crear` | `IEstudiosService`, `IPersonaService`, `IProfesionService` |
| `Views/Estudios/Editar.cshtml` | `EstudiosEditarViewModel` | `EstudiosEditarController` | `GET/POST /Estudios/Editar/{idProf}/{ccPer}` | `IEstudiosService`, `IPersonaService`, `IProfesionService` |
| `Views/Estudios/Eliminar.cshtml` | `EstudiosEliminarViewModel` | `EstudiosEliminarController` | `GET/POST /Estudios/Eliminar/{idProf}/{ccPer}` | `IEstudiosService`, `IPersonaService`, `IProfesionService` |
| `Views/Telefono/Listar.cshtml` | `TelefonoListarViewModel` | `TelefonoListarController` | `GET /Telefonos` | `ITelefonoService`, `IPersonaService` |
| `Views/Telefono/Detalle.cshtml` | `TelefonoDetalleViewModel` | `TelefonoDetalleController` | `GET /Telefonos/Detalle/{num}` | `ITelefonoService`, `IPersonaService` |
| `Views/Telefono/Crear.cshtml` | `TelefonoCrearViewModel` | `TelefonoCrearController` | `GET/POST /Telefonos/Crear` | `ITelefonoService`, `IPersonaService` |
| `Views/Telefono/Editar.cshtml` | `TelefonoEditarViewModel` | `TelefonoEditarController` | `GET/POST /Telefonos/Editar/{num}` | `ITelefonoService`, `IPersonaService` |
| `Views/Telefono/Eliminar.cshtml` | `TelefonoEliminarViewModel` | `TelefonoEliminarController` | `GET/POST /Telefonos/Eliminar/{num}` | `ITelefonoService`, `IPersonaService` |

Carpetas: `Controllers/ViewControllers/{Entidad}/`, `Models/ViewModels/{Entidad}/` y `Views/{Entidad}/`, con
`{Entidad}` = `Persona`, `Profesion`, `Estudios`, `Telefono` (estructura real de `main`). Los parámetros numéricos
llevan restricción `:int` (`{cc:int}`, `{id:int}`, `{idProf:int}/{ccPer:int}`).

Los servicios extra se usan solo para mostrar nombres en lugar de llaves (persona, profesión, dueño) y para llenar
las listas de selección. El Detalle de Persona usa cuatro servicios porque los DTO no traen navegaciones
(contratos §5): los estudios y teléfonos de la persona se filtran de `ObtenerTodosAsync`.

Navegación: el menú de `Views/Shared/_Layout.cshtml` enlaza Inicio, Personas, Profesiones, Estudios, Teléfonos,
`/swagger` y Privacidad. `Views/Home/Index.cshtml` tiene un acceso a cada una de las cuatro listas.

## 3. Reglas comunes

- **Acciones async** en todo lo que llama a un servicio. Los GET de Crear de Persona y Profesión no llaman a
  ningún servicio y son síncronos.
- **Antifalsificación:** `[ValidateAntiForgeryToken]` en los 12 POST; el Tag Helper `<form method="post">` agrega el token.
- **ModelState:** todo POST con formulario revisa `ModelState.IsValid` antes de llamar al servicio.
- **PRG:** después de crear, editar o eliminar, `RedirectToAction("Listar", ...)` con el mensaje en
  `TempData["Mensaje"]`. Listar lo muestra en una alerta verde una sola vez.
- **NotFound:** si el servicio devuelve `null` (consultar) o `false` (actualizar o eliminar), la acción responde 404.
- **Llave desde la URL:** en los POST de Editar y Eliminar la llave es un parámetro `[FromRoute]`. Las propiedades de
  la llave en los ViewModels de Editar tienen `[BindNever]`, así que un campo del formulario no puede cambiar qué
  registro se edita (probado en los casos 59, 73, 83 y 94 de la prueba de humo).
- **Listas:** Crear y Editar de Estudios y Teléfono recargan sus `SelectListItem` en cada respuesta, también
  cuando el formulario vuelve con errores; la opción elegida se conserva.

## 4. Validaciones (contratos §7)

Mensajes en español con DataAnnotations; se validan en el navegador (jQuery Validation, `_ValidationScriptsPartial`)
y en el servidor (`ModelState`). Los campos numéricos obligatorios son `int?` con `[Required]` para que el campo
vacío dé el mensaje en español y no el del model binder.

| Campo | Regla | Mensaje |
|---|---|---|
| Persona `Cc` | obligatorio, 1 a `int.MaxValue` | "La cédula es obligatoria." / "La cédula debe ser un número positivo." |
| Persona `Nombre`, `Apellido` | obligatorios, máximo 45 | "El nombre es obligatorio." / "El nombre admite máximo 45 caracteres." |
| Persona `Genero` | obligatorio, `M` o `F`, elegido de una lista (Masculino/Femenino) | "Seleccione el género." / "El género debe ser Masculino o Femenino." |
| Persona `Edad` | obligatoria, 0 a 130 | "La edad es obligatoria." / "La edad debe estar entre 0 y 130." |
| Profesión `Id` | obligatorio, positivo (la tabla no usa IDENTITY) | "El id es obligatorio." / "El id debe ser un número positivo." |
| Profesión `Nom` | obligatorio, máximo 90 | "El nombre es obligatorio." / "El nombre admite máximo 90 caracteres." |
| Profesión `Des` | opcional | — |
| Estudios `IdProf`, `CcPer` | elegidos de listas de registros existentes | "Seleccione la profesión." / "Seleccione la persona." |
| Estudios `Univer` | opcional, máximo 50 | "La universidad admite máximo 50 caracteres." |
| Estudios `Fecha` | opcional, `<input type="date">` (`DateOnly?`) | fecha inválida → error del model binder |
| Teléfono `Num` | obligatorio, máximo 15 | "El número es obligatorio." / "El número admite máximo 15 caracteres." |
| Teléfono `Oper` | obligatorio, máximo 45 | "El operador es obligatorio." / "El operador admite máximo 45 caracteres." |
| Teléfono `Duenio` | elegido de la lista de personas | "Seleccione el dueño." |

Las reglas que necesitan la base (llave repetida, registro relacionado inexistente, borrado con dependientes) son
del servicio: llegan como `ConflictoNegocioException` y el controlador las muestra con `ModelState.AddModelError`.

## 5. Manejo de errores de negocio

| Tipo (`TipoConflicto`) | Dónde | Mensaje en la vista |
|---|---|---|
| `LlaveDuplicada` | Crear persona | "Ya existe una persona con esa cédula." (junto al campo Cédula) |
| `LlaveDuplicada` | Crear profesión | "Ya existe una profesión con ese id." |
| `LlaveDuplicada` | Crear estudio | "Esa persona ya tiene registrado un estudio de esa profesión." |
| `LlaveDuplicada` | Crear teléfono | "Ya existe un teléfono con ese número." |
| `ReferenciaInexistente` | Crear estudio | "La persona o la profesión seleccionada ya no existe." |
| `ReferenciaInexistente` | Crear y editar teléfono | "La persona seleccionada como dueño ya no existe." |
| `BorradoConReferencias` | Eliminar persona | "No se puede eliminar la persona porque tiene estudios o teléfonos registrados. Elimínelos primero." |
| `BorradoConReferencias` | Eliminar profesión | "No se puede eliminar la profesión porque hay estudios registrados con ella. Elimínelos primero." |

Cualquier otra excepción no se captura: llega a la página de error de la plantilla (500), para no esconder fallas.

## 6. Casos especiales

- **Llave compuesta de Estudios:** todas las rutas y enlaces llevan `idProf` y `ccPer` (`asp-route-idProf`,
  `asp-route-ccPer`). Para cambiar la persona o la profesión se elimina el estudio y se crea otro (contratos §4.3).
- **Solo lectura en Editar:** la cédula, el id de profesión y el número de teléfono se muestran con `readonly`; la
  persona y la profesión de un estudio, como listas `disabled` con la opción actual. Ninguno se envía al servicio:
  la llave sale de la URL.
- **Borrado sin cascada (contratos §2.2, NO ACTION):** si una persona tiene estudios o teléfonos, o una profesión
  tiene estudios, la vista Eliminar se vuelve a mostrar con el error en rojo y sin el botón Eliminar. No se borra nada.
- **Género:** se guarda `M` o `F` y se muestra como Masculino o Femenino (`Models/ViewModels/Persona/Generos.cs`).

## 7. Prueba de humo

Se hizo sin la parte del integrante 2: se registraron **temporalmente** servicios falsos en memoria con datos
ficticios (3 personas, 3 profesiones, 1 estudio y 1 teléfono) que lanzan `ConflictoNegocioException` igual que
pide el contrato. Se levantó la app en `http://localhost:5269` y se recorrió con `curl` usando cookies y el token
antifalsificación tomado del formulario. Después se quitaron los falsos y su registro; no están en ningún commit.

**Resultado: 100 de 100 casos OK.**

| Grupo | Casos | Resultado |
|---|---|---|
| Las 20 rutas GET | 20 | 200 en todas |
| Contenido (menú, inicio, nombres en lugar de llaves, género en texto, enlaces con las llaves, solo lectura, input de fecha, labels, scripts de validación) | 16 | OK |
| No existe (GET de Detalle, Editar y Eliminar con llaves inexistentes) | 10 | 404 |
| POST sin token antifalsificación | 1 | 400 |
| Crear válido de las 4 entidades | 6 | 302 a la lista (PRG) y mensaje `TempData` una sola vez |
| Llave repetida (persona, profesión, estudio, teléfono) y recarga de listas | 5 | 200 con el mensaje; listas recargadas |
| Registro relacionado inexistente (estudio, teléfono al crear y al editar) | 3 | 200 con el mensaje |
| Validaciones (vacíos, longitudes 46/91/51/16, rango de edad, cédula negativa, género, fecha inválida) | 17 | 200 con los mensajes |
| Editar con la llave cambiada en el formulario (4 entidades) | 10 | se edita el registro de la URL y no el otro |
| Editar y Eliminar inexistentes por POST | 2 | 404 |
| Borrado con dependientes (persona y profesión) | 3 | no borra, explica y oculta el botón |
| Eliminar válido y listas vacías | 7 | 302 y mensaje "No hay … registrados." |

<details>
<summary>Tabla completa de los 100 casos</summary>

| # | Caso | Petición | Respuesta | Estado |
|---|---|---|---|---|
| 1 | Vista | `GET /Personas` | 200 | OK |
| 2 | Vista | `GET /Personas/Detalle/123456` | 200 | OK |
| 3 | Vista | `GET /Personas/Crear` | 200 | OK |
| 4 | Vista | `GET /Personas/Editar/123456` | 200 | OK |
| 5 | Vista | `GET /Personas/Eliminar/222333` | 200 | OK |
| 6 | Vista | `GET /Profesiones` | 200 | OK |
| 7 | Vista | `GET /Profesiones/Detalle/1` | 200 | OK |
| 8 | Vista | `GET /Profesiones/Crear` | 200 | OK |
| 9 | Vista | `GET /Profesiones/Editar/1` | 200 | OK |
| 10 | Vista | `GET /Profesiones/Eliminar/2` | 200 | OK |
| 11 | Vista | `GET /Estudios` | 200 | OK |
| 12 | Vista | `GET /Estudios/Detalle/1/123456` | 200 | OK |
| 13 | Vista | `GET /Estudios/Crear` | 200 | OK |
| 14 | Vista | `GET /Estudios/Editar/1/123456` | 200 | OK |
| 15 | Vista | `GET /Estudios/Eliminar/1/123456` | 200 | OK |
| 16 | Vista | `GET /Telefonos` | 200 | OK |
| 17 | Vista | `GET /Telefonos/Detalle/3001234567` | 200 | OK |
| 18 | Vista | `GET /Telefonos/Crear` | 200 | OK |
| 19 | Vista | `GET /Telefonos/Editar/3001234567` | 200 | OK |
| 20 | Vista | `GET /Telefonos/Eliminar/3001234567` | 200 | OK |
| 21 | Inicio con accesos a las 4 listas | `GET /` | 200 | OK |
| 22 | Menú con enlace a Swagger | `GET /` | - | OK |
| 23 | Enlaces con la cédula | `GET /Personas` | 200 | OK |
| 24 | Género como texto | `GET /Personas` | - | OK |
| 25 | Enlaces con las 2 llaves | `GET /Estudios` | 200 | OK |
| 26 | Nombre de persona y profesión | `GET /Estudios` | - | OK |
| 27 | Dueño por nombre | `GET /Telefonos` | 200 | OK |
| 28 | Detalle con teléfonos | `GET /Personas/Detalle/123456` | 200 | OK |
| 29 | Detalle con estudios | `GET /Personas/Detalle/123456` | - | OK |
| 30 | Llave de solo lectura (disabled) | `GET /Estudios/Editar/1/123456` | 200 | OK |
| 31 | Fecha como input date | `GET /Estudios/Editar/1/123456` | - | OK |
| 32 | Fecha en formato ISO | `GET /Estudios/Editar/1/123456` | - | OK |
| 33 | Cédula de solo lectura | `GET /Personas/Editar/123456` | 200 | OK |
| 34 | Label asociado (for=Cc) | `GET /Personas/Editar/123456` | - | OK |
| 35 | Número de solo lectura | `GET /Telefonos/Editar/3001234567` | 200 | OK |
| 36 | Scripts de validación | `GET /Personas/Crear` | 200 | OK |
| 37 | No existe | `GET /Personas/Detalle/999` | 404 | OK |
| 38 | No existe | `GET /Personas/Editar/999` | 404 | OK |
| 39 | No existe | `GET /Personas/Eliminar/999` | 404 | OK |
| 40 | No existe | `GET /Profesiones/Detalle/999` | 404 | OK |
| 41 | No existe | `GET /Profesiones/Editar/999` | 404 | OK |
| 42 | No existe | `GET /Estudios/Detalle/9/9` | 404 | OK |
| 43 | No existe | `GET /Estudios/Editar/9/9` | 404 | OK |
| 44 | No existe | `GET /Estudios/Eliminar/9/9` | 404 | OK |
| 45 | No existe | `GET /Telefonos/Detalle/000` | 404 | OK |
| 46 | No existe | `GET /Telefonos/Eliminar/000` | 404 | OK |
| 47 | POST sin token antifalsificación | `POST /Personas/Crear` | 400 | OK |
| 48 | Crear persona (PRG) | `POST /Personas/Crear` | 302 → /Personas | OK |
| 49 | Mensaje TempData tras crear | `GET /Personas` | 200 | OK |
| 50 | Mensaje se muestra una sola vez | `GET /Personas` | - | OK |
| 51 | Cédula repetida | `POST /Personas/Crear` | 200 | OK |
| 52 | Validación: edad fuera de rango | `POST /Personas/Crear` | 200 | OK |
| 53 | Validación: nombre vacío | `POST /Personas/Crear` | - | OK |
| 54 | Validación: cédula negativa | `POST /Personas/Crear` | - | OK |
| 55 | Validación: género distinto de M/F | `POST /Personas/Crear` | - | OK |
| 56 | Cédula y edad vacías | `POST /Personas/Crear` | 200 | OK |
| 57 | Mensaje en español (cédula vacía) | `POST /Personas/Crear` | - | OK |
| 58 | Nombre de 46 caracteres | `POST /Personas/Crear` | 200 | OK |
| 59 | Editar con Cc=123456 en el formulario | `POST /Personas/Editar/777` | 302 → /Personas | OK |
| 60 | Se editó el 777 (llave de la URL) | `GET /Personas/Detalle/777` | - | OK |
| 61 | No se tocó el 123456 | `GET /Personas/Detalle/123456` | - | OK |
| 62 | Editar con error de validación | `POST /Personas/Editar/777` | 200 | OK |
| 63 | Cédula sigue visible tras el error | `POST /Personas/Editar/777` | - | OK |
| 64 | Editar inexistente | `POST /Personas/Editar/999` | 404 | OK |
| 65 | Borrar persona con dependientes | `POST /Personas/Eliminar/123456` | 200 | OK |
| 66 | No ofrece el botón Eliminar tras el bloqueo | `POST /Personas/Eliminar/123456` | - | OK |
| 67 | Eliminar persona | `POST /Personas/Eliminar/777` | 302 → /Personas | OK |
| 68 | Eliminar inexistente | `POST /Personas/Eliminar/777` | 404 | OK |
| 69 | Crear profesión | `POST /Profesiones/Crear` | 302 → /Profesiones | OK |
| 70 | Id repetido | `POST /Profesiones/Crear` | 200 | OK |
| 71 | Nombre vacío | `POST /Profesiones/Crear` | 200 | OK |
| 72 | Nombre de 91 caracteres | `POST /Profesiones/Crear` | 200 | OK |
| 73 | Editar con Id=1 en el formulario | `POST /Profesiones/Editar/9` | 302 → /Profesiones | OK |
| 74 | No se tocó la profesión 1 | `GET /Profesiones/Detalle/1` | - | OK |
| 75 | Borrar profesión con estudios | `POST /Profesiones/Eliminar/1` | 200 | OK |
| 76 | Eliminar profesión | `POST /Profesiones/Eliminar/9` | 302 → /Profesiones | OK |
| 77 | Crear estudio | `POST /Estudios/Crear` | 302 → /Estudios | OK |
| 78 | Combinación repetida | `POST /Estudios/Crear` | 200 | OK |
| 79 | Listas recargadas tras el error (selección conservada) | `POST /Estudios/Crear` | - | OK |
| 80 | Profesión inexistente (FK) | `POST /Estudios/Crear` | 200 | OK |
| 81 | Sin persona ni profesión | `POST /Estudios/Crear` | 200 | OK |
| 82 | Universidad de 51 caracteres | `POST /Estudios/Crear` | 200 | OK |
| 83 | Editar con otras llaves en el formulario | `POST /Estudios/Editar/2/222333` | 302 → /Estudios | OK |
| 84 | Se editó (2, 222333) | `GET /Estudios/Detalle/2/222333` | - | OK |
| 85 | No se tocó (1, 123456) | `GET /Estudios/Detalle/1/123456` | - | OK |
| 86 | Fecha inválida | `POST /Estudios/Editar/2/222333` | 200 | OK |
| 87 | Eliminar estudio | `POST /Estudios/Eliminar/2/222333` | 302 → /Estudios | OK |
| 88 | Crear teléfono | `POST /Telefonos/Crear` | 302 → /Telefonos | OK |
| 89 | Número repetido | `POST /Telefonos/Crear` | 200 | OK |
| 90 | Dueño inexistente (FK) | `POST /Telefonos/Crear` | 200 | OK |
| 91 | Validación: operador vacío | `POST /Telefonos/Crear` | 200 | OK |
| 92 | Validación: sin dueño | `POST /Telefonos/Crear` | - | OK |
| 93 | Validación: número de 16 caracteres | `POST /Telefonos/Crear` | - | OK |
| 94 | Editar con otro Num en el formulario | `POST /Telefonos/Editar/3110000000` | 302 → /Telefonos | OK |
| 95 | No se tocó 3001234567 | `GET /Telefonos/Detalle/3001234567` | - | OK |
| 96 | Editar con dueño inexistente | `POST /Telefonos/Editar/3110000000` | 200 | OK |
| 97 | Eliminar teléfono | `POST /Telefonos/Eliminar/3110000000` | 302 → /Telefonos | OK |
| 98 | Sin dependientes, la persona ya se borra | `POST /Personas/Eliminar/123456` | 302 → /Personas | OK |
| 99 | Lista vacía con mensaje | `GET /Telefonos` | 200 | OK |
| 100 | Lista vacía con mensaje | `GET /Estudios` | 200 | OK |

</details>

**Hallazgo:** si se envía a mano (sin navegador) un número o una fecha mal formados, por ejemplo `Cc=abc` o
`Fecha=xx`, el error aparece en el formulario pero con el texto en inglés del model binder ("The value 'abc' is not
valid for Cédula."). Desde el navegador no pasa: los campos son `type="number"` y `type="date"`. La corrección está
propuesta en `docs/decisiones-mvc.md` porque toca `Program.cs`.

## 8. Capturas

Tomadas con Playwright (Chromium sin interfaz, 1280 px de ancho) sobre los mismos servicios falsos; están en
`docs/evidencias/mvc/`.

| Archivo | Qué muestra |
|---|---|
| `01-inicio.png` | Inicio con accesos a las cuatro listas y menú en español |
| `02-personas-listar.png` | Lista de personas con género en texto |
| `03-personas-detalle.png` | Detalle con estudios y teléfonos de la persona |
| `04-personas-crear.png` | Formulario vacío |
| `05-personas-crear-validacion.png` | Validación en el navegador al enviar vacío |
| `06-personas-crear-cedula-repetida.png` | Error del servicio: cédula repetida |
| `07-personas-listar-mensaje.png` | PRG: mensaje verde tras crear |
| `08-personas-editar.png` | Cédula de solo lectura |
| `09-personas-eliminar.png` | Confirmación de borrado |
| `10-personas-eliminar-con-dependientes.png` | Borrado bloqueado por estudios y teléfonos |
| `11-profesiones-listar.png` a `15-profesiones-eliminar.png` | Las cinco vistas de Profesión |
| `16-estudios-listar.png` | Nombres de persona y profesión en lugar de llaves |
| `17-estudios-detalle.png` | Detalle con la llave compuesta |
| `18-estudios-crear.png` | Listas de personas y profesiones |
| `19-estudios-crear-repetido.png` | Error: combinación persona–profesión repetida |
| `20-estudios-editar.png` | Persona y profesión deshabilitadas; fecha con selector |
| `21-estudios-eliminar.png` | Confirmación de borrado de un estudio |
| `22-telefonos-listar.png` a `26-telefonos-eliminar.png` | Las cinco vistas de Teléfono (dueño por nombre) |
| `27-telefonos-listar-tras-eliminar.png` | Lista vacía con su mensaje tras eliminar |
