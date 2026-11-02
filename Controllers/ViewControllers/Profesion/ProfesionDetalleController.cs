using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Models.ViewModels.Profesion;

namespace personapi_dotnet.Controllers.ViewControllers.Profesion
{
    /// <summary>Controller View de Views/Profesion/Detalle.cshtml.</summary>
    [ApiExplorerSettings(IgnoreApi = true)] // Controller View: fuera de Swagger, que documenta solo la API
    public class ProfesionDetalleController : Controller
    {
        private readonly IProfesionService _profesiones;

        public ProfesionDetalleController(IProfesionService profesiones)
        {
            _profesiones = profesiones;
        }

        [HttpGet("Profesiones/Detalle/{id:int}")]
        public async Task<IActionResult> Detalle(int id)
        {
            var profesion = await _profesiones.ObtenerPorIdAsync(id);
            if (profesion == null)
                return NotFound();

            return View("~/Views/Profesion/Detalle.cshtml", new ProfesionDetalleViewModel
            {
                Id = profesion.Id,
                Nom = profesion.Nom,
                Des = profesion.Des
            });
        }
    }
}
