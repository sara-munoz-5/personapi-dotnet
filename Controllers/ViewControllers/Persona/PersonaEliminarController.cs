using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Models.ViewModels.Persona;

namespace personapi_dotnet.Controllers.ViewControllers.Persona
{
    /// <summary>
    /// Controller View de Views/Persona/Eliminar.cshtml. Sin cascada (contratos §2.2): si la persona
    /// tiene estudios o teléfonos, el servicio lo impide y la vista lo explica.
    /// </summary>
    public class PersonaEliminarController : Controller
    {
        private readonly IPersonaService _personas;

        public PersonaEliminarController(IPersonaService personas)
        {
            _personas = personas;
        }

        [HttpGet("Personas/Eliminar/{cc:int}")]
        public async Task<IActionResult> Eliminar(int cc)
        {
            var vm = await CargarAsync(cc);
            return vm == null ? NotFound() : Vista(vm);
        }

        [HttpPost("Personas/Eliminar/{cc:int}")]
        [ValidateAntiForgeryToken]
        [ActionName("Eliminar")]
        public async Task<IActionResult> EliminarConfirmado([FromRoute] int cc)
        {
            try
            {
                if (!await _personas.EliminarAsync(cc))
                    return NotFound();
            }
            catch (ConflictoNegocioException ex) when (ex.Tipo == TipoConflicto.BorradoConReferencias)
            {
                var vm = await CargarAsync(cc);
                if (vm == null)
                    return NotFound();

                ModelState.AddModelError(string.Empty,
                    "No se puede eliminar la persona porque tiene estudios o teléfonos registrados. Elimínelos primero.");
                return Vista(vm);
            }

            TempData["Mensaje"] = $"Se eliminó la persona con cédula {cc}.";
            return RedirectToAction("Listar", "PersonaListar");
        }

        private async Task<PersonaEliminarViewModel?> CargarAsync(int cc)
        {
            var persona = await _personas.ObtenerPorIdAsync(cc);
            if (persona == null)
                return null;

            return new PersonaEliminarViewModel
            {
                Cc = persona.Cc,
                Nombre = persona.Nombre,
                Apellido = persona.Apellido,
                Genero = Generos.Texto(persona.Genero),
                Edad = persona.Edad
            };
        }

        private ViewResult Vista(PersonaEliminarViewModel vm) => View("~/Views/Persona/Eliminar.cshtml", vm);
    }
}
