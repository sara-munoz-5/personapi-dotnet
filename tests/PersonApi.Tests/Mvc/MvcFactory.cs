using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PersonApi.Tests.Fakes;
using personapi_dotnet.Interfaces;

namespace PersonApi.Tests.Mvc;

/// <summary>
/// Levanta la aplicación en memoria con los repositorios falsos para recorrer las vistas por HTTP,
/// como lo haría un navegador sin JavaScript. Cada prueba crea su propia fábrica y sus propios datos.
/// </summary>
public class MvcFactory : WebApplicationFactory<Program>
{
    /// <summary>Datos en memoria que ven los repositorios de esta instancia.</summary>
    public FakeDatos Datos { get; } = FakeDatos.ConEjemplos();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IPersonaRepository>();
            services.RemoveAll<IProfesionRepository>();
            services.RemoveAll<IEstudioRepository>();
            services.RemoveAll<ITelefonoRepository>();

            services.AddSingleton(Datos);
            services.AddScoped<IPersonaRepository, FakePersonaRepository>();
            services.AddScoped<IProfesionRepository, FakeProfesionRepository>();
            services.AddScoped<IEstudioRepository, FakeEstudioRepository>();
            services.AddScoped<ITelefonoRepository, FakeTelefonoRepository>();
        });
    }

    /// <summary>Cliente con cookies (antifalsificación y TempData) que no sigue redirecciones solo.</summary>
    public Navegador Navegador() =>
        new(CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }));
}

/// <summary>Página recibida: código HTTP, HTML decodificado (Razor escribe los acentos como entidades) y redirección.</summary>
public record Pagina(HttpStatusCode Codigo, string Html, string? Redireccion);

/// <summary>Cliente HTTP que se comporta como un navegador sin JavaScript.</summary>
public partial class Navegador(HttpClient cliente)
{
    public async Task<Pagina> Abrir(string url)
    {
        var respuesta = await cliente.GetAsync(url);
        return await Leer(respuesta);
    }

    /// <summary>
    /// Abre el formulario en <paramref name="urlFormulario"/>, toma su token antifalsificación
    /// y envía los campos con POST a la misma dirección (o a <paramref name="urlDestino"/>).
    /// </summary>
    public async Task<Pagina> Enviar(string urlFormulario, Dictionary<string, string> campos, string? urlDestino = null)
    {
        var formulario = await Abrir(urlFormulario);
        Assert.Equal(HttpStatusCode.OK, formulario.Codigo);

        var token = TokenRegex().Match(formulario.Html);
        Assert.True(token.Success, $"El formulario {urlFormulario} no trae el token antifalsificación.");

        var datos = new Dictionary<string, string>(campos) { ["__RequestVerificationToken"] = token.Groups[1].Value };
        return await EnviarSinToken(urlDestino ?? urlFormulario, datos);
    }

    public async Task<Pagina> EnviarSinToken(string url, Dictionary<string, string> campos)
    {
        var respuesta = await cliente.PostAsync(url, new FormUrlEncodedContent(campos));
        return await Leer(respuesta);
    }

    /// <summary>Sigue una redirección como lo hace el navegador (GET a la nueva dirección).</summary>
    public Task<Pagina> Seguir(Pagina pagina)
    {
        Assert.Equal(HttpStatusCode.Redirect, pagina.Codigo);
        return Abrir(pagina.Redireccion!);
    }

    private static async Task<Pagina> Leer(HttpResponseMessage respuesta)
    {
        var html = WebUtility.HtmlDecode(await respuesta.Content.ReadAsStringAsync());
        return new Pagina(respuesta.StatusCode, html, respuesta.Headers.Location?.OriginalString);
    }

    [GeneratedRegex("name=\"__RequestVerificationToken\" type=\"hidden\" value=\"([^\"]+)\"")]
    private static partial Regex TokenRegex();
}
