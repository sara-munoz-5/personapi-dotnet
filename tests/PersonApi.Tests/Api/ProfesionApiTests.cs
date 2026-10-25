using System.Net;
using System.Net.Http.Json;
using personapi_dotnet.Models.Entities;

namespace PersonApi.Tests.Api;

/// <summary>Los 5 endpoints de /api/profesiones con sus códigos de la Tabla 12.</summary>
public class ProfesionApiTests
{
    private static Profesion Nueva(int id = 4) =>
        new() { Id = id, Nom = "Arquitectura", Des = "Diseño de edificaciones" };

    [Fact]
    public async Task GetAll_devuelve_200_con_todas_las_profesiones()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().GetAsync("/api/profesiones");

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var profesiones = await respuesta.Content.ReadFromJsonAsync<List<Profesion>>();
        Assert.Equal(3, profesiones!.Count);
    }

    [Fact]
    public async Task GetById_devuelve_200_con_sus_estudios()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().GetAsync("/api/profesiones/1");

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var profesion = await respuesta.Content.ReadFromJsonAsync<Profesion>();
        Assert.Equal("Ingeniería de Sistemas", profesion!.Nom);
        Assert.Single(profesion.Estudios);
    }

    [Fact]
    public async Task GetById_devuelve_404_si_no_existe()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().GetAsync("/api/profesiones/99");

        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
    }

    [Fact]
    public async Task Create_devuelve_201_con_la_ubicacion_del_nuevo_recurso()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().PostAsJsonAsync("/api/profesiones", Nueva());

        Assert.Equal(HttpStatusCode.Created, respuesta.StatusCode);
        Assert.Equal("/api/profesiones/4", respuesta.Headers.Location?.AbsolutePath);
        Assert.Contains(app.Datos.Profesiones, p => p.Id == 4);
    }

    [Fact]
    public async Task Create_devuelve_409_si_el_id_ya_existe()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().PostAsJsonAsync("/api/profesiones", Nueva(1));

        Assert.Equal(HttpStatusCode.Conflict, respuesta.StatusCode);
        Assert.Equal("Ingeniería de Sistemas", app.Datos.Profesiones.Single(p => p.Id == 1).Nom);
    }

    [Fact]
    public async Task Create_devuelve_409_si_la_base_rechaza_el_guardado()
    {
        await using var app = new ApiFactory();
        app.Datos.FallarAlGuardar = true;

        var respuesta = await app.CreateClient().PostAsJsonAsync("/api/profesiones", Nueva());

        Assert.Equal(HttpStatusCode.Conflict, respuesta.StatusCode);
    }

    [Fact]
    public async Task Update_devuelve_204_y_guarda_los_cambios()
    {
        await using var app = new ApiFactory();
        var cambios = new Profesion { Id = 3, Nom = "Derecho", Des = "Ciencias jurídicas" };

        var respuesta = await app.CreateClient().PutAsJsonAsync("/api/profesiones/3", cambios);

        Assert.Equal(HttpStatusCode.NoContent, respuesta.StatusCode);
        Assert.Equal("Ciencias jurídicas", app.Datos.Profesiones.Single(p => p.Id == 3).Des);
    }

    [Fact]
    public async Task Update_devuelve_400_si_el_id_de_la_url_no_coincide()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().PutAsJsonAsync("/api/profesiones/2", Nueva(3));

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
    }

    [Fact]
    public async Task Update_devuelve_404_si_no_existe()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().PutAsJsonAsync("/api/profesiones/4", Nueva());

        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
    }

    [Fact]
    public async Task Delete_devuelve_204_y_borra_sus_estudios()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().DeleteAsync("/api/profesiones/2");

        Assert.Equal(HttpStatusCode.NoContent, respuesta.StatusCode);
        Assert.DoesNotContain(app.Datos.Profesiones, p => p.Id == 2);
        Assert.DoesNotContain(app.Datos.Estudios, e => e.IdProf == 2);
    }

    [Fact]
    public async Task Delete_devuelve_404_si_no_existe()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().DeleteAsync("/api/profesiones/99");

        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
    }
}
