using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Interfaces;
using personapi_dotnet.Models.Entities;
using personapi_dotnet.ViewModels;

namespace personapi_dotnet.Controllers;

/// <summary>
/// Interfaz MVC del CRUD de teléfonos. Usa el mismo repositorio que TelefonoApiController
/// y el de personas para la lista de dueños.
/// </summary>
public class TelefonoController : Controller
{
    private readonly ITelefonoRepository _telefonos;
    private readonly IPersonaRepository _personas;

    public TelefonoController(ITelefonoRepository telefonos, IPersonaRepository personas)
    {
        _telefonos = telefonos;
        _personas = personas;
    }

    // GET: /Telefono
    public async Task<IActionResult> Index()
    {
        return View(await _telefonos.GetAllAsync());
    }

    // GET: /Telefono/Details/{num}
    public async Task<IActionResult> Details(string? id)
    {
        var telefono = await BuscarAsync(id);
        if (telefono is null)
        {
            return NotFound();
        }

        return View(telefono);
    }

    // GET: /Telefono/Create
    public async Task<IActionResult> Create()
    {
        return View(await ArmarFormularioAsync(new Telefono()));
    }

    // POST: /Telefono/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Num,Oper,Duenio", Prefix = nameof(TelefonoFormViewModel.Telefono))] Telefono telefono)
    {
        if (ModelState.IsValid && await _telefonos.GetByIdAsync(telefono.Num) is not null)
        {
            ModelState.AddModelError($"{nameof(TelefonoFormViewModel.Telefono)}.{nameof(Telefono.Num)}", "El número de teléfono ya existe.");
        }

        if (!ModelState.IsValid)
        {
            // La lista de dueños no viaja en el formulario: se recarga antes de devolver la vista
            return View(await ArmarFormularioAsync(telefono));
        }

        try
        {
            await _telefonos.AddAsync(telefono);
        }
        catch (DbUpdateException)
        {
            // EF Core lanza DbUpdateException si el número quedó repetido o el dueño no existe
            ModelState.AddModelError(string.Empty, "No se pudo guardar el teléfono. Verifique que el número no esté repetido y que el dueño exista.");
            return View(await ArmarFormularioAsync(telefono));
        }

        TempData["Mensaje"] = "Teléfono creado.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Telefono/Edit/{num}
    public async Task<IActionResult> Edit(string? id)
    {
        var telefono = await BuscarAsync(id);
        if (telefono is null)
        {
            return NotFound();
        }

        return View(await ArmarFormularioAsync(telefono));
    }

    // POST: /Telefono/Edit/{num}
    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost([FromRoute] string? id)
    {
        var telefono = await BuscarAsync(id);
        if (telefono is null)
        {
            return NotFound();
        }

        // Contra la sobrepublicación: el número es la llave; solo se editan operador y dueño
        if (await TryUpdateModelAsync(telefono, nameof(TelefonoFormViewModel.Telefono), t => t.Oper, t => t.Duenio))
        {
            try
            {
                await _telefonos.UpdateAsync(telefono);
                TempData["Mensaje"] = "Teléfono actualizado.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "No se pudo actualizar el teléfono. Verifique que el dueño exista.");
            }
        }

        return View(await ArmarFormularioAsync(telefono));
    }

    // GET: /Telefono/Delete/{num}
    public async Task<IActionResult> Delete(string? id)
    {
        var telefono = await BuscarAsync(id);
        if (telefono is null)
        {
            return NotFound();
        }

        return View(telefono);
    }

    // POST: /Telefono/Delete/{num}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed([FromRoute] string id)
    {
        if (await _telefonos.GetByIdAsync(id) is null)
        {
            return NotFound();
        }

        try
        {
            await _telefonos.DeleteAsync(id);
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "No se pudo eliminar el teléfono. Intente de nuevo.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        TempData["Mensaje"] = "Teléfono eliminado.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<Telefono?> BuscarAsync(string? num)
    {
        if (string.IsNullOrWhiteSpace(num))
        {
            return null;
        }

        return await _telefonos.GetByIdAsync(num);
    }

    private async Task<TelefonoFormViewModel> ArmarFormularioAsync(Telefono telefono)
    {
        var personas = await _personas.GetAllAsync();

        return new TelefonoFormViewModel
        {
            Telefono = telefono,
            Personas = personas
                .OrderBy(p => p.Nombre).ThenBy(p => p.Apellido)
                .Select(p => new SelectListItem($"{p.Nombre} {p.Apellido}", p.Cc.ToString()))
        };
    }
}
