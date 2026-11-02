using System.ComponentModel.DataAnnotations;

namespace personapi_dotnet.Models.DTOs
{
    /// <summary>Datos de un teléfono que entrega el servicio (respuesta).</summary>
    public class TelefonoDto
    {
        public string Num { get; set; } = string.Empty;
        public string Oper { get; set; } = string.Empty;
        public int Duenio { get; set; }
    }

    /// <summary>Datos para crear un teléfono; el dueño debe existir.</summary>
    public class TelefonoCrearDto
    {
        [Required(ErrorMessage = "El número es obligatorio."), StringLength(15, ErrorMessage = "El número admite máximo 15 caracteres.")]
        public string Num { get; set; } = string.Empty;
        [Required(ErrorMessage = "El operador es obligatorio."), StringLength(45, ErrorMessage = "El operador admite máximo 45 caracteres.")]
        public string Oper { get; set; } = string.Empty;
        [Range(1, int.MaxValue, ErrorMessage = "La cédula del dueño debe ser un número positivo.")]
        public int Duenio { get; set; }
    }

    /// <summary>Campos editables de un teléfono (contratos §5.2); el número viaja en la ruta.</summary>
    public class TelefonoActualizarDto
    {
        [Required(ErrorMessage = "El operador es obligatorio."), StringLength(45, ErrorMessage = "El operador admite máximo 45 caracteres.")]
        public string Oper { get; set; } = string.Empty;
        [Range(1, int.MaxValue, ErrorMessage = "La cédula del dueño debe ser un número positivo.")]
        public int Duenio { get; set; }
    }
}
