using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Models.ViewModels.Profesion;

namespace personapi_dotnet.Controllers.ViewControllers.Profesion
{
    /// <summary>Controller View de Views/Profesion/Listar.cshtml.</summary>
    [ApiExplorerSettings(IgnoreApi = true)] // Controller View: fuera de Swagger, que documenta solo la API
    public class ProfesionListarController : Controller
    {
        private readonly IProfesionService _profesiones;

        public ProfesionListarController(IProfesionService profesiones)
        {
            _profesiones = profesiones;
        }

        [HttpGet("Profesiones")]
        public async Task<IActionResult> Listar()
        {
            var profesiones = await _profesiones.ObtenerTodosAsync();

            var vm = new ProfesionListarViewModel
            {
                Mensaje = TempData["Mensaje"] as string,
                Profesiones = profesiones
                    .OrderBy(p => p.Id)
                    .Select(p => new ProfesionListarViewModel.Fila { Id = p.Id, Nom = p.Nom, Des = p.Des })
                    .ToList()
            };

            return View("~/Views/Profesion/Listar.cshtml", vm);
        }
    }
}
