using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Models.ViewModels.Estudios;

namespace personapi_dotnet.Controllers.ViewControllers.Estudios
{
    /// <summary>
    /// Controller View de Views/Estudios/Listar.cshtml. Usa los servicios de persona y profesión
    /// para mostrar nombres en lugar de las llaves.
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)] // Controller View: fuera de Swagger, que documenta solo la API
    public class EstudiosListarController : Controller
    {
        private readonly IEstudiosService _estudios;
        private readonly IPersonaService _personas;
        private readonly IProfesionService _profesiones;

        public EstudiosListarController(IEstudiosService estudios, IPersonaService personas, IProfesionService profesiones)
        {
            _estudios = estudios;
            _personas = personas;
            _profesiones = profesiones;
        }

        [HttpGet("Estudios")]
        public async Task<IActionResult> Listar()
        {
            var estudios = await _estudios.ObtenerTodosAsync();
            var personas = (await _personas.ObtenerTodosAsync()).ToDictionary(p => p.Cc, p => $"{p.Nombre} {p.Apellido}");
            var profesiones = (await _profesiones.ObtenerTodosAsync()).ToDictionary(p => p.Id, p => p.Nom);

            var vm = new EstudiosListarViewModel
            {
                Mensaje = TempData["Mensaje"] as string,
                Estudios = estudios
                    .OrderBy(e => e.CcPer).ThenBy(e => e.IdProf)
                    .Select(e => new EstudiosListarViewModel.Fila
                    {
                        IdProf = e.IdProf,
                        CcPer = e.CcPer,
                        Profesion = profesiones.GetValueOrDefault(e.IdProf, e.IdProf.ToString()),
                        Persona = personas.GetValueOrDefault(e.CcPer, e.CcPer.ToString()),
                        Fecha = e.Fecha,
                        Univer = e.Univer
                    })
                    .ToList()
            };

            return View("~/Views/Estudios/Listar.cshtml", vm);
        }
    }
}
