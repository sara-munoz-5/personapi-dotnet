namespace personapi_dotnet.Models.ViewModels.Telefono
{
    /// <summary>Vista Telefonos/Listar: muestra el nombre del dueño en lugar de su cédula.</summary>
    public class TelefonoListarViewModel
    {
        public List<Fila> Telefonos { get; set; } = [];

        public string? Mensaje { get; set; }

        public class Fila
        {
            public string Num { get; set; } = string.Empty;
            public string Oper { get; set; } = string.Empty;
            public int Duenio { get; set; }
            public string NombreDuenio { get; set; } = string.Empty;
        }
    }
}
