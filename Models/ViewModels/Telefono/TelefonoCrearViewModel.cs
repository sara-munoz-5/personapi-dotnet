using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace personapi_dotnet.Models.ViewModels.Telefono
{
    /// <summary>Vista Telefonos/Crear: el dueño se elige de la lista de personas existentes.</summary>
    public class TelefonoCrearViewModel
    {
        [Display(Name = "Número")]
        [Required(ErrorMessage = "El número es obligatorio.")]
        [StringLength(15, ErrorMessage = "El número admite máximo 15 caracteres.")]
        public string Num { get; set; } = string.Empty;

        [Display(Name = "Operador")]
        [Required(ErrorMessage = "El operador es obligatorio.")]
        [StringLength(45, ErrorMessage = "El operador admite máximo 45 caracteres.")]
        public string Oper { get; set; } = string.Empty;

        [Display(Name = "Dueño")]
        [Required(ErrorMessage = "Seleccione el dueño.")]
        public int? Duenio { get; set; }

        [ValidateNever]
        public List<SelectListItem> Personas { get; set; } = [];
    }
}
