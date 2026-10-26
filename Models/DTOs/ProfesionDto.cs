// PROPUESTA del integrante 3 según contratos §4B; pendiente de aprobación del integrante 2
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
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string? Des { get; set; }
    }

    /// <summary>Campos editables de una profesión (contratos §5.2); el id viaja en la ruta.</summary>
    public class ProfesionActualizarDto
    {
        public string Nom { get; set; } = string.Empty;
        public string? Des { get; set; }
    }
}
