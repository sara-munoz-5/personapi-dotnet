## Integrante 3 — Vistas

Procedimiento seguido para la interfaz web (ViewModels, Controller View y vistas Razor). El detalle está en
`docs/vistas-mvc.md` y las decisiones en `docs/decisiones-mvc.md`.

1. **Rama.** Desde `main` actualizado se creó `feature/view-mvc` (nombre de la sección 9 de los contratos).
2. **Contrato con el integrante 2.** Como aún no había interfaces de Service ni DTO con contenido, se escribió una
   propuesta según contratos §4B en `Controllers/Services/Interfaces/` y `Models/DTOs/`, marcada como PROPUESTA en
   cada archivo, con un único mecanismo de errores de negocio (`ConflictoNegocioException` con `TipoConflicto`).
   No se implementaron servicios ni se registró nada en `Program.cs`.
3. **ViewModels.** Uno por vista en `Models/ViewModels/{Entidad}/`, con solo los campos de esa pantalla y las
   validaciones de contratos §7 con mensajes en español. Los de Crear y Editar de Estudios y Teléfono traen sus
   listas (`SelectListItem`).
4. **Controller View.** Uno por vista en `Controllers/ViewControllers/{Entidad}/`, con rutas por atributo en español
   (`/Personas`, `/Personas/Crear`, `/Estudios/Editar/{idProf}/{ccPer}`, …). GET y POST de la misma vista van en el
   mismo controlador. Inyectan solo interfaces `I...Service`, revisan `ModelState`, usan `[ValidateAntiForgeryToken]`,
   PRG con `TempData`, `NotFound()` cuando el servicio devuelve `null` o `false`, y la llave de Editar y Eliminar
   sale de la URL.
5. **Vistas Razor.** Bootstrap 5 y Tag Helpers en `Views/{Entidad}/` (Listar, Detalle, Crear, Editar, Eliminar);
   `_ValidationScriptsPartial` en Crear y Editar; mensaje de lista vacía; botón Volver o Cancelar en todas. Menú en
   español en `_Layout.cshtml` con enlace a `/swagger`, y accesos a las cuatro listas en `Home/Index.cshtml`.
6. **Verificación sin el integrante 2.** Se registraron temporalmente servicios falsos en memoria, se levantó la app
   y se recorrieron con `curl` las 20 rutas y los POST principales (100 de 100 casos OK). Se tomaron 27 capturas con
   Playwright en `docs/evidencias/mvc/`. Luego se quitaron los falsos y su registro; ningún commit los contiene.
7. **Compilación.** `dotnet build personapi-dotnet.csproj`: 0 advertencias y 0 errores en cada commit.

### Cómo ver las vistas cuando estén los servicios reales

1. Crear la base con los scripts de `Database/` (integrante 1) y revisar la cadena `PersonaConnection` de `appsettings.json`.
2. Integrar la rama del integrante 2 (servicios registrados en `Program.cs`).
3. `dotnet run --launch-profile http` y abrir `http://localhost:5269`; el menú lleva a las cuatro listas.
