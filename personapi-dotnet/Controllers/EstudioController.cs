using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Interfaces;
using personapi_dotnet.Models.Entities;
using personapi_dotnet.ViewModels;

namespace personapi_dotnet.Controllers;

/// <summary>
/// Interfaz MVC del CRUD de estudios. La llave es compuesta, así que Details, Edit y Delete
/// reciben idProf y ccPer: /Estudio/Edit?idProf=3&amp;ccPer=1012345678.
/// </summary>
public class EstudioController : Controller
{
    private const string MensajeParejaRepetida = "Esta persona ya tiene registrada esa profesión.";

    private readonly IEstudioRepository _estudios;
    private readonly IPersonaRepository _personas;
    private readonly IProfesionRepository _profesiones;

    public EstudioController(IEstudioRepository estudios, IPersonaRepository personas, IProfesionRepository profesiones)
    {
        _estudios = estudios;
        _personas = personas;
        _profesiones = profesiones;
    }

    // GET: /Estudio
    public async Task<IActionResult> Index()
    {
        return View(await _estudios.GetAllAsync());
    }

    // GET: /Estudio/Details?idProf=&ccPer=
    public async Task<IActionResult> Details(int? idProf, int? ccPer)
    {
        var estudio = await BuscarAsync(idProf, ccPer);
        if (estudio is null)
        {
            return NotFound();
        }

        return View(estudio);
    }

    // GET: /Estudio/Create
    public async Task<IActionResult> Create()
    {
        return View(await ArmarFormularioAsync(new Estudio()));
    }

    // POST: /Estudio/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("IdProf,CcPer,Fecha,Univer", Prefix = nameof(EstudioFormViewModel.Estudio))] Estudio estudio)
    {
        if (ModelState.IsValid && await _estudios.GetByIdAsync(estudio.IdProf, estudio.CcPer) is not null)
        {
            ModelState.AddModelError(string.Empty, MensajeParejaRepetida);
        }

        if (!ModelState.IsValid)
        {
            // Las listas no viajan en el formulario: se recargan antes de devolver la vista
            return View(await ArmarFormularioAsync(estudio));
        }

        try
        {
            await _estudios.AddAsync(estudio);
        }
        catch (DbUpdateException)
        {
            // EF Core lanza DbUpdateException si la pareja quedó repetida o la persona/profesión no existe
            ModelState.AddModelError(string.Empty, "No se pudo guardar el estudio. Verifique que la persona no tenga ya esa profesión y que las dos existan.");
            return View(await ArmarFormularioAsync(estudio));
        }

        TempData["Mensaje"] = "Estudio creado.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Estudio/Edit?idProf=&ccPer=
    public async Task<IActionResult> Edit(int? idProf, int? ccPer)
    {
        var estudio = await BuscarAsync(idProf, ccPer);
        if (estudio is null)
        {
            return NotFound();
        }

        // Persona y profesión son la llave: en Edit se muestran de solo lectura, sin listas
        return View(new EstudioFormViewModel { Estudio = estudio });
    }

    // POST: /Estudio/Edit?idProf=&ccPer=
    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost(int? idProf, int? ccPer)
    {
        var estudio = await BuscarAsync(idProf, ccPer);
        if (estudio is null)
        {
            return NotFound();
        }

        // Contra la sobrepublicación: solo se editan la fecha y la universidad
        if (await TryUpdateModelAsync(estudio, nameof(EstudioFormViewModel.Estudio), e => e.Fecha, e => e.Univer))
        {
            try
            {
                await _estudios.UpdateAsync(estudio);
                TempData["Mensaje"] = "Estudio actualizado.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "No se pudo actualizar el estudio.");
            }
        }

        return View(new EstudioFormViewModel { Estudio = estudio });
    }

    // GET: /Estudio/Delete?idProf=&ccPer=
    public async Task<IActionResult> Delete(int? idProf, int? ccPer)
    {
        var estudio = await BuscarAsync(idProf, ccPer);
        if (estudio is null)
        {
            return NotFound();
        }

        return View(estudio);
    }

    // POST: /Estudio/Delete?idProf=&ccPer=
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int idProf, int ccPer)
    {
        if (await _estudios.GetByIdAsync(idProf, ccPer) is null)
        {
            return NotFound();
        }

        try
        {
            await _estudios.DeleteAsync(idProf, ccPer);
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "No se pudo eliminar el estudio. Intente de nuevo.";
            return RedirectToAction(nameof(Delete), new { idProf, ccPer });
        }

        TempData["Mensaje"] = "Estudio eliminado.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<Estudio?> BuscarAsync(int? idProf, int? ccPer)
    {
        if (idProf is null || ccPer is null)
        {
            return null;
        }

        return await _estudios.GetByIdAsync(idProf.Value, ccPer.Value);
    }

    private async Task<EstudioFormViewModel> ArmarFormularioAsync(Estudio estudio)
    {
        var personas = await _personas.GetAllAsync();
        var profesiones = await _profesiones.GetAllAsync();

        return new EstudioFormViewModel
        {
            Estudio = estudio,
            Personas = personas
                .OrderBy(p => p.Nombre).ThenBy(p => p.Apellido)
                .Select(p => new SelectListItem($"{p.Nombre} {p.Apellido}", p.Cc.ToString())),
            Profesiones = profesiones
                .OrderBy(p => p.Nom)
                .Select(p => new SelectListItem(p.Nom, p.Id.ToString()))
        };
    }
}
