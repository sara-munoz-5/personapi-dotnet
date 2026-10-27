using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Models.DTOs;
using personapi_dotnet.Models.ViewModels.Estudios;

namespace personapi_dotnet.Controllers.ViewControllers.Estudios
{
    /// <summary>
    /// Controller View de Views/Estudios/Editar.cshtml. Las dos partes de la llave salen de la URL
    /// y no se modifican; solo cambian fecha y universidad (contratos §4.3).
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)] // Controller View: fuera de Swagger, que documenta solo la API
    public class EstudiosEditarController : Controller
    {
        private readonly IEstudiosService _estudios;
        private readonly IPersonaService _personas;
        private readonly IProfesionService _profesiones;

        public EstudiosEditarController(IEstudiosService estudios, IPersonaService personas, IProfesionService profesiones)
        {
            _estudios = estudios;
            _personas = personas;
            _profesiones = profesiones;
        }

        [HttpGet("Estudios/Editar/{idProf:int}/{ccPer:int}")]
        public async Task<IActionResult> Editar(int idProf, int ccPer)
        {
            var estudio = await _estudios.ObtenerPorIdAsync(idProf, ccPer);
            if (estudio == null)
                return NotFound();

            return await VistaAsync(new EstudiosEditarViewModel
            {
                IdProf = estudio.IdProf,
                CcPer = estudio.CcPer,
                Fecha = estudio.Fecha,
                Univer = estudio.Univer
            });
        }

        [HttpPost("Estudios/Editar/{idProf:int}/{ccPer:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar([FromRoute] int idProf, [FromRoute] int ccPer, EstudiosEditarViewModel vm)
        {
            vm.IdProf = idProf;
            vm.CcPer = ccPer;
            if (!ModelState.IsValid)
                return await VistaAsync(vm);

            var actualizado = await _estudios.ActualizarAsync(idProf, ccPer, new EstudiosActualizarDto
            {
                Fecha = vm.Fecha,
                Univer = vm.Univer
            });
            if (!actualizado)
                return NotFound();

            TempData["Mensaje"] = "Se actualizó el estudio.";
            return RedirectToAction("Listar", "EstudiosListar");
        }

        // Recarga las listas en cada respuesta, también cuando el formulario vuelve con errores.
        private async Task<ViewResult> VistaAsync(EstudiosEditarViewModel vm)
        {
            vm.Personas = (await _personas.ObtenerTodosAsync())
                .Select(p => new SelectListItem($"{p.Nombre} {p.Apellido} ({p.Cc})", p.Cc.ToString()))
                .ToList();
            vm.Profesiones = (await _profesiones.ObtenerTodosAsync())
                .Select(p => new SelectListItem(p.Nom, p.Id.ToString()))
                .ToList();

            return View("~/Views/Estudios/Editar.cshtml", vm);
        }
    }
}
