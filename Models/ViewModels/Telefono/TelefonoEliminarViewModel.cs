namespace personapi_dotnet.Models.ViewModels.Telefono
{
    /// <summary>Vista Telefonos/Eliminar: datos que se muestran antes de confirmar el borrado.</summary>
    public class TelefonoEliminarViewModel
    {
        public string Num { get; set; } = string.Empty;
        public string Oper { get; set; } = string.Empty;
        public string NombreDuenio { get; set; } = string.Empty;
    }
}
