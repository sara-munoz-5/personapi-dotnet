using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Models.DTOs;
using personapi_dotnet.Models.ViewModels.Estudios;

namespace personapi_dotnet.Controllers.ViewControllers.Estudios
{
    /// <summary>Controller View de Views/Estudios/Crear.cshtml. Persona y profesión se eligen de listas.</summary>
    public class EstudiosCrearController : Controller
    {
        private readonly IEstudiosService _estudios;
        private readonly IPersonaService _personas;
        private readonly IProfesionService _profesiones;

        public EstudiosCrearController(IEstudiosService estudios, IPersonaService personas, IProfesionService profesiones)
        {
            _estudios = estudios;
            _personas = personas;
            _profesiones = profesiones;
        }

        [HttpGet("Estudios/Crear")]
        public async Task<IActionResult> Crear()
        {
            return await VistaAsync(new EstudiosCrearViewModel());
        }

        [HttpPost("Estudios/Crear")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(EstudiosCrearViewModel vm)
        {
            if (!ModelState.IsValid)
                return await VistaAsync(vm);

            try
            {
                await _estudios.CrearAsync(new EstudiosCrearDto
                {
                    IdProf = vm.IdProf!.Value,
                    CcPer = vm.CcPer!.Value,
                    Fecha = vm.Fecha,
                    Univer = vm.Univer
                });
            }
            catch (ConflictoNegocioException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Tipo == TipoConflicto.LlaveDuplicada
                    ? "Esa persona ya tiene registrado un estudio de esa profesión."
                    : "La persona o la profesión seleccionada ya no existe.");
                return await VistaAsync(vm);
            }

            TempData["Mensaje"] = "Se creó el estudio.";
            return RedirectToAction("Listar", "EstudiosListar");
        }

        // Recarga las listas en cada respuesta, también cuando el formulario vuelve con errores.
        private async Task<ViewResult> VistaAsync(EstudiosCrearViewModel vm)
        {
            vm.Personas = (await _personas.ObtenerTodosAsync())
                .OrderBy(p => p.Apellido).ThenBy(p => p.Nombre)
                .Select(p => new SelectListItem($"{p.Nombre} {p.Apellido} ({p.Cc})", p.Cc.ToString()))
                .ToList();
            vm.Profesiones = (await _profesiones.ObtenerTodosAsync())
                .OrderBy(p => p.Nom)
                .Select(p => new SelectListItem(p.Nom, p.Id.ToString()))
                .ToList();

            return View("~/Views/Estudios/Crear.cshtml", vm);
        }
    }
}
