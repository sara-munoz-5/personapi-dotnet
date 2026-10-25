using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Models.Metadata;

namespace personapi_dotnet.Models.Entities
{
    // Une la entidad generada con sus reglas sin editar Telefono.cs (Scaffold-DbContext lo sobrescribe)
    [ModelMetadataType(typeof(TelefonoMetadata))]
    public partial class Telefono
    {
    }
}

namespace personapi_dotnet.Models.Metadata
{
    /// <summary>
    /// Reglas de validación de Telefono (Tabla 19 del documento).
    /// </summary>
    public class TelefonoMetadata
    {
        [Display(Name = "Número")]
        [Required(ErrorMessage = "El número es obligatorio.")]
        [StringLength(15, ErrorMessage = "El número admite máximo 15 caracteres.")]
        public string Num { get; set; } = null!;

        [Display(Name = "Operador")]
        [Required(ErrorMessage = "El operador es obligatorio.")]
        [StringLength(45, ErrorMessage = "El operador admite máximo 45 caracteres.")]
        public string Oper { get; set; } = null!;

        [Display(Name = "Dueño")]
        [Required(ErrorMessage = "Seleccione el dueño.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione el dueño.")]
        public int Duenio { get; set; }
    }
}
