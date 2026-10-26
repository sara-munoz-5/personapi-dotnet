using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Models.DTOs;
using personapi_dotnet.Models.ViewModels.Telefono;

namespace personapi_dotnet.Controllers.ViewControllers.Telefono
{
    /// <summary>Controller View de Views/Telefono/Crear.cshtml. El dueño se elige de la lista de personas.</summary>
    public class TelefonoCrearController : Controller
    {
        private readonly ITelefonoService _telefonos;
        private readonly IPersonaService _personas;

        public TelefonoCrearController(ITelefonoService telefonos, IPersonaService personas)
        {
            _telefonos = telefonos;
            _personas = personas;
        }

        [HttpGet("Telefonos/Crear")]
        public async Task<IActionResult> Crear()
        {
            return await VistaAsync(new TelefonoCrearViewModel());
        }

        [HttpPost("Telefonos/Crear")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(TelefonoCrearViewModel vm)
        {
            if (!ModelState.IsValid)
                return await VistaAsync(vm);

            try
            {
                await _telefonos.CrearAsync(new TelefonoCrearDto
                {
                    Num = vm.Num,
                    Oper = vm.Oper,
                    Duenio = vm.Duenio!.Value
                });
            }
            catch (ConflictoNegocioException ex) when (ex.Tipo == TipoConflicto.LlaveDuplicada)
            {
                ModelState.AddModelError(nameof(vm.Num), "Ya existe un teléfono con ese número.");
                return await VistaAsync(vm);
            }
            catch (ConflictoNegocioException ex) when (ex.Tipo == TipoConflicto.ReferenciaInexistente)
            {
                ModelState.AddModelError(nameof(vm.Duenio), "La persona seleccionada como dueño ya no existe.");
                return await VistaAsync(vm);
            }

            TempData["Mensaje"] = $"Se creó el teléfono {vm.Num}.";
            return RedirectToAction("Listar", "TelefonoListar");
        }

        // Recarga la lista de personas en cada respuesta, también cuando el formulario vuelve con errores.
        private async Task<ViewResult> VistaAsync(TelefonoCrearViewModel vm)
        {
            vm.Personas = (await _personas.ObtenerTodosAsync())
                .OrderBy(p => p.Apellido).ThenBy(p => p.Nombre)
                .Select(p => new SelectListItem($"{p.Nombre} {p.Apellido} ({p.Cc})", p.Cc.ToString()))
                .ToList();

            return View("~/Views/Telefono/Crear.cshtml", vm);
        }
    }
}
