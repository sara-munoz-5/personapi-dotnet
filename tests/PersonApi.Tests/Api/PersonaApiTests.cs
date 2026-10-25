using System.Net;
using System.Net.Http.Json;
using personapi_dotnet.Models.Entities;

namespace PersonApi.Tests.Api;

/// <summary>Los 5 endpoints de /api/personas con sus códigos de la Tabla 12.</summary>
public class PersonaApiTests
{
    private static Persona Nueva(int cc = 1098765432) =>
        new() { Cc = cc, Nombre = "Mateo", Apellido = "Castro", Genero = "M", Edad = 30 };

    [Fact]
    public async Task GetAll_devuelve_200_con_todas_las_personas()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().GetAsync("/api/personas");

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var personas = await respuesta.Content.ReadFromJsonAsync<List<Persona>>();
        Assert.Equal(3, personas!.Count);
    }

    [Fact]
    public async Task GetById_devuelve_200_con_sus_estudios_y_telefonos()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().GetAsync("/api/personas/1012345678");

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var ana = await respuesta.Content.ReadFromJsonAsync<Persona>();
        Assert.Equal("Ana", ana!.Nombre);
        Assert.Equal(2, ana.Estudios.Count);
        Assert.Equal(2, ana.Telefonos.Count);
    }

    [Fact]
    public async Task GetById_devuelve_404_si_no_existe()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().GetAsync("/api/personas/1");

        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
    }

    [Fact]
    public async Task Create_devuelve_201_con_la_ubicacion_del_nuevo_recurso()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().PostAsJsonAsync("/api/personas", Nueva());

        Assert.Equal(HttpStatusCode.Created, respuesta.StatusCode);
        Assert.Equal("/api/personas/1098765432", respuesta.Headers.Location?.AbsolutePath);
        Assert.Contains(app.Datos.Personas, p => p.Cc == 1098765432);
    }

    [Fact]
    public async Task Create_devuelve_409_si_la_cedula_ya_existe()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().PostAsJsonAsync("/api/personas", Nueva(1012345678));

        Assert.Equal(HttpStatusCode.Conflict, respuesta.StatusCode);
        Assert.Single(app.Datos.Personas, p => p.Cc == 1012345678);
    }

    [Fact]
    public async Task Create_devuelve_409_si_la_base_rechaza_el_guardado()
    {
        await using var app = new ApiFactory();
        app.Datos.FallarAlGuardar = true;

        var respuesta = await app.CreateClient().PostAsJsonAsync("/api/personas", Nueva());

        Assert.Equal(HttpStatusCode.Conflict, respuesta.StatusCode);
    }

    [Fact]
    public async Task Create_devuelve_400_si_el_cuerpo_no_es_valido()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().PostAsync("/api/personas",
            JsonContent.Create(new { cc = "no es un número", nombre = "Mateo" }));

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
    }

    [Fact]
    public async Task Update_devuelve_204_y_guarda_los_cambios()
    {
        await using var app = new ApiFactory();
        var cambios = new Persona { Cc = 79123456, Nombre = "Carlos Andrés", Apellido = "Gómez", Genero = "M", Edad = 46 };

        var respuesta = await app.CreateClient().PutAsJsonAsync("/api/personas/79123456", cambios);

        Assert.Equal(HttpStatusCode.NoContent, respuesta.StatusCode);
        var carlos = app.Datos.Personas.Single(p => p.Cc == 79123456);
        Assert.Equal("Carlos Andrés", carlos.Nombre);
        Assert.Equal(46, carlos.Edad);
    }

    [Fact]
    public async Task Update_devuelve_400_si_la_cedula_de_la_url_no_coincide()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().PutAsJsonAsync("/api/personas/79123456", Nueva(52987654));

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
    }

    [Fact]
    public async Task Update_devuelve_404_si_no_existe()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().PutAsJsonAsync("/api/personas/1098765432", Nueva());

        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
    }

    [Fact]
    public async Task Delete_devuelve_204_y_borra_en_cascada()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().DeleteAsync("/api/personas/1012345678");

        Assert.Equal(HttpStatusCode.NoContent, respuesta.StatusCode);
        Assert.DoesNotContain(app.Datos.Personas, p => p.Cc == 1012345678);
        Assert.DoesNotContain(app.Datos.Telefonos, t => t.Duenio == 1012345678);
    }

    [Fact]
    public async Task Delete_devuelve_404_si_no_existe()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().DeleteAsync("/api/personas/1");

        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
    }
}
