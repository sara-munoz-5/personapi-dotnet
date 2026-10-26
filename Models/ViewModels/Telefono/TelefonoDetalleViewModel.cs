namespace personapi_dotnet.Models.ViewModels.Telefono
{
    /// <summary>Vista Telefonos/Detalle.</summary>
    public class TelefonoDetalleViewModel
    {
        public string Num { get; set; } = string.Empty;
        public string Oper { get; set; } = string.Empty;
        public int Duenio { get; set; }
        public string NombreDuenio { get; set; } = string.Empty;
    }
}
