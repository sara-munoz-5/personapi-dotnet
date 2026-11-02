using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Models.DTOs;
using personapi_dotnet.Models.ViewModels.Profesion;

namespace personapi_dotnet.Controllers.ViewControllers.Profesion
{
    /// <summary>Controller View de Views/Profesion/Editar.cshtml. El id sale de la URL, nunca del formulario.</summary>
    [ApiExplorerSettings(IgnoreApi = true)] // Controller View: fuera de Swagger, que documenta solo la API
    public class ProfesionEditarController : Controller
    {
        private readonly IProfesionService _profesiones;

        public ProfesionEditarController(IProfesionService profesiones)
        {
            _profesiones = profesiones;
        }

        [HttpGet("Profesiones/Editar/{id:int}")]
        public async Task<IActionResult> Editar(int id)
        {
            var profesion = await _profesiones.ObtenerPorIdAsync(id);
            if (profesion == null)
                return NotFound();

            return Vista(new ProfesionEditarViewModel { Id = profesion.Id, Nom = profesion.Nom, Des = profesion.Des });
        }

        [HttpPost("Profesiones/Editar/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar([FromRoute] int id, ProfesionEditarViewModel vm)
        {
            vm.Id = id;
            if (!ModelState.IsValid)
                return Vista(vm);

            var actualizada = await _profesiones.ActualizarAsync(id, new ProfesionActualizarDto { Nom = vm.Nom, Des = vm.Des });
            if (!actualizada)
                return NotFound();

            TempData["Mensaje"] = $"Se actualizó la profesión {vm.Nom}.";
            return RedirectToAction("Listar", "ProfesionListar");
        }

        private ViewResult Vista(ProfesionEditarViewModel vm) => View("~/Views/Profesion/Editar.cshtml", vm);
    }
}
