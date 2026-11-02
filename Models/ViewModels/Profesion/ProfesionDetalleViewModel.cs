namespace personapi_dotnet.Models.ViewModels.Profesion
{
    /// <summary>Vista Profesiones/Detalle.</summary>
    public class ProfesionDetalleViewModel
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string? Des { get; set; }
    }
}
