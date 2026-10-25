using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;

namespace PersonApi.Tests.Api;

/// <summary>
/// Las reglas de Models/Metadata (Tabla 19) también protegen la API: [ApiController] responde 400
/// con los mensajes en español antes de llegar al repositorio, en el POST y en el PUT.
/// </summary>
public class ValidacionApiTests
{
    private static readonly string Largo46 = new('a', 46);
    private static readonly string Largo51 = new('a', 51);
    private static readonly string Largo91 = new('a', 91);
    private static readonly string Manana = DateOnly.FromDateTime(DateTime.Today.AddDays(1)).ToString("yyyy-MM-dd");

    public static TheoryData<string, string, object, string> CasosInvalidos => new()
    {
        // método, url, cuerpo, mensaje esperado
        { "POST", "/api/personas", new { cc = 1098765432, nombre = "", apellido = "Castro", genero = "M" }, "El nombre es obligatorio." },
        { "POST", "/api/personas", new { cc = 1098765432, nombre = Largo46, apellido = "Castro", genero = "M" }, "El nombre admite máximo 45 caracteres." },
        { "POST", "/api/personas", new { cc = 1098765432, nombre = "Mateo", apellido = "Castro", genero = "X" }, "El género debe ser Masculino (M) o Femenino (F)." },
        { "POST", "/api/personas", new { cc = 1098765432, nombre = "Mateo", apellido = "Castro", genero = "M", edad = 150 }, "La edad debe estar entre 0 y 120." },
        { "PUT", "/api/personas/79123456", new { cc = 79123456, nombre = "Carlos", apellido = "", genero = "M" }, "El apellido es obligatorio." },
        { "PUT", "/api/personas/79123456", new { cc = 79123456, nombre = "Carlos", apellido = Largo46, genero = "M" }, "El apellido admite máximo 45 caracteres." },
        { "PUT", "/api/personas/79123456", new { cc = 79123456, nombre = "Carlos", apellido = "Gómez", genero = "H" }, "El género debe ser Masculino (M) o Femenino (F)." },

        { "POST", "/api/profesiones", new { id = 4, nom = "" }, "El nombre es obligatorio." },
        { "POST", "/api/profesiones", new { id = 4, nom = Largo91 }, "El nombre admite máximo 90 caracteres." },
        { "PUT", "/api/profesiones/3", new { id = 3, nom = "" }, "El nombre es obligatorio." },
        { "PUT", "/api/profesiones/3", new { id = 3, nom = Largo91 }, "El nombre admite máximo 90 caracteres." },

        { "POST", "/api/estudios", new { idProf = 1, ccPer = 79123456, fecha = Manana }, "La fecha no puede ser futura." },
        { "POST", "/api/estudios", new { idProf = 1, ccPer = 79123456, univer = Largo51 }, "La universidad admite máximo 50 caracteres." },
        { "POST", "/api/estudios", new { idProf = 0, ccPer = 79123456 }, "Seleccione una profesión." },
        { "PUT", "/api/estudios/2/79123456", new { idProf = 2, ccPer = 79123456, fecha = Manana }, "La fecha no puede ser futura." },
        { "PUT", "/api/estudios/2/79123456", new { idProf = 2, ccPer = 79123456, univer = Largo51 }, "La universidad admite máximo 50 caracteres." },

        { "POST", "/api/telefonos", new { num = "", oper = "WOM", duenio = 52987654 }, "El número es obligatorio." },
        { "POST", "/api/telefonos", new { num = "3201112233", oper = "", duenio = 52987654 }, "El operador es obligatorio." },
        { "POST", "/api/telefonos", new { num = "3201112233445566", oper = "WOM", duenio = 52987654 }, "El número admite máximo 15 caracteres." },
        { "PUT", "/api/telefonos/3001234567", new { num = "3001234567", oper = Largo46, duenio = 1012345678 }, "El operador admite máximo 45 caracteres." },
        { "PUT", "/api/telefonos/3001234567", new { num = "3001234567", oper = "", duenio = 1012345678 }, "El operador es obligatorio." },
    };

    [Theory]
    [MemberData(nameof(CasosInvalidos))]
    public async Task Datos_invalidos_responden_400_con_el_mensaje_en_espanol(string metodo, string url, object cuerpo, string mensaje)
    {
        await using var app = new ApiFactory();
        var cliente = app.CreateClient();
        var antes = Instantanea(app);

        var respuesta = metodo == "POST"
            ? await cliente.PostAsJsonAsync(url, cuerpo)
            : await cliente.PutAsJsonAsync(url, cuerpo);

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
        var problema = await respuesta.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.Contains(mensaje, problema!.Errors.SelectMany(e => e.Value));
        Assert.Equal("Uno o más datos no son válidos.", problema.Title);
        // El repositorio no se tocó
        Assert.Equal(antes, Instantanea(app));
    }

    [Fact]
    public async Task Un_registro_valido_no_lo_detiene_la_validacion()
    {
        await using var app = new ApiFactory();
        var hoy = DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd");

        var respuesta = await app.CreateClient().PostAsJsonAsync("/api/estudios",
            new { idProf = 1, ccPer = 79123456, fecha = hoy, univer = new string('a', 50) });

        Assert.Equal(HttpStatusCode.Created, respuesta.StatusCode);
    }

    [Fact]
    public async Task Los_titulos_de_404_y_400_por_llave_distinta_estan_en_espanol()
    {
        await using var app = new ApiFactory();
        var cliente = app.CreateClient();

        var noExiste = await (await cliente.GetAsync("/api/personas/1")).Content.ReadFromJsonAsync<ProblemDetails>();
        var distinta = await (await cliente.PutAsJsonAsync("/api/profesiones/2",
            new { id = 3, nom = "Derecho" })).Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal("No se encontró el registro.", noExiste!.Title);
        Assert.Equal("El id de la URL no coincide con el del cuerpo.", distinta!.Title);
    }

    private static string Instantanea(ApiFactory app) =>
        string.Join("|", app.Datos.Personas.Select(p => $"{p.Cc}:{p.Nombre}:{p.Apellido}:{p.Genero}:{p.Edad}"))
        + string.Join("|", app.Datos.Profesiones.Select(p => $"{p.Id}:{p.Nom}"))
        + string.Join("|", app.Datos.Estudios.Select(e => $"{e.IdProf}:{e.CcPer}:{e.Fecha}:{e.Univer}"))
        + string.Join("|", app.Datos.Telefonos.Select(t => $"{t.Num}:{t.Oper}:{t.Duenio}"));
}
