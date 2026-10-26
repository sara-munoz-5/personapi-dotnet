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

### Pendiente

- Repetir la prueba con los servicios reales del integrante 2 y la base `persona_db`, en especial que el borrado
  con dependientes llegue como `ConflictoNegocioException` (las FK son NO ACTION).
- Un número o una fecha mal formados enviados sin navegador muestran el mensaje del model binder en inglés
  (ver `docs/decisiones-mvc.md`).
