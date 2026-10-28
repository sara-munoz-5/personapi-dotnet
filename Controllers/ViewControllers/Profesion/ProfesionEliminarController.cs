using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Models.ViewModels.Profesion;

namespace personapi_dotnet.Controllers.ViewControllers.Profesion
{
    /// <summary>
    /// Controller View de Views/Profesion/Eliminar.cshtml. Sin cascada (contratos §2.2): si la profesión
    /// tiene estudios, el servicio lo impide y la vista lo explica.
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)] // Controller View: fuera de Swagger, que documenta solo la API
    public class ProfesionEliminarController : Controller
    {
        private readonly IProfesionService _profesiones;

        public ProfesionEliminarController(IProfesionService profesiones)
        {
            _profesiones = profesiones;
        }

        [HttpGet("Profesiones/Eliminar/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var vm = await CargarAsync(id);
            return vm == null ? NotFound() : Vista(vm);
        }

        [HttpPost("Profesiones/Eliminar/{id:int}")]
        [ValidateAntiForgeryToken]
        [ActionName("Eliminar")]
        public async Task<IActionResult> EliminarConfirmado([FromRoute] int id)
        {
            try
            {
                if (!await _profesiones.EliminarAsync(id))
                    return NotFound();
            }
            catch (ConflictoNegocioException ex) when (ex.Tipo == TipoConflicto.BorradoConReferencias)
            {
                var vm = await CargarAsync(id);
                if (vm == null)
                    return NotFound();

                ModelState.AddModelError(string.Empty,
                    "No se puede eliminar la profesión porque hay estudios registrados con ella. Elimínelos primero.");
                return Vista(vm);
            }

            TempData["Mensaje"] = $"Se eliminó la profesión con id {id}.";
            return RedirectToAction("Listar", "ProfesionListar");
        }

        private async Task<ProfesionEliminarViewModel?> CargarAsync(int id)
        {
            var profesion = await _profesiones.ObtenerPorIdAsync(id);
            return profesion == null
                ? null
                : new ProfesionEliminarViewModel { Id = profesion.Id, Nom = profesion.Nom, Des = profesion.Des };
        }

        private ViewResult Vista(ProfesionEliminarViewModel vm) => View("~/Views/Profesion/Eliminar.cshtml", vm);
    }
}
