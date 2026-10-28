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
        [Required, StringLength(15)]
        public string Num { get; set; } = string.Empty;
        [Required, StringLength(45)]
        public string Oper { get; set; } = string.Empty;
        [Range(1, int.MaxValue)]
        public int Duenio { get; set; }
    }

    /// <summary>Campos editables de un teléfono (contratos §5.2); el número viaja en la ruta.</summary>
    public class TelefonoActualizarDto
    {
        [Required, StringLength(45)]
        public string Oper { get; set; } = string.Empty;
        [Range(1, int.MaxValue)]
        public int Duenio { get; set; }
    }
}
