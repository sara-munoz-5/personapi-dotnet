using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace personapi_dotnet.Models.ViewModels.Estudios
{
    /// <summary>
    /// Vista Estudios/Editar: las dos partes de la llave son de solo lectura y salen de la URL.
    /// Las listas solo muestran la persona y la profesión seleccionadas (deshabilitadas).
    /// </summary>
    public class EstudiosEditarViewModel
    {
        [Display(Name = "Profesión")]
        [BindNever]
        public int IdProf { get; set; }

        [Display(Name = "Persona")]
        [BindNever]
        public int CcPer { get; set; }

        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        public DateOnly? Fecha { get; set; }

        [Display(Name = "Universidad")]
        [StringLength(50, ErrorMessage = "La universidad admite máximo 50 caracteres.")]
        public string? Univer { get; set; }

        [ValidateNever]
        public List<SelectListItem> Profesiones { get; set; } = [];

        [ValidateNever]
        public List<SelectListItem> Personas { get; set; } = [];
    }
}
