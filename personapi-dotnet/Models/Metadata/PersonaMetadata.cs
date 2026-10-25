using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Models.Metadata;

namespace personapi_dotnet.Models.Entities
{
    // Une la entidad generada con sus reglas sin editar Persona.cs (Scaffold-DbContext lo sobrescribe)
    [ModelMetadataType(typeof(PersonaMetadata))]
    public partial class Persona
    {
    }
}

namespace personapi_dotnet.Models.Metadata
{
    /// <summary>
    /// Reglas de validación de Persona (Tabla 19 del documento).
    /// </summary>
    public class PersonaMetadata
    {
        [Display(Name = "Cédula")]
        [Required(ErrorMessage = "La cédula es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cédula debe ser un número positivo.")]
        public int Cc { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(45, ErrorMessage = "El nombre admite máximo 45 caracteres.")]
        public string Nombre { get; set; } = null!;

        [Display(Name = "Apellido")]
        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(45, ErrorMessage = "El apellido admite máximo 45 caracteres.")]
        public string Apellido { get; set; } = null!;

        [Display(Name = "Género")]
        [Required(ErrorMessage = "El género es obligatorio.")]
        [RegularExpression("^[MF]$", ErrorMessage = "El género debe ser Masculino (M) o Femenino (F).")]
        public string Genero { get; set; } = null!;

        [Display(Name = "Edad")]
        [Range(0, 120, ErrorMessage = "La edad debe estar entre 0 y 120.")]
        public int? Edad { get; set; }
    }
}
