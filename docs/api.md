# API REST

La API se documenta con Swagger/OpenAPI y está disponible en `/swagger`.

## Personas

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/personas` | Lista personas |
| GET | `/api/personas/{cc}` | Consulta una persona |
| POST | `/api/personas` | Crea una persona |
| PUT | `/api/personas/{cc}` | Actualiza una persona |
| DELETE | `/api/personas/{cc}` | Elimina una persona |

## Profesiones

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/profesiones` | Lista profesiones |
| GET | `/api/profesiones/{id}` | Consulta una profesión |
| POST | `/api/profesiones` | Crea una profesión |
| PUT | `/api/profesiones/{id}` | Actualiza una profesión |
| DELETE | `/api/profesiones/{id}` | Elimina una profesión |

## Estudios

`Estudios` usa una clave compuesta formada por `idProf` y `ccPer`.

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/estudios` | Lista estudios |
| GET | `/api/estudios/{idProf}/{ccPer}` | Consulta un estudio |
| POST | `/api/estudios` | Crea un estudio |
| PUT | `/api/estudios/{idProf}/{ccPer}` | Actualiza un estudio |
| DELETE | `/api/estudios/{idProf}/{ccPer}` | Elimina un estudio |

## Teléfonos

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/telefonos` | Lista teléfonos |
| GET | `/api/telefonos/{num}` | Consulta un teléfono |
| POST | `/api/telefonos` | Crea un teléfono |
| PUT | `/api/telefonos/{num}` | Actualiza un teléfono |
| DELETE | `/api/telefonos/{num}` | Elimina un teléfono |

## Respuestas HTTP

- `200 OK`: consulta exitosa.
- `201 Created`: creación exitosa; incluye la URL del recurso en `Location`.
- `204 No Content`: actualización o eliminación exitosa.
- `400 Bad Request`: JSON inválido o violación de validaciones del DTO.
- `404 Not Found`: recurso inexistente.
- `409 Conflict`: clave duplicada, referencia inexistente o borrado con dependencias.

## Ejemplo de creación de persona

```json
{
  "cc": 123456,
  "nombre": "Sara",
  "apellido": "Muñoz",
  "genero": "F",
  "edad": 21
}
```

## Seguridad y alcance

El laboratorio no solicita autenticación. La API usa la cadena de conexión configurada en `appsettings.json`; en un ambiente productivo se recomienda usar secretos o variables de entorno en lugar de guardar credenciales en archivos de configuración.
