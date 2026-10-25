using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PersonApi.Tests.Fakes;
using personapi_dotnet.Interfaces;

namespace PersonApi.Tests.Api;

/// <summary>
/// Levanta la aplicación completa en memoria y cambia los repositorios reales por los falsos,
/// así las pruebas recorren rutas, códigos HTTP y JSON sin necesitar SQL Server.
/// Cada prueba crea su propia fábrica, con sus propios datos de ejemplo.
/// </summary>
public class ApiFactory : WebApplicationFactory<Program>
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
}
