namespace personapi_dotnet.Models.ViewModels.Profesion
{
    /// <summary>Vista Profesiones/Listar: tabla de profesiones y mensaje del último cambio.</summary>
    public class ProfesionListarViewModel
    {
        public List<Fila> Profesiones { get; set; } = [];

        public string? Mensaje { get; set; }

        public class Fila
        {
            public int Id { get; set; }
            public string Nom { get; set; } = string.Empty;
            public string? Des { get; set; }
        }
    }
}
