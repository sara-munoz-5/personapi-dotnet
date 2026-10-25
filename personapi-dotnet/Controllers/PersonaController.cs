using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Interfaces;
using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.Controllers;

/// <summary>
/// Interfaz MVC del CRUD de personas. Usa el mismo repositorio que PersonaApiController.
/// </summary>
public class PersonaController : Controller
{
    private readonly IPersonaRepository _personas;

    public PersonaController(IPersonaRepository personas)
    {
        _personas = personas;
    }

    // GET: /Persona
    public async Task<IActionResult> Index()
    {
        return View(await _personas.GetAllAsync());
    }

    // GET: /Persona/Details/{cc}
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var persona = await _personas.GetByIdAsync(id.Value);
        if (persona is null)
        {
            return NotFound();
        }

        return View(persona);
    }

    // GET: /Persona/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Persona/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Cc,Nombre,Apellido,Genero,Edad")] Persona persona)
    {
        if (ModelState.IsValid && await _personas.GetByIdAsync(persona.Cc) is not null)
        {
            ModelState.AddModelError(nameof(Persona.Cc), "La cédula ya existe.");
        }

        if (!ModelState.IsValid)
        {
            return View(persona);
        }

        try
        {
            await _personas.AddAsync(persona);
        }
        catch (DbUpdateException)
        {
            // EF Core lanza DbUpdateException si la cédula quedó repetida u otra restricción falló
            ModelState.AddModelError(string.Empty, "No se pudo guardar la persona. Verifique que la cédula no esté repetida.");
            return View(persona);
        }

        TempData["Mensaje"] = "Persona creada.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Persona/Edit/{cc}
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var persona = await _personas.GetByIdAsync(id.Value);
        if (persona is null)
        {
            return NotFound();
        }

        return View(persona);
    }

    // POST: /Persona/Edit/{cc}
    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var persona = await _personas.GetByIdAsync(id.Value);
        if (persona is null)
        {
            return NotFound();
        }

        // Contra la sobrepublicación: solo se copian estos campos; la cédula es la llave y no se edita
        if (await TryUpdateModelAsync(persona, string.Empty,
                p => p.Nombre, p => p.Apellido, p => p.Genero, p => p.Edad))
        {
            try
            {
                await _personas.UpdateAsync(persona);
                TempData["Mensaje"] = "Persona actualizada.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "No se pudo actualizar la persona.");
            }
        }

        return View(persona);
    }

    // GET: /Persona/Delete/{cc}
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var persona = await _personas.GetByIdAsync(id.Value);
        if (persona is null)
        {
            return NotFound();
        }

        return View(persona);
    }

    // POST: /Persona/Delete/{cc}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (await _personas.GetByIdAsync(id) is null)
        {
            return NotFound();
        }

        try
        {
            await _personas.DeleteAsync(id);
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "No se pudo eliminar la persona. Intente de nuevo.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        TempData["Mensaje"] = "Persona eliminada junto con sus estudios y teléfonos.";
        return RedirectToAction(nameof(Index));
    }
}
