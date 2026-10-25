using System.Net;
using System.Text.Json;

namespace PersonApi.Tests.Api;

/// <summary>
/// Confirma que /swagger/v1/swagger.json es un documento OpenAPI 3.0 (no 3.1)
/// y que documenta exactamente los 20 endpoints de la Tabla 11.
/// </summary>
public class SwaggerTests
{
    private static readonly string[] EndpointsEsperados =
    [
        "GET /api/personas", "POST /api/personas",
        "GET /api/personas/{cc}", "PUT /api/personas/{cc}", "DELETE /api/personas/{cc}",
        "GET /api/profesiones", "POST /api/profesiones",
        "GET /api/profesiones/{id}", "PUT /api/profesiones/{id}", "DELETE /api/profesiones/{id}",
        "GET /api/estudios", "POST /api/estudios",
        "GET /api/estudios/{idProf}/{ccPer}", "PUT /api/estudios/{idProf}/{ccPer}", "DELETE /api/estudios/{idProf}/{ccPer}",
        "GET /api/telefonos", "POST /api/telefonos",
        "GET /api/telefonos/{num}", "PUT /api/telefonos/{num}", "DELETE /api/telefonos/{num}"
    ];

    private static readonly string[] MetodosHttp = ["get", "post", "put", "delete", "patch", "head", "options", "trace"];

    private static async Task<JsonElement> LeerDocumento()
    {
        await using var app = new ApiFactory();
        var respuesta = await app.CreateClient().GetAsync("/swagger/v1/swagger.json");
        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);

        using var documento = JsonDocument.Parse(await respuesta.Content.ReadAsStringAsync());
        return documento.RootElement.Clone();
    }

    [Fact]
    public async Task El_documento_es_OpenAPI_3_0()
    {
        var raiz = await LeerDocumento();

        Assert.StartsWith("3.0", raiz.GetProperty("openapi").GetString());
        Assert.Equal("personapi-dotnet API", raiz.GetProperty("info").GetProperty("title").GetString());
    }

    [Fact]
    public async Task El_documento_tiene_los_20_endpoints_de_la_tabla_11()
    {
        var raiz = await LeerDocumento();

        var endpoints = raiz.GetProperty("paths").EnumerateObject()
            .SelectMany(ruta => ruta.Value.EnumerateObject()
                .Where(operacion => MetodosHttp.Contains(operacion.Name))
                .Select(operacion => $"{operacion.Name.ToUpperInvariant()} {ruta.Name}"))
            .OrderBy(e => e)
            .ToList();

        Assert.Equal(20, endpoints.Count);
        Assert.Equal(EndpointsEsperados.Order(), endpoints);
    }

    [Fact]
    public async Task Cada_endpoint_documenta_sus_codigos_y_un_resumen_en_espanol()
    {
        var raiz = await LeerDocumento();

        foreach (var ruta in raiz.GetProperty("paths").EnumerateObject())
        {
            foreach (var operacion in ruta.Value.EnumerateObject().Where(o => MetodosHttp.Contains(o.Name)))
            {
                var nombre = $"{operacion.Name.ToUpperInvariant()} {ruta.Name}";
                var codigos = operacion.Value.GetProperty("responses").EnumerateObject().Select(c => c.Name).ToList();

                // El resumen sale del comentario XML <summary> del controlador
                Assert.True(operacion.Value.TryGetProperty("summary", out var resumen) && resumen.GetString()!.Length > 0,
                    $"{nombre} no tiene resumen.");

                var esperados = operacion.Name switch
                {
                    "get" when ruta.Name.Contains('{') => new[] { "200", "404" },
                    "get" => ["200"],
                    "post" => ["201", "400", "409"],
                    "put" => ["204", "400", "404"],
                    _ => ["204", "404"]
                };
                Assert.All(esperados, codigo => Assert.Contains(codigo, codigos));
            }
        }
    }
}
