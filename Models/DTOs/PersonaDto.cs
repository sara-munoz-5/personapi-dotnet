// PROPUESTA del integrante 3 según contratos §4B; pendiente de aprobación del integrante 2
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
        public int Cc { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public int Edad { get; set; }
    }

    /// <summary>Campos editables de una persona (contratos §5.2); la cédula viaja en la ruta.</summary>
    public class PersonaActualizarDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public int Edad { get; set; }
    }
}
