namespace personapi_dotnet.Models.ViewModels.Persona
{
    /// <summary>Vista Personas/Listar: tabla de personas y mensaje del último cambio.</summary>
    public class PersonaListarViewModel
    {
        public List<Fila> Personas { get; set; } = [];

        public string? Mensaje { get; set; }

        public class Fila
        {
            public int Cc { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public string Apellido { get; set; } = string.Empty;
            public string Genero { get; set; } = string.Empty;
            public int Edad { get; set; }
        }
    }
}
