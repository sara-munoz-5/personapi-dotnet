using System.ComponentModel.DataAnnotations;

namespace personapi_dotnet.Models.DTOs
{
    /// <summary>Datos de una persona que entrega el servicio (respuesta).</summary>
    public class PersonaDto
    {
        public int Cc { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public int Edad { get; set; }
    }

    /// <summary>Datos para crear una persona; la cédula se ingresa manualmente.</summary>
    public class PersonaCrearDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "La cédula debe ser un número positivo.")]
        public int Cc { get; set; }
        [Required, StringLength(45)]
        public string Nombre { get; set; } = string.Empty;
        [Required, StringLength(45)]
        public string Apellido { get; set; } = string.Empty;
        [Required, RegularExpression("^[MF]$", ErrorMessage = "El género debe ser M o F.")]
        public string Genero { get; set; } = string.Empty;
        [Range(0, 130)]
        public int Edad { get; set; }
    }

    /// <summary>Campos editables de una persona (contratos §5.2); la cédula viaja en la ruta.</summary>
    public class PersonaActualizarDto
    {
        [Required, StringLength(45)]
        public string Nombre { get; set; } = string.Empty;
        [Required, StringLength(45)]
        public string Apellido { get; set; } = string.Empty;
        [Required, RegularExpression("^[MF]$", ErrorMessage = "El género debe ser M o F.")]
        public string Genero { get; set; } = string.Empty;
        [Range(0, 130)]
        public int Edad { get; set; }
    }
}
