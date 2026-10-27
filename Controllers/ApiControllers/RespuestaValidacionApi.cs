using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace personapi_dotnet.Controllers.ApiControllers;

/// <summary>
/// Respuesta 400 de la API cuando el modelo no es válido: ValidationProblemDetails con textos en español.
/// Reemplaza los mensajes en inglés del lector JSON y quita el error duplicado del parámetro del cuerpo
/// ("The dto field is required.") que ASP.NET Core agrega cuando el JSON no se pudo leer.
/// </summary>
public static class RespuestaValidacionApi
{
    public static IActionResult Crear(ActionContext contexto)
    {
        var parametrosCuerpo = contexto.ActionDescriptor.Parameters
            .Where(p => p.BindingInfo?.BindingSource == BindingSource.Body)
            .Select(p => p.Name)
            .ToHashSet();

        var errores = new Dictionary<string, string[]>();
        foreach (var (clave, entrada) in contexto.ModelState)
        {
            if (entrada.Errors.Count == 0 || parametrosCuerpo.Contains(clave))
                continue;

            if (clave.StartsWith('$'))
            {
                // Error del lector JSON: la clave es la ruta JSON ($.cc) y el mensaje es técnico y en inglés.
                var campo = clave.TrimStart('$', '.');
                errores[campo.Length == 0 ? "cuerpo" : campo] = [campo.Length == 0
                    ? "El JSON no es válido."
                    : $"El JSON no es válido o el valor de '{campo}' no tiene el tipo esperado."];
                continue;
            }

            errores[clave.Length == 0 ? "cuerpo" : clave] = entrada.Errors
                .Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? "El valor no es válido." : e.ErrorMessage)
                .ToArray();
        }

        if (errores.Count == 0)
            errores["cuerpo"] = ["Se requiere un cuerpo JSON válido."];

        var problema = new ValidationProblemDetails(errores)
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Title = "Uno o más datos no son válidos.",
            Status = StatusCodes.Status400BadRequest
        };
        problema.Extensions["traceId"] = Activity.Current?.Id ?? contexto.HttpContext.TraceIdentifier;

        return new BadRequestObjectResult(problema) { ContentTypes = { "application/problem+json" } };
    }
}
