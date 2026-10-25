using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.ViewModels;

/// <summary>
/// Modelo de los formularios Create y Edit de Telefono: el teléfono y la lista desplegable
/// de posibles dueños (valor cc, texto nombre y apellido).
/// </summary>
public class TelefonoFormViewModel
{
    public Telefono Telefono { get; set; } = new();

    [ValidateNever]
    public IEnumerable<SelectListItem> Personas { get; set; } = [];
}
