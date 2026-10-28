using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Models.DTOs;
using personapi_dotnet.Models.ViewModels.Profesion;

namespace personapi_dotnet.Controllers.ViewControllers.Profesion
{
    /// <summary>Controller View de Views/Profesion/Crear.cshtml (GET formulario, POST crear).</summary>
    [ApiExplorerSettings(IgnoreApi = true)] // Controller View: fuera de Swagger, que documenta solo la API
    public class ProfesionCrearController : Controller
    {
        private readonly IProfesionService _profesiones;

        public ProfesionCrearController(IProfesionService profesiones)
        {
            _profesiones = profesiones;
        }

        [HttpGet("Profesiones/Crear")]
        public IActionResult Crear()
        {
            return Vista(new ProfesionCrearViewModel());
        }

        [HttpPost("Profesiones/Crear")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(ProfesionCrearViewModel vm)
        {
            if (!ModelState.IsValid)
                return Vista(vm);

            try
            {
                await _profesiones.CrearAsync(new ProfesionCrearDto
                {
                    Id = vm.Id!.Value,
                    Nom = vm.Nom,
                    Des = vm.Des
                });
            }
            catch (ConflictoNegocioException ex) when (ex.Tipo == TipoConflicto.LlaveDuplicada)
            {
                ModelState.AddModelError(nameof(vm.Id), "Ya existe una profesión con ese id.");
                return Vista(vm);
            }

            TempData["Mensaje"] = $"Se creó la profesión {vm.Nom}.";
            return RedirectToAction("Listar", "ProfesionListar");
        }

        private ViewResult Vista(ProfesionCrearViewModel vm) => View("~/Views/Profesion/Crear.cshtml", vm);
    }
}
