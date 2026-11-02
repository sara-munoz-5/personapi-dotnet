using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Models.ViewModels.Telefono;

namespace personapi_dotnet.Controllers.ViewControllers.Telefono
{
    /// <summary>Controller View de Views/Telefono/Detalle.cshtml.</summary>
    [ApiExplorerSettings(IgnoreApi = true)] // Controller View: fuera de Swagger, que documenta solo la API
    public class TelefonoDetalleController : Controller
    {
        private readonly ITelefonoService _telefonos;
        private readonly IPersonaService _personas;

        public TelefonoDetalleController(ITelefonoService telefonos, IPersonaService personas)
        {
            _telefonos = telefonos;
            _personas = personas;
        }

        [HttpGet("Telefonos/Detalle/{num}")]
        public async Task<IActionResult> Detalle(string num)
        {
            var telefono = await _telefonos.ObtenerPorIdAsync(num);
            if (telefono == null)
                return NotFound();

            var duenio = await _personas.ObtenerPorIdAsync(telefono.Duenio);

            return View("~/Views/Telefono/Detalle.cshtml", new TelefonoDetalleViewModel
            {
                Num = telefono.Num,
                Oper = telefono.Oper,
                Duenio = telefono.Duenio,
                NombreDuenio = duenio == null ? telefono.Duenio.ToString() : $"{duenio.Nombre} {duenio.Apellido}"
            });
        }
    }
}
