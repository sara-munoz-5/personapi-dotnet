using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.ViewModels;

/// <summary>
/// Modelo de los formularios Create y Edit de Estudio: el estudio y las listas desplegables
/// de personas (valor cc, texto nombre y apellido) y profesiones (valor id, texto nom).
/// </summary>
public class EstudioFormViewModel
{
    public Estudio Estudio { get; set; } = new();

    [ValidateNever]
    public IEnumerable<SelectListItem> Personas { get; set; } = [];

    [ValidateNever]
    public IEnumerable<SelectListItem> Profesiones { get; set; } = [];
}
