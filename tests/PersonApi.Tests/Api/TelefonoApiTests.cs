using System.Net;
using System.Net.Http.Json;
using personapi_dotnet.Models.Entities;

namespace PersonApi.Tests.Api;

/// <summary>Los 5 endpoints de /api/telefonos con sus códigos de la Tabla 12; la llave es texto.</summary>
public class TelefonoApiTests
{
    private static Telefono Nuevo(string num = "3201112233", int duenio = 52987654) =>
        new() { Num = num, Oper = "WOM", Duenio = duenio };

    [Fact]
    public async Task GetAll_devuelve_200_con_el_duenio_de_cada_telefono()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().GetAsync("/api/telefonos");

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var telefonos = await respuesta.Content.ReadFromJsonAsync<List<Telefono>>();
        Assert.Equal(3, telefonos!.Count);
        Assert.All(telefonos, t => Assert.NotNull(t.DuenioNavigation));
    }

    [Fact]
    public async Task GetById_devuelve_200()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().GetAsync("/api/telefonos/3109876543");

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var telefono = await respuesta.Content.ReadFromJsonAsync<Telefono>();
        Assert.Equal("Movistar", telefono!.Oper);
        Assert.Equal(79123456, telefono.Duenio);
    }

    [Fact]
    public async Task GetById_devuelve_404_si_no_existe()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().GetAsync("/api/telefonos/0000000");

        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
    }

    [Fact]
    public async Task Create_devuelve_201_con_la_ubicacion_del_nuevo_recurso()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().PostAsJsonAsync("/api/telefonos", Nuevo());

        Assert.Equal(HttpStatusCode.Created, respuesta.StatusCode);
        Assert.Equal("/api/telefonos/3201112233", respuesta.Headers.Location?.AbsolutePath);
        Assert.Single(app.Datos.Personas.Single(p => p.Cc == 52987654).Telefonos);
    }

    [Fact]
    public async Task Create_devuelve_409_si_el_numero_ya_existe()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().PostAsJsonAsync("/api/telefonos", Nuevo("3001234567"));

        Assert.Equal(HttpStatusCode.Conflict, respuesta.StatusCode);
        Assert.Equal("Claro", app.Datos.Telefonos.Single(t => t.Num == "3001234567").Oper);
    }

    [Fact]
    public async Task Create_devuelve_409_si_el_duenio_no_existe()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().PostAsJsonAsync("/api/telefonos", Nuevo(duenio: 1));

        Assert.Equal(HttpStatusCode.Conflict, respuesta.StatusCode);
        Assert.DoesNotContain(app.Datos.Telefonos, t => t.Num == "3201112233");
    }

    [Fact]
    public async Task Update_devuelve_204_y_cambia_de_duenio()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().PutAsJsonAsync("/api/telefonos/3155551234", Nuevo("3155551234", 52987654));

        Assert.Equal(HttpStatusCode.NoContent, respuesta.StatusCode);
        var telefono = app.Datos.Telefonos.Single(t => t.Num == "3155551234");
        Assert.Equal("WOM", telefono.Oper);
        Assert.Equal(52987654, telefono.Duenio);
        Assert.Single(app.Datos.Personas.Single(p => p.Cc == 1012345678).Telefonos);
    }

    [Fact]
    public async Task Update_devuelve_400_si_el_numero_de_la_url_no_coincide()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().PutAsJsonAsync("/api/telefonos/3155551234", Nuevo("3001234567"));

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
    }

    [Fact]
    public async Task Update_devuelve_404_si_no_existe()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().PutAsJsonAsync("/api/telefonos/3201112233", Nuevo());

        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
    }

    [Fact]
    public async Task Update_devuelve_409_si_el_nuevo_duenio_no_existe()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().PutAsJsonAsync("/api/telefonos/3155551234", Nuevo("3155551234", 1));

        Assert.Equal(HttpStatusCode.Conflict, respuesta.StatusCode);
        Assert.Equal(1012345678, app.Datos.Telefonos.Single(t => t.Num == "3155551234").Duenio);
    }

    [Fact]
    public async Task Delete_devuelve_204()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().DeleteAsync("/api/telefonos/3001234567");

        Assert.Equal(HttpStatusCode.NoContent, respuesta.StatusCode);
        Assert.DoesNotContain(app.Datos.Telefonos, t => t.Num == "3001234567");
    }

    [Fact]
    public async Task Delete_devuelve_404_si_no_existe()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().DeleteAsync("/api/telefonos/0000000");

        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
    }
}
