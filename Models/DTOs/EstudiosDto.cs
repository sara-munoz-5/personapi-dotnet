using System.ComponentModel.DataAnnotations;

namespace personapi_dotnet.Models.DTOs
{
    /// <summary>Datos de un estudio que entrega el servicio (respuesta). Llave compuesta (IdProf, CcPer).</summary>
    public class EstudiosDto
    {
        public int IdProf { get; set; }
        public int CcPer { get; set; }
        public DateOnly? Fecha { get; set; }
        public string? Univer { get; set; }
    }

    /// <summary>Datos para crear un estudio; la persona y la profesión deben existir.</summary>
    public class EstudiosCrearDto
    {
        [Range(1, int.MaxValue)]
        public int IdProf { get; set; }
        [Range(1, int.MaxValue)]
        public int CcPer { get; set; }
        public DateOnly? Fecha { get; set; }
        [StringLength(50)]
        public string? Univer { get; set; }
    }

    /// <summary>Campos editables de un estudio (contratos §5.2); las dos partes de la llave viajan en la ruta.</summary>
    public class EstudiosActualizarDto
    {
        public DateOnly? Fecha { get; set; }
        [StringLength(50)]
        public string? Univer { get; set; }
    }
}
