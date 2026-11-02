using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace personapi_dotnet.Models.ViewModels.Persona
{
    /// <summary>Vista Personas/Editar: la cédula es de solo lectura y sale de la URL.</summary>
    public class PersonaEditarViewModel
    {
        [Display(Name = "Cédula")]
        [BindNever]
        public int Cc { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(45, ErrorMessage = "El nombre admite máximo 45 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Apellido")]
        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(45, ErrorMessage = "El apellido admite máximo 45 caracteres.")]
        public string Apellido { get; set; } = string.Empty;

        [Display(Name = "Género")]
        [Required(ErrorMessage = "Seleccione el género.")]
        [RegularExpression("^[MF]$", ErrorMessage = "El género debe ser Masculino o Femenino.")]
        public string Genero { get; set; } = string.Empty;

        [Display(Name = "Edad")]
        [Required(ErrorMessage = "La edad es obligatoria.")]
        [Range(0, 130, ErrorMessage = "La edad debe estar entre 0 y 130.")]
        public int? Edad { get; set; }

        [ValidateNever]
        public List<SelectListItem> OpcionesGenero { get; set; } = Generos.Opciones();
    }
}
