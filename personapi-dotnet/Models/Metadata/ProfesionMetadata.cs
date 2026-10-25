using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Models.Metadata;

namespace personapi_dotnet.Models.Entities
{
    // Une la entidad generada con sus reglas sin editar Profesion.cs (Scaffold-DbContext lo sobrescribe)
    [ModelMetadataType(typeof(ProfesionMetadata))]
    public partial class Profesion
    {
    }
}

namespace personapi_dotnet.Models.Metadata
{
    /// <summary>
    /// Reglas de validación de Profesion (Tabla 19 del documento).
    /// </summary>
    public class ProfesionMetadata
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "El id es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El id debe ser un número positivo.")]
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(90, ErrorMessage = "El nombre admite máximo 90 caracteres.")]
        public string Nom { get; set; } = null!;

        [Display(Name = "Descripción")]
        [DataType(DataType.MultilineText)]
        public string? Des { get; set; }
    }
}
