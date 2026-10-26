using Microsoft.AspNetCore.Mvc.Rendering;

namespace personapi_dotnet.Models.ViewModels.Persona
{
    /// <summary>Opciones de género (CHECK M/F de la tabla persona) y su texto para mostrar.</summary>
    public static class Generos
    {
        public static List<SelectListItem> Opciones() =>
        [
            new SelectListItem("Masculino", "M"),
            new SelectListItem("Femenino", "F")
        ];

        public static string Texto(string genero) => genero switch
        {
            "M" => "Masculino",
            "F" => "Femenino",
            _ => genero
        };
    }
}
