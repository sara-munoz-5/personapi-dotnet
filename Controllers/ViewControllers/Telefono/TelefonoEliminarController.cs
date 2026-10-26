using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Models.ViewModels.Telefono;

namespace personapi_dotnet.Controllers.ViewControllers.Telefono
{
    /// <summary>Controller View de Views/Telefono/Eliminar.cshtml.</summary>
    public class TelefonoEliminarController : Controller
    {
        private readonly ITelefonoService _telefonos;
        private readonly IPersonaService _personas;

        public TelefonoEliminarController(ITelefonoService telefonos, IPersonaService personas)
        {
            _telefonos = telefonos;
            _personas = personas;
        }

        [HttpGet("Telefonos/Eliminar/{num}")]
        public async Task<IActionResult> Eliminar(string num)
        {
            var telefono = await _telefonos.ObtenerPorIdAsync(num);
            if (telefono == null)
                return NotFound();

            var duenio = await _personas.ObtenerPorIdAsync(telefono.Duenio);

            return View("~/Views/Telefono/Eliminar.cshtml", new TelefonoEliminarViewModel
            {
                Num = telefono.Num,
                Oper = telefono.Oper,
                NombreDuenio = duenio == null ? telefono.Duenio.ToString() : $"{duenio.Nombre} {duenio.Apellido}"
            });
        }

        [HttpPost("Telefonos/Eliminar/{num}")]
        [ValidateAntiForgeryToken]
        [ActionName("Eliminar")]
        public async Task<IActionResult> EliminarConfirmado([FromRoute] string num)
        {
            if (!await _telefonos.EliminarAsync(num))
                return NotFound();

            TempData["Mensaje"] = $"Se eliminó el teléfono {num}.";
            return RedirectToAction("Listar", "TelefonoListar");
        }
    }
}
