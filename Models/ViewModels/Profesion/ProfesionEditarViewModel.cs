using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace personapi_dotnet.Models.ViewModels.Profesion
{
    /// <summary>Vista Profesiones/Editar: el id es de solo lectura y sale de la URL.</summary>
    public class ProfesionEditarViewModel
    {
        [Display(Name = "Id")]
        [BindNever]
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(90, ErrorMessage = "El nombre admite máximo 90 caracteres.")]
        public string Nom { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        public string? Des { get; set; }
    }
}
