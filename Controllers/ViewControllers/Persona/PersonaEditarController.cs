using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Models.DTOs;
using personapi_dotnet.Models.ViewModels.Persona;

namespace personapi_dotnet.Controllers.ViewControllers.Persona
{
    /// <summary>Controller View de Views/Persona/Editar.cshtml. La cédula sale de la URL, nunca del formulario.</summary>
    public class PersonaEditarController : Controller
    {
        private readonly IPersonaService _personas;

        public PersonaEditarController(IPersonaService personas)
        {
            _personas = personas;
        }

        [HttpGet("Personas/Editar/{cc:int}")]
        public async Task<IActionResult> Editar(int cc)
        {
            var persona = await _personas.ObtenerPorIdAsync(cc);
            if (persona == null)
                return NotFound();

            return Vista(new PersonaEditarViewModel
            {
                Cc = persona.Cc,
                Nombre = persona.Nombre,
                Apellido = persona.Apellido,
                Genero = persona.Genero,
                Edad = persona.Edad
            });
        }

        [HttpPost("Personas/Editar/{cc:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar([FromRoute] int cc, PersonaEditarViewModel vm)
        {
            vm.Cc = cc;
            if (!ModelState.IsValid)
                return Vista(vm);

            var actualizada = await _personas.ActualizarAsync(cc, new PersonaActualizarDto
            {
                Nombre = vm.Nombre,
                Apellido = vm.Apellido,
                Genero = vm.Genero,
                Edad = vm.Edad!.Value
            });
            if (!actualizada)
                return NotFound();

            TempData["Mensaje"] = $"Se actualizó la persona {vm.Nombre} {vm.Apellido}.";
            return RedirectToAction("Listar", "PersonaListar");
        }

        private ViewResult Vista(PersonaEditarViewModel vm) => View("~/Views/Persona/Editar.cshtml", vm);
    }
}
