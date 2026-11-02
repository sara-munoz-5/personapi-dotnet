## Integrante 3 — Vistas

### Prueba de humo de la interfaz web

- **Alcance:** las 20 vistas (Listar, Detalle, Crear, Editar y Eliminar de Persona, Profesión, Estudios y Teléfono),
  el menú y el inicio.
- **Entorno:** .NET SDK 10.0.402, app en `http://localhost:5269`, servicios falsos en memoria registrados solo
  durante la prueba (la rama del integrante 2 aún no existe). Datos ficticios: 3 personas, 3 profesiones,
  1 estudio y 1 teléfono.
- **Herramienta:** script de `curl` con cookies (antifalsificación y `TempData`) que toma el token
  `__RequestVerificationToken` del formulario antes de cada POST.
- **Resultado:** **100 de 100 casos OK**. La tabla completa está en `docs/vistas-mvc.md`, sección 7.

| Criterio (contratos §6, §7 y §8) | Casos | Resultado |
|---|---|---|
| Las 20 rutas GET responden | 20 | 200 |
| Clave inexistente en Detalle, Editar y Eliminar | 12 | 404 |
| POST sin token antifalsificación | 1 | 400 |
| Crear, editar y eliminar válidos | 13 | 302 a la lista con mensaje |
| Datos inválidos | 17 | el formulario vuelve con los mensajes en español |
| Llave repetida | 4 | mensaje en el formulario, no se crea |
| Registro relacionado inexistente | 3 | mensaje en el formulario, no se guarda |
| Borrado con referencias (persona, profesión) | 3 | no se borra y se explica |
| Llave tomada de la URL aunque el formulario traiga otra | 10 | se modifica el registro correcto |
| Estudios por sus dos llaves | todas las de Estudios | OK |

### Capturas

27 capturas tomadas con Playwright en `docs/evidencias/mvc/` (lista en `docs/vistas-mvc.md`, sección 8).

### Prueba con los servicios reales (2026-10-27)

- **Entorno:** rama `feature/rest-api-services` del integrante 2 (commit `dbc8325`) unida a esta rama. Base
  `persona_db` en SQL Server 2022 (contenedor Docker), creada con `Database/DDL/02_crear_tablas.sql` y los dos
  scripts DML. Se agregaron 2 personas y 2 profesiones de prueba. La cadena se pasó por la variable
  `ConnectionStrings__PersonaConnection`, sin tocar `appsettings.json`.
- **Vistas:** la misma prueba de humo, **100 de 100 casos OK**. Incluye el borrado con dependientes, que llega como
  `ConflictoNegocioException`.
- **API REST:** **62 de 62 casos OK**:
  - los 20 endpoints;
  - los códigos 200, 201 (con `Location`), 204, 400, 404 y 409;
  - Swagger.
- **Casos de borde:** todos OK, sin errores en el log:
  - tildes;
  - `telefono.oper` nulo en la base (se muestra `""`);
  - estudio sin fecha ni universidad;
  - cédula fuera del rango de `INT`;
  - números con espacios o `+`.
- **Falla encontrada y corregida:** Swagger mostraba 52 operaciones porque incluía las rutas de los Controller View.
  Se corrigió con el commit `fix(mvc): excluye los Controller View de Swagger`; ahora muestra solo las 20 de `/api`.

### Pendiente

- La API responde algunos 400 con textos en inglés (`"The Nombre field is required."`), porque los DTO del
  integrante 2 no tienen `ErrorMessage` en `[Required]` y `[StringLength]`.
- Un número o una fecha mal formados enviados sin navegador muestran el mensaje del model binder en inglés
  (ver `docs/decisiones-mvc.md`).
