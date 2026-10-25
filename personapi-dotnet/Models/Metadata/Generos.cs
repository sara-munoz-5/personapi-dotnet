using Microsoft.AspNetCore.Mvc.Rendering;

namespace personapi_dotnet.Models.Metadata;

/// <summary>
/// Opciones de la columna persona.genero (CHAR(1) con CHECK M o F) y su texto en pantalla.
/// </summary>
public static class Generos
{
    private static readonly Dictionary<string, string> Textos = new()
    {
        ["M"] = "Masculino",
        ["F"] = "Femenino"
    };

    /// <summary>Devuelve "Masculino" o "Femenino" a partir de la letra guardada en la base.</summary>
    public static string Texto(string? codigo)
    {
        var letra = codigo?.Trim() ?? string.Empty;
        return Textos.TryGetValue(letra, out var texto) ? texto : letra;
    }

    /// <summary>Lista desplegable de dos opciones con la actual marcada.</summary>
    public static List<SelectListItem> Lista(string? seleccionado = null) =>
        Textos.Select(t => new SelectListItem(t.Value, t.Key, t.Key == seleccionado?.Trim())).ToList();
}
