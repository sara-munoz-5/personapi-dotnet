namespace personapi_dotnet.Models.ViewModels.Estudios
{
    /// <summary>Vista Estudios/Listar: muestra nombres de persona y profesión en lugar de las llaves.</summary>
    public class EstudiosListarViewModel
    {
        public List<Fila> Estudios { get; set; } = [];

        public string? Mensaje { get; set; }

        public class Fila
        {
            public int IdProf { get; set; }
            public int CcPer { get; set; }
            public string Profesion { get; set; } = string.Empty;
            public string Persona { get; set; } = string.Empty;
            public DateOnly? Fecha { get; set; }
            public string? Univer { get; set; }
        }
    }
}
