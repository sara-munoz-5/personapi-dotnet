// PROPUESTA del integrante 3 según contratos §4B; pendiente de aprobación del integrante 2
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
        public string Num { get; set; } = string.Empty;
        public string Oper { get; set; } = string.Empty;
        public int Duenio { get; set; }
    }

    /// <summary>Campos editables de un teléfono (contratos §5.2); el número viaja en la ruta.</summary>
    public class TelefonoActualizarDto
    {
        public string Oper { get; set; } = string.Empty;
        public int Duenio { get; set; }
    }
}
