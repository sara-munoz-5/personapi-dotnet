using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Models.ViewModels.Persona;

namespace personapi_dotnet.Controllers.ViewControllers.Persona
{
    /// <summary>Controller View de Views/Persona/Listar.cshtml.</summary>
    [ApiExplorerSettings(IgnoreApi = true)] // Controller View: fuera de Swagger, que documenta solo la API
    public class PersonaListarController : Controller
    {
        private readonly IPersonaService _personas;

        public PersonaListarController(IPersonaService personas)
        {
            _personas = personas;
        }

        [HttpGet("Personas")]
        public async Task<IActionResult> Listar()
        {
            var personas = await _personas.ObtenerTodosAsync();

            var vm = new PersonaListarViewModel
            {
                Mensaje = TempData["Mensaje"] as string,
                Personas = personas
                    .OrderBy(p => p.Cc)
                    .Select(p => new PersonaListarViewModel.Fila
                    {
                        Cc = p.Cc,
                        Nombre = p.Nombre,
                        Apellido = p.Apellido,
                        Genero = Generos.Texto(p.Genero),
                        Edad = p.Edad
                    })
                    .ToList()
            };

            return View("~/Views/Persona/Listar.cshtml", vm);
        }
    }
}
