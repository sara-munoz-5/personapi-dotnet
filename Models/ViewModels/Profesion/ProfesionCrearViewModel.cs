using System.ComponentModel.DataAnnotations;

namespace personapi_dotnet.Models.ViewModels.Profesion
{
    /// <summary>Vista Profesiones/Crear. El id se escribe a mano porque la tabla no usa IDENTITY.</summary>
    public class ProfesionCrearViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "El id es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El id debe ser un número positivo.")]
        public int? Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(90, ErrorMessage = "El nombre admite máximo 90 caracteres.")]
        public string Nom { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        public string? Des { get; set; }
    }
}
