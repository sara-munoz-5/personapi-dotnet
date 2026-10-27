using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Models.ViewModels.Persona;

namespace personapi_dotnet.Controllers.ViewControllers.Persona
{
    /// <summary>
    /// Controller View de Views/Persona/Detalle.cshtml. Los DTO no traen navegaciones (contratos §5),
    /// así que los estudios y teléfonos de la persona se piden a sus propios servicios.
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)] // Controller View: fuera de Swagger, que documenta solo la API
    public class PersonaDetalleController : Controller
    {
        private readonly IPersonaService _personas;
        private readonly IEstudiosService _estudios;
        private readonly ITelefonoService _telefonos;
        private readonly IProfesionService _profesiones;

        public PersonaDetalleController(
            IPersonaService personas,
            IEstudiosService estudios,
            ITelefonoService telefonos,
            IProfesionService profesiones)
        {
            _personas = personas;
            _estudios = estudios;
            _telefonos = telefonos;
            _profesiones = profesiones;
        }

        [HttpGet("Personas/Detalle/{cc:int}")]
        public async Task<IActionResult> Detalle(int cc)
        {
            var persona = await _personas.ObtenerPorIdAsync(cc);
            if (persona == null)
                return NotFound();

            var profesiones = (await _profesiones.ObtenerTodosAsync()).ToDictionary(p => p.Id, p => p.Nom);
            var estudios = (await _estudios.ObtenerTodosAsync()).Where(e => e.CcPer == cc);
            var telefonos = (await _telefonos.ObtenerTodosAsync()).Where(t => t.Duenio == cc);

            var vm = new PersonaDetalleViewModel
            {
                Cc = persona.Cc,
                Nombre = persona.Nombre,
                Apellido = persona.Apellido,
                Genero = Generos.Texto(persona.Genero),
                Edad = persona.Edad,
                Estudios = estudios
                    .Select(e => new PersonaDetalleViewModel.EstudioFila
                    {
                        IdProf = e.IdProf,
                        Profesion = profesiones.GetValueOrDefault(e.IdProf, e.IdProf.ToString()),
                        Univer = e.Univer,
                        Fecha = e.Fecha
                    })
                    .ToList(),
                Telefonos = telefonos
                    .Select(t => new PersonaDetalleViewModel.TelefonoFila { Num = t.Num, Oper = t.Oper })
                    .ToList()
            };

            return View("~/Views/Persona/Detalle.cshtml", vm);
        }
    }
}
