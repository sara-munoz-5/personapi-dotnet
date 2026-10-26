namespace personapi_dotnet.Models.ViewModels.Estudios
{
    /// <summary>Vista Estudios/Eliminar: datos que se muestran antes de confirmar el borrado.</summary>
    public class EstudiosEliminarViewModel
    {
        public int IdProf { get; set; }
        public int CcPer { get; set; }
        public string Profesion { get; set; } = string.Empty;
        public string Persona { get; set; } = string.Empty;
        public DateOnly? Fecha { get; set; }
        public string? Univer { get; set; }
    }
}
