using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Models.ViewModels.Estudios;

namespace personapi_dotnet.Controllers.ViewControllers.Estudios
{
    /// <summary>Controller View de Views/Estudios/Detalle.cshtml (llave compuesta en la URL).</summary>
    [ApiExplorerSettings(IgnoreApi = true)] // Controller View: fuera de Swagger, que documenta solo la API
    public class EstudiosDetalleController : Controller
    {
        private readonly IEstudiosService _estudios;
        private readonly IPersonaService _personas;
        private readonly IProfesionService _profesiones;

        public EstudiosDetalleController(IEstudiosService estudios, IPersonaService personas, IProfesionService profesiones)
        {
            _estudios = estudios;
            _personas = personas;
            _profesiones = profesiones;
        }

        [HttpGet("Estudios/Detalle/{idProf:int}/{ccPer:int}")]
        public async Task<IActionResult> Detalle(int idProf, int ccPer)
        {
            var estudio = await _estudios.ObtenerPorIdAsync(idProf, ccPer);
            if (estudio == null)
                return NotFound();

            var persona = await _personas.ObtenerPorIdAsync(ccPer);
            var profesion = await _profesiones.ObtenerPorIdAsync(idProf);

            return View("~/Views/Estudios/Detalle.cshtml", new EstudiosDetalleViewModel
            {
                IdProf = estudio.IdProf,
                CcPer = estudio.CcPer,
                Profesion = profesion?.Nom ?? idProf.ToString(),
                Persona = persona == null ? ccPer.ToString() : $"{persona.Nombre} {persona.Apellido}",
                Fecha = estudio.Fecha,
                Univer = estudio.Univer
            });
        }
    }
}
