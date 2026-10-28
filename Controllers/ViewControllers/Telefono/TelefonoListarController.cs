using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Models.ViewModels.Telefono;

namespace personapi_dotnet.Controllers.ViewControllers.Telefono
{
    /// <summary>Controller View de Views/Telefono/Listar.cshtml. Usa el servicio de personas para mostrar el dueño.</summary>
    [ApiExplorerSettings(IgnoreApi = true)] // Controller View: fuera de Swagger, que documenta solo la API
    public class TelefonoListarController : Controller
    {
        private readonly ITelefonoService _telefonos;
        private readonly IPersonaService _personas;

        public TelefonoListarController(ITelefonoService telefonos, IPersonaService personas)
        {
            _telefonos = telefonos;
            _personas = personas;
        }

        [HttpGet("Telefonos")]
        public async Task<IActionResult> Listar()
        {
            var telefonos = await _telefonos.ObtenerTodosAsync();
            var personas = (await _personas.ObtenerTodosAsync()).ToDictionary(p => p.Cc, p => $"{p.Nombre} {p.Apellido}");

            var vm = new TelefonoListarViewModel
            {
                Mensaje = TempData["Mensaje"] as string,
                Telefonos = telefonos
                    .OrderBy(t => t.Num)
                    .Select(t => new TelefonoListarViewModel.Fila
                    {
                        Num = t.Num,
                        Oper = t.Oper,
                        Duenio = t.Duenio,
                        NombreDuenio = personas.GetValueOrDefault(t.Duenio, t.Duenio.ToString())
                    })
                    .ToList()
            };

            return View("~/Views/Telefono/Listar.cshtml", vm);
        }
    }
}
