using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Models.Metadata;

namespace personapi_dotnet.Models.Entities
{
    // Une la entidad generada con sus reglas sin editar Estudio.cs (Scaffold-DbContext lo sobrescribe)
    [ModelMetadataType(typeof(EstudioMetadata))]
    public partial class Estudio
    {
    }
}

namespace personapi_dotnet.Models.Metadata
{
    /// <summary>
    /// Reglas de validación de Estudio (Tabla 19 del documento).
    /// </summary>
    public class EstudioMetadata
    {
        [Display(Name = "Profesión")]
        [Required(ErrorMessage = "Seleccione una profesión.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una profesión.")]
        public int IdProf { get; set; }

        [Display(Name = "Persona")]
        [Required(ErrorMessage = "Seleccione una persona.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una persona.")]
        public int CcPer { get; set; }

        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        [NoFutura]
        public DateOnly? Fecha { get; set; }

        [Display(Name = "Universidad")]
        [StringLength(50, ErrorMessage = "La universidad admite máximo 50 caracteres.")]
        public string? Univer { get; set; }
    }
}
