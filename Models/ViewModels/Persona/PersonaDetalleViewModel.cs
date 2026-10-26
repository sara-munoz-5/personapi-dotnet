namespace personapi_dotnet.Models.ViewModels.Persona
{
    /// <summary>Vista Personas/Detalle: datos de la persona con sus estudios y teléfonos.</summary>
    public class PersonaDetalleViewModel
    {
        public int Cc { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public int Edad { get; set; }

        public List<EstudioFila> Estudios { get; set; } = [];
        public List<TelefonoFila> Telefonos { get; set; } = [];

        public class EstudioFila
        {
            public int IdProf { get; set; }
            public string Profesion { get; set; } = string.Empty;
            public string? Univer { get; set; }
            public DateOnly? Fecha { get; set; }
        }

        public class TelefonoFila
        {
            public string Num { get; set; } = string.Empty;
            public string Oper { get; set; } = string.Empty;
        }
    }
}
