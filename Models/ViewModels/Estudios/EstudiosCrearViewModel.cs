using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace personapi_dotnet.Models.ViewModels.Estudios
{
    /// <summary>Vista Estudios/Crear: persona y profesión se eligen de listas de registros existentes.</summary>
    public class EstudiosCrearViewModel
    {
        [Display(Name = "Profesión")]
        [Required(ErrorMessage = "Seleccione la profesión.")]
        public int? IdProf { get; set; }

        [Display(Name = "Persona")]
        [Required(ErrorMessage = "Seleccione la persona.")]
        public int? CcPer { get; set; }

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
