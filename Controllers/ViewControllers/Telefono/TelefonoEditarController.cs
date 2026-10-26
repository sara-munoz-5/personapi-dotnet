using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Models.DTOs;
using personapi_dotnet.Models.ViewModels.Telefono;

namespace personapi_dotnet.Controllers.ViewControllers.Telefono
{
    /// <summary>
    /// Controller View de Views/Telefono/Editar.cshtml. El número sale de la URL y no se modifica;
    /// cambian el operador y el dueño (contratos §4.4).
    /// </summary>
    public class TelefonoEditarController : Controller
    {
        private readonly ITelefonoService _telefonos;
        private readonly IPersonaService _personas;

        public TelefonoEditarController(ITelefonoService telefonos, IPersonaService personas)
        {
            _telefonos = telefonos;
            _personas = personas;
        }

        [HttpGet("Telefonos/Editar/{num}")]
        public async Task<IActionResult> Editar(string num)
        {
            var telefono = await _telefonos.ObtenerPorIdAsync(num);
            if (telefono == null)
                return NotFound();

            return await VistaAsync(new TelefonoEditarViewModel
            {
                Num = telefono.Num,
                Oper = telefono.Oper,
                Duenio = telefono.Duenio
            });
        }

        [HttpPost("Telefonos/Editar/{num}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar([FromRoute] string num, TelefonoEditarViewModel vm)
        {
            vm.Num = num;
            if (!ModelState.IsValid)
                return await VistaAsync(vm);

            bool actualizado;
            try
            {
                actualizado = await _telefonos.ActualizarAsync(num, new TelefonoActualizarDto
                {
                    Oper = vm.Oper,
                    Duenio = vm.Duenio!.Value
                });
            }
            catch (ConflictoNegocioException ex) when (ex.Tipo == TipoConflicto.ReferenciaInexistente)
            {
                ModelState.AddModelError(nameof(vm.Duenio), "La persona seleccionada como dueño ya no existe.");
                return await VistaAsync(vm);
            }
            if (!actualizado)
                return NotFound();

            TempData["Mensaje"] = $"Se actualizó el teléfono {num}.";
            return RedirectToAction("Listar", "TelefonoListar");
        }

        // Recarga la lista de personas en cada respuesta, también cuando el formulario vuelve con errores.
        private async Task<ViewResult> VistaAsync(TelefonoEditarViewModel vm)
        {
            vm.Personas = (await _personas.ObtenerTodosAsync())
                .OrderBy(p => p.Apellido).ThenBy(p => p.Nombre)
                .Select(p => new SelectListItem($"{p.Nombre} {p.Apellido} ({p.Cc})", p.Cc.ToString()))
                .ToList();

            return View("~/Views/Telefono/Editar.cshtml", vm);
        }
    }
}
