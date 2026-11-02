# Evidencias

Capturas de la aplicación con datos ficticios, tomadas con Playwright (Chromium, 1280 px de ancho). Se tomaron sobre
la versión publicada (`dotnet publish`, ambiente Production), con los servicios reales y SQL Server 2022, cargado con
los scripts de `Database/`.

## API REST en Swagger UI (`api/`)

Cada captura prueba una operación desde Swagger UI con "Try it out" y muestra la respuesta del servidor.

| Archivo | Qué muestra |
|---|---|
| `01-swagger-endpoints.png` | Los 20 endpoints de `/api` y los esquemas (DTO, `ProblemDetails`) |
| `02-swagger-post-persona-201.png` | `POST /api/personas` válido → `201 Created` |
| `03-swagger-get-persona-200.png` | `GET /api/personas/{cc}` de la persona creada → `200 OK` |
| `04-swagger-post-persona-400.png` | `POST /api/personas` con datos inválidos → `400` con los mensajes en español |
| `05-swagger-delete-persona-409.png` | `DELETE /api/personas/{cc}` de una persona con estudios y teléfonos → `409 Conflict` |
| `06-swagger-get-estudio-200.png` | `GET /api/estudios/{idProf}/{ccPer}` por sus dos llaves → `200 OK` |
| `07-swagger-delete-persona-204.png` | `DELETE /api/personas/{cc}` sin registros relacionados → `204 No Content` |
| `08-swagger-get-persona-404.png` | `GET /api/personas/{cc}` después de borrar → `404 Not Found` |

## Vistas MVC (`mvc/`)

27 capturas del CRUD de las cuatro entidades:

- las listas;
- los formularios con validación;
- la llave repetida;
- el borrado bloqueado por registros relacionados;
- los mensajes de confirmación.

La lista con la descripción de cada una está en [docs/vistas-mvc.md](../vistas-mvc.md), sección 8.
