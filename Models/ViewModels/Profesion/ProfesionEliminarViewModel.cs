namespace personapi_dotnet.Models.ViewModels.Profesion
{
    /// <summary>Vista Profesiones/Eliminar: datos que se muestran antes de confirmar el borrado.</summary>
    public class ProfesionEliminarViewModel
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string? Des { get; set; }
    }
}
