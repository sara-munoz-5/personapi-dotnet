namespace personapi_dotnet.Models.ViewModels.Estudios
{
    /// <summary>Vista Estudios/Detalle (llave compuesta idProf + ccPer).</summary>
    public class EstudiosDetalleViewModel
    {
        public int IdProf { get; set; }
        public int CcPer { get; set; }
        public string Profesion { get; set; } = string.Empty;
        public string Persona { get; set; } = string.Empty;
        public DateOnly? Fecha { get; set; }
        public string? Univer { get; set; }
    }
}
