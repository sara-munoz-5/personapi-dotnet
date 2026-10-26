using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Models.ViewModels.Estudios;

namespace personapi_dotnet.Controllers.ViewControllers.Estudios
{
    /// <summary>Controller View de Views/Estudios/Eliminar.cshtml (llave compuesta en la URL).</summary>
    public class EstudiosEliminarController : Controller
    {
        private readonly IEstudiosService _estudios;
        private readonly IPersonaService _personas;
        private readonly IProfesionService _profesiones;

        public EstudiosEliminarController(IEstudiosService estudios, IPersonaService personas, IProfesionService profesiones)
        {
            _estudios = estudios;
            _personas = personas;
            _profesiones = profesiones;
        }

        [HttpGet("Estudios/Eliminar/{idProf:int}/{ccPer:int}")]
        public async Task<IActionResult> Eliminar(int idProf, int ccPer)
        {
            var estudio = await _estudios.ObtenerPorIdAsync(idProf, ccPer);
            if (estudio == null)
                return NotFound();

            var persona = await _personas.ObtenerPorIdAsync(ccPer);
            var profesion = await _profesiones.ObtenerPorIdAsync(idProf);

            return View("~/Views/Estudios/Eliminar.cshtml", new EstudiosEliminarViewModel
            {
                IdProf = estudio.IdProf,
                CcPer = estudio.CcPer,
                Profesion = profesion?.Nom ?? idProf.ToString(),
                Persona = persona == null ? ccPer.ToString() : $"{persona.Nombre} {persona.Apellido}",
                Fecha = estudio.Fecha,
                Univer = estudio.Univer
            });
        }

        [HttpPost("Estudios/Eliminar/{idProf:int}/{ccPer:int}")]
        [ValidateAntiForgeryToken]
        [ActionName("Eliminar")]
        public async Task<IActionResult> EliminarConfirmado([FromRoute] int idProf, [FromRoute] int ccPer)
        {
            if (!await _estudios.EliminarAsync(idProf, ccPer))
                return NotFound();

            TempData["Mensaje"] = "Se eliminó el estudio.";
            return RedirectToAction("Listar", "EstudiosListar");
        }
    }
}
