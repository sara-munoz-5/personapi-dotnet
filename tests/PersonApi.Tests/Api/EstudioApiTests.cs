using System.Net;
using System.Net.Http.Json;
using personapi_dotnet.Models.Entities;

namespace PersonApi.Tests.Api;

/// <summary>
/// Los 5 endpoints de /api/estudios. La llave es compuesta (idProf, ccPer), así que se prueba
/// que las dos partes viajan en la URL y que basta que una no coincida para responder 400 o 404.
/// </summary>
public class EstudioApiTests
{
    // Carlos (79123456) no tiene estudios de Ingeniería de Sistemas (1): pareja libre
    private static Estudio Nuevo(int idProf = 1, int ccPer = 79123456) =>
        new() { IdProf = idProf, CcPer = ccPer, Univer = "Universidad de los Andes", Fecha = new DateOnly(2010, 7, 1) };

    [Fact]
    public async Task GetAll_devuelve_200_con_persona_y_profesion()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().GetAsync("/api/estudios");

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var estudios = await respuesta.Content.ReadFromJsonAsync<List<Estudio>>();
        Assert.Equal(3, estudios!.Count);
        Assert.All(estudios, e =>
        {
            Assert.NotNull(e.CcPerNavigation);
            Assert.NotNull(e.IdProfNavigation);
        });
    }

    [Fact]
    public async Task GetById_devuelve_200_con_la_llave_compuesta()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().GetAsync("/api/estudios/2/79123456");

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var estudio = await respuesta.Content.ReadFromJsonAsync<Estudio>();
        Assert.Equal("Universidad Nacional de Colombia", estudio!.Univer);
        Assert.Equal(new DateOnly(2005, 12, 10), estudio.Fecha);
    }

    [Theory]
    [InlineData("/api/estudios/1/79123456")] // las dos llaves existen, pero no juntas
    [InlineData("/api/estudios/99/1012345678")]
    [InlineData("/api/estudios/1/1")]
    public async Task GetById_devuelve_404_si_la_pareja_no_existe(string url)
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().GetAsync(url);

        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
    }

    [Fact]
    public async Task Create_devuelve_201_con_las_dos_llaves_en_la_ubicacion()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().PostAsJsonAsync("/api/estudios", Nuevo());

        Assert.Equal(HttpStatusCode.Created, respuesta.StatusCode);
        Assert.Equal("/api/estudios/1/79123456", respuesta.Headers.Location?.AbsolutePath);
        Assert.Contains(app.Datos.Estudios, e => e.IdProf == 1 && e.CcPer == 79123456);
    }

    [Fact]
    public async Task Create_devuelve_409_si_la_persona_ya_tiene_esa_profesion()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().PostAsJsonAsync("/api/estudios", Nuevo(2, 79123456));

        Assert.Equal(HttpStatusCode.Conflict, respuesta.StatusCode);
        Assert.Single(app.Datos.Estudios, e => e.IdProf == 2 && e.CcPer == 79123456);
    }

    [Theory]
    [InlineData(99, 79123456)] // la profesión no existe
    [InlineData(1, 1)]         // la persona no existe
    public async Task Create_devuelve_409_si_una_llave_foranea_no_existe(int idProf, int ccPer)
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().PostAsJsonAsync("/api/estudios", Nuevo(idProf, ccPer));

        Assert.Equal(HttpStatusCode.Conflict, respuesta.StatusCode);
        Assert.Equal(3, app.Datos.Estudios.Count);
    }

    [Fact]
    public async Task Update_devuelve_204_y_cambia_fecha_y_universidad()
    {
        await using var app = new ApiFactory();
        var cambios = new Estudio { IdProf = 3, CcPer = 1012345678, Univer = "Universidad Externado", Fecha = new DateOnly(2023, 11, 30) };

        var respuesta = await app.CreateClient().PutAsJsonAsync("/api/estudios/3/1012345678", cambios);

        Assert.Equal(HttpStatusCode.NoContent, respuesta.StatusCode);
        var estudio = app.Datos.Estudios.Single(e => e.IdProf == 3 && e.CcPer == 1012345678);
        Assert.Equal("Universidad Externado", estudio.Univer);
        Assert.Equal(new DateOnly(2023, 11, 30), estudio.Fecha);
    }

    [Theory]
    [InlineData("/api/estudios/3/1012345678", 1, 1012345678)] // no coincide idProf
    [InlineData("/api/estudios/3/1012345678", 3, 79123456)]   // no coincide ccPer
    public async Task Update_devuelve_400_si_alguna_llave_de_la_url_no_coincide(string url, int idProf, int ccPer)
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().PutAsJsonAsync(url, Nuevo(idProf, ccPer));

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
    }

    [Fact]
    public async Task Update_devuelve_404_si_la_pareja_no_existe()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().PutAsJsonAsync("/api/estudios/1/79123456", Nuevo());

        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
    }

    [Fact]
    public async Task Delete_devuelve_204_y_borra_solo_ese_estudio()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().DeleteAsync("/api/estudios/1/1012345678");

        Assert.Equal(HttpStatusCode.NoContent, respuesta.StatusCode);
        Assert.DoesNotContain(app.Datos.Estudios, e => e.IdProf == 1 && e.CcPer == 1012345678);
        // Ana conserva su otro estudio y la persona y la profesión siguen existiendo
        Assert.Contains(app.Datos.Estudios, e => e.IdProf == 3 && e.CcPer == 1012345678);
        Assert.Contains(app.Datos.Personas, p => p.Cc == 1012345678);
        Assert.Contains(app.Datos.Profesiones, p => p.Id == 1);
    }

    [Fact]
    public async Task Delete_devuelve_404_si_la_pareja_no_existe()
    {
        await using var app = new ApiFactory();

        var respuesta = await app.CreateClient().DeleteAsync("/api/estudios/1/79123456");

        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
    }
}
