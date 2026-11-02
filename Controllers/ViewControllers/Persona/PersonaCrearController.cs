using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Models.DTOs;
using personapi_dotnet.Models.ViewModels.Persona;

namespace personapi_dotnet.Controllers.ViewControllers.Persona
{
    /// <summary>Controller View de Views/Persona/Crear.cshtml (GET formulario, POST crear).</summary>
    [ApiExplorerSettings(IgnoreApi = true)] // Controller View: fuera de Swagger, que documenta solo la API
    public class PersonaCrearController : Controller
    {
        private readonly IPersonaService _personas;

        public PersonaCrearController(IPersonaService personas)
        {
            _personas = personas;
        }

        [HttpGet("Personas/Crear")]
        public IActionResult Crear()
        {
            return Vista(new PersonaCrearViewModel());
        }

        [HttpPost("Personas/Crear")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(PersonaCrearViewModel vm)
        {
            if (!ModelState.IsValid)
                return Vista(vm);

            try
            {
                await _personas.CrearAsync(new PersonaCrearDto
                {
                    Cc = vm.Cc!.Value,
                    Nombre = vm.Nombre,
                    Apellido = vm.Apellido,
                    Genero = vm.Genero,
                    Edad = vm.Edad!.Value
                });
            }
            catch (ConflictoNegocioException ex) when (ex.Tipo == TipoConflicto.LlaveDuplicada)
            {
                ModelState.AddModelError(nameof(vm.Cc), "Ya existe una persona con esa cédula.");
                return Vista(vm);
            }

            TempData["Mensaje"] = $"Se creó la persona {vm.Nombre} {vm.Apellido}.";
            return RedirectToAction("Listar", "PersonaListar");
        }

        private ViewResult Vista(PersonaCrearViewModel vm) => View("~/Views/Persona/Crear.cshtml", vm);
    }
}
