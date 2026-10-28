using System.ComponentModel.DataAnnotations;

namespace personapi_dotnet.Models.DTOs
{
    /// <summary>Datos de una profesión que entrega el servicio (respuesta).</summary>
    public class ProfesionDto
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string? Des { get; set; }
    }

    /// <summary>
    /// Datos para crear una profesión. Incluye el id porque en la tabla real
    /// <c>profesion.id</c> no es IDENTITY (contratos §5: si no es IDENTITY, el POST lo trae).
    /// </summary>
    public class ProfesionCrearDto
    {
        [Range(1, int.MaxValue)]
        public int Id { get; set; }
        [Required, StringLength(90)]
        public string Nom { get; set; } = string.Empty;
        [StringLength(4000)]
        public string? Des { get; set; }
    }

    /// <summary>Campos editables de una profesión (contratos §5.2); el id viaja en la ruta.</summary>
    public class ProfesionActualizarDto
    {
        [Required, StringLength(90)]
        public string Nom { get; set; } = string.Empty;
        [StringLength(4000)]
        public string? Des { get; set; }
    }
}
