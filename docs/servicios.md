# Services y reglas de negocio

## Interfaces

Cada Service expone las operaciones `ObtenerTodosAsync`, `ObtenerPorIdAsync`, `CrearAsync`, `ActualizarAsync` y `EliminarAsync`. Los Services dependen de interfaces DAO y no de clases concretas de infraestructura.

## Reglas implementadas

### Persona

- La cédula debe ser única.
- La edad debe estar entre 0 y 130.
- El género debe ser `M` o `F`.
- No se puede eliminar una persona con estudios o teléfonos relacionados.

### Profesión

- El identificador se recibe explícitamente porque `profesion.id` no es `IDENTITY`.
- El identificador debe ser único.
- No se puede eliminar una profesión con estudios relacionados.

### Estudio

- La clave está formada por `id_prof` y `cc_per`.
- La persona y la profesión deben existir antes de crear el registro.
- No se permiten dos estudios con la misma combinación de persona y profesión.
- La fecha y la universidad se pueden actualizar; las claves no cambian.

### Teléfono

- El número es la clave y debe ser único.
- El dueño debe existir antes de crear o actualizar.
- El número no se modifica durante una actualización.

## DTO

Los DTO separan los contratos HTTP de las entidades EF. Los DTO de creación y actualización tienen validaciones DataAnnotations para longitudes, rangos, campos obligatorios y formato del género.

## Conflictos

`ConflictoNegocioException` permite que la capa de negocio identifique tres situaciones:

- `LlaveDuplicada`;
- `ReferenciaInexistente`;
- `BorradoConReferencias`.

La API devuelve estas situaciones como `409 Conflict` con un objeto JSON que contiene `mensaje` y `tipo`. Los controladores MVC las convierten en mensajes de formulario.
