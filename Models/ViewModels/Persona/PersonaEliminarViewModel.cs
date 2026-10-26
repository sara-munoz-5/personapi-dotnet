namespace personapi_dotnet.Models.ViewModels.Persona
{
    /// <summary>Vista Personas/Eliminar: datos que se muestran antes de confirmar el borrado.</summary>
    public class PersonaEliminarViewModel
    {
        public int Cc { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public int Edad { get; set; }
    }
}
