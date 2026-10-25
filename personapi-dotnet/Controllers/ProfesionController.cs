using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Interfaces;
using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.Controllers;

/// <summary>
/// Interfaz MVC del CRUD de profesiones. Usa el mismo repositorio que ProfesionApiController.
/// </summary>
public class ProfesionController : Controller
{
    private readonly IProfesionRepository _profesiones;

    public ProfesionController(IProfesionRepository profesiones)
    {
        _profesiones = profesiones;
    }

    // GET: /Profesion
    public async Task<IActionResult> Index()
    {
        return View(await _profesiones.GetAllAsync());
    }

    // GET: /Profesion/Details/{id}
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var profesion = await _profesiones.GetByIdAsync(id.Value);
        if (profesion is null)
        {
            return NotFound();
        }

        return View(profesion);
    }

    // GET: /Profesion/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Profesion/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Nom,Des")] Profesion profesion)
    {
        if (ModelState.IsValid && await _profesiones.GetByIdAsync(profesion.Id) is not null)
        {
            ModelState.AddModelError(nameof(Profesion.Id), "El id de la profesión ya existe.");
        }

        if (!ModelState.IsValid)
        {
            return View(profesion);
        }

        try
        {
            await _profesiones.AddAsync(profesion);
        }
        catch (DbUpdateException)
        {
            // EF Core lanza DbUpdateException si el id quedó repetido u otra restricción falló
            ModelState.AddModelError(string.Empty, "No se pudo guardar la profesión. Verifique que el id no esté repetido.");
            return View(profesion);
        }

        TempData["Mensaje"] = "Profesión creada.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Profesion/Edit/{id}
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var profesion = await _profesiones.GetByIdAsync(id.Value);
        if (profesion is null)
        {
            return NotFound();
        }

        return View(profesion);
    }

    // POST: /Profesion/Edit/{id}
    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost([FromRoute] int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var profesion = await _profesiones.GetByIdAsync(id.Value);
        if (profesion is null)
        {
            return NotFound();
        }

        // Contra la sobrepublicación: solo se copian estos campos; el id es la llave y no se edita
        if (await TryUpdateModelAsync(profesion, string.Empty, p => p.Nom, p => p.Des))
        {
            try
            {
                await _profesiones.UpdateAsync(profesion);
                TempData["Mensaje"] = "Profesión actualizada.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "No se pudo actualizar la profesión.");
            }
        }

        return View(profesion);
    }

    // GET: /Profesion/Delete/{id}
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var profesion = await _profesiones.GetByIdAsync(id.Value);
        if (profesion is null)
        {
            return NotFound();
        }

        return View(profesion);
    }

    // POST: /Profesion/Delete/{id}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed([FromRoute] int id)
    {
        if (await _profesiones.GetByIdAsync(id) is null)
        {
            return NotFound();
        }

        try
        {
            await _profesiones.DeleteAsync(id);
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "No se pudo eliminar la profesión. Intente de nuevo.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        TempData["Mensaje"] = "Profesión eliminada junto con sus estudios.";
        return RedirectToAction(nameof(Index));
    }
}
