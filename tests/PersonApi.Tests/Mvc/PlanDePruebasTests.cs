using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using personapi_dotnet.Interfaces;

namespace PersonApi.Tests.Mvc;

/// <summary>
/// Casos de la Tabla 21 (sección 6.9), en el mismo orden y con el mismo número.
/// Se recorren por HTTP sin navegador; lo que depende de JavaScript se comprueba con los atributos
/// data-val-* que usa jQuery Validation y con la validación del servidor.
/// </summary>
public partial class PlanDePruebasTests
{
    private static Dictionary<string, string> PersonaValida(string cc = "1098765432", string nombre = "Mateo") => new()
    {
        ["Cc"] = cc,
        ["Nombre"] = nombre,
        ["Apellido"] = "Castro",
        ["Genero"] = "M",
        ["Edad"] = "30"
    };

    /// <summary>Mensaje que la vista muestra junto a un campo (span asp-validation-for).</summary>
    private static string ErrorDe(string html, string campo)
    {
        var error = Regex.Match(html, $"data-valmsg-for=\"{Regex.Escape(campo)}\"[^>]*>([^<]*)<");
        return error.Success ? error.Groups[1].Value.Trim() : string.Empty;
    }

    [Fact]
    public async Task Caso01_Listar_muestra_los_datos_y_los_enlaces()
    {
        await using var app = new MvcFactory();

        var lista = await app.Navegador().Abrir("/Persona");

        Assert.Equal(HttpStatusCode.OK, lista.Codigo);
        Assert.Contains("Ana", lista.Html);
        Assert.Contains("Rodríguez", lista.Html);
        Assert.Contains("Femenino", lista.Html); // el género se muestra con su texto completo
        Assert.Contains("href=\"/Persona/Details/1012345678\"", lista.Html);
        Assert.Contains("href=\"/Persona/Edit/1012345678\"", lista.Html);
        Assert.Contains("href=\"/Persona/Delete/1012345678\"", lista.Html);
        Assert.Contains(">Detalles<", lista.Html);
        Assert.Contains(">Editar<", lista.Html);
        Assert.Contains(">Borrar<", lista.Html);
    }

    [Fact]
    public async Task Caso02_Crear_valido_redirige_a_la_lista_con_mensaje()
    {
        await using var app = new MvcFactory();
        var navegador = app.Navegador();

        var respuesta = await navegador.Enviar("/Persona/Create", PersonaValida());
        var lista = await navegador.Seguir(respuesta);

        Assert.Equal("/Persona", respuesta.Redireccion);
        Assert.Contains("Persona creada.", lista.Html);
        Assert.Contains("Mateo", lista.Html);
        Assert.Contains(app.Datos.Personas, p => p.Cc == 1098765432);
    }

    [Fact]
    public async Task Caso03_Crear_vacio_marca_cada_campo_y_el_navegador_puede_validar_sin_recargar()
    {
        await using var app = new MvcFactory();
        var navegador = app.Navegador();

        // Sin recargar: el formulario trae las reglas para jQuery Validation
        var formulario = await navegador.Abrir("/Persona/Create");
        Assert.Contains("data-val-required=\"El nombre es obligatorio.\"", formulario.Html);
        Assert.Contains("data-val-length-max=\"45\"", formulario.Html);
        Assert.Contains("jquery.validate", formulario.Html);

        var respuesta = await navegador.Enviar("/Persona/Create", new()
        {
            ["Cc"] = "",
            ["Nombre"] = "",
            ["Apellido"] = "",
            ["Genero"] = "",
            ["Edad"] = ""
        });

        Assert.Equal(HttpStatusCode.OK, respuesta.Codigo);
        Assert.NotEmpty(ErrorDe(respuesta.Html, "Cc"));
        Assert.Equal("El nombre es obligatorio.", ErrorDe(respuesta.Html, "Nombre"));
        Assert.Equal("El apellido es obligatorio.", ErrorDe(respuesta.Html, "Apellido"));
        Assert.Equal("El género es obligatorio.", ErrorDe(respuesta.Html, "Genero"));
        Assert.Empty(ErrorDe(respuesta.Html, "Edad")); // la edad es opcional
        Assert.Equal(3, app.Datos.Personas.Count);
    }

    [Fact]
    public async Task Caso04_Crear_sin_JavaScript_el_servidor_devuelve_los_mismos_errores()
    {
        await using var app = new MvcFactory();

        // Sin JavaScript el formulario llega al servidor tal cual; estos son datos que el navegador habría frenado
        var respuesta = await app.Navegador().Enviar("/Persona/Create", new()
        {
            ["Cc"] = "-5",
            ["Nombre"] = "",
            ["Apellido"] = "Castro",
            ["Genero"] = "X",
            ["Edad"] = "150"
        });

        Assert.Equal(HttpStatusCode.OK, respuesta.Codigo);
        Assert.Equal("La cédula debe ser un número positivo.", ErrorDe(respuesta.Html, "Cc"));
        Assert.Equal("El nombre es obligatorio.", ErrorDe(respuesta.Html, "Nombre"));
        Assert.Equal("El género debe ser Masculino (M) o Femenino (F).", ErrorDe(respuesta.Html, "Genero"));
        Assert.Equal("La edad debe estar entre 0 y 120.", ErrorDe(respuesta.Html, "Edad"));
        Assert.Equal(3, app.Datos.Personas.Count);
    }

    [Fact]
    public async Task Caso05_Llave_repetida_avisa_y_no_pierde_lo_escrito()
    {
        await using var app = new MvcFactory();

        var respuesta = await app.Navegador().Enviar("/Persona/Create", PersonaValida("1012345678", "Mateo"));

        Assert.Equal(HttpStatusCode.OK, respuesta.Codigo);
        Assert.Equal("La cédula ya existe.", ErrorDe(respuesta.Html, "Cc"));
        Assert.Contains("value=\"Mateo\"", respuesta.Html);
        Assert.Contains("value=\"Castro\"", respuesta.Html);
        Assert.Equal("Ana", app.Datos.Personas.Single(p => p.Cc == 1012345678).Nombre);
    }

    [Fact]
    public async Task Caso06_Longitud_maxima_rechaza_46_caracteres_en_nombre()
    {
        await using var app = new MvcFactory();

        var respuesta = await app.Navegador().Enviar("/Persona/Create", PersonaValida(nombre: new string('a', 46)));

        Assert.Equal(HttpStatusCode.OK, respuesta.Codigo);
        Assert.Equal("El nombre admite máximo 45 caracteres.", ErrorDe(respuesta.Html, "Nombre"));
        Assert.DoesNotContain(app.Datos.Personas, p => p.Cc == 1098765432);
    }

    [Fact]
    public async Task Caso07_Editar_cambia_el_apellido_y_la_cedula_no_se_edita()
    {
        await using var app = new MvcFactory();
        var navegador = app.Navegador();

        var formulario = await navegador.Abrir("/Persona/Edit/79123456");
        Assert.Contains("value=\"79123456\"", formulario.Html);
        Assert.DoesNotContain("name=\"Cc\"", formulario.Html); // la cédula se muestra pero no viaja

        // Aunque alguien envíe otra cédula a mano, se ignora (protección contra sobrepublicación)
        var respuesta = await navegador.Enviar("/Persona/Edit/79123456", new()
        {
            ["Cc"] = "999",
            ["Nombre"] = "Carlos",
            ["Apellido"] = "Gómez Ruiz",
            ["Genero"] = "M",
            ["Edad"] = "45"
        });
        var lista = await navegador.Seguir(respuesta);

        Assert.Contains("Persona actualizada.", lista.Html);
        Assert.Contains("Gómez Ruiz", lista.Html);
        Assert.Equal("Gómez Ruiz", app.Datos.Personas.Single(p => p.Cc == 79123456).Apellido);
        Assert.DoesNotContain(app.Datos.Personas, p => p.Cc == 999);
    }

    [Theory]
    [InlineData("/Persona/Details/999")]
    [InlineData("/Persona/Edit/999")]
    [InlineData("/Persona/Delete/999")]
    [InlineData("/Profesion/Details/999")]
    [InlineData("/Estudio/Details?idProf=1&ccPer=79123456")] // las dos llaves existen, pero no juntas
    [InlineData("/Estudio/Edit?idProf=1")]
    [InlineData("/Telefono/Details/0000000")]
    public async Task Caso08_No_existe_responde_404(string url)
    {
        await using var app = new MvcFactory();

        var pagina = await app.Navegador().Abrir(url);

        Assert.Equal(HttpStatusCode.NotFound, pagina.Codigo);
    }

    [Fact]
    public async Task Caso09_Borrar_una_persona_con_estudios_avisa_los_dependientes()
    {
        await using var app = new MvcFactory();
        var navegador = app.Navegador();

        var confirmacion = await navegador.Abrir("/Persona/Delete/1012345678");
        Assert.Contains("2 estudio(s)", confirmacion.Html);
        Assert.Contains("2 teléfono(s)", confirmacion.Html);

        var lista = await navegador.Seguir(await navegador.Enviar("/Persona/Delete/1012345678", []));

        Assert.Contains("Persona eliminada junto con sus estudios y teléfonos.", lista.Html);
        Assert.DoesNotContain(app.Datos.Estudios, e => e.CcPer == 1012345678);
        Assert.DoesNotContain(app.Datos.Telefonos, t => t.Duenio == 1012345678);
    }

    [Fact]
    public async Task Caso10_Recargar_tras_guardar_no_crea_un_duplicado()
    {
        await using var app = new MvcFactory();
        var navegador = app.Navegador();

        var respuesta = await navegador.Enviar("/Persona/Create", PersonaValida());
        Assert.Equal(HttpStatusCode.Redirect, respuesta.Codigo); // PRG: el POST no devuelve una vista

        var primera = await navegador.Seguir(respuesta);
        var recarga = await navegador.Abrir("/Persona"); // F5 repite el GET, no el POST

        Assert.Contains("Persona creada.", primera.Html);
        Assert.DoesNotContain("Persona creada.", recarga.Html); // TempData dura una sola redirección
        Assert.Single(app.Datos.Personas, p => p.Cc == 1098765432);
        Assert.Equal(4, app.Datos.Personas.Count);
    }

    [Fact]
    public async Task Caso11_Las_listas_de_Estudio_y_Telefono_muestran_nombres()
    {
        await using var app = new MvcFactory();
        var navegador = app.Navegador();

        var estudio = await navegador.Abrir("/Estudio/Create");
        var telefono = await navegador.Abrir("/Telefono/Create");

        Assert.Contains("<option value=\"1012345678\">Ana Pérez</option>", estudio.Html);
        Assert.Contains("<option value=\"1\">Ingeniería de Sistemas</option>", estudio.Html);
        Assert.Contains("<option value=\"79123456\">Carlos Gómez</option>", telefono.Html);
    }

    [Fact]
    public async Task Caso12_Si_el_formulario_vuelve_con_error_las_listas_siguen_llenas()
    {
        await using var app = new MvcFactory();
        var navegador = app.Navegador();
        var manana = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");

        var estudio = await navegador.Enviar("/Estudio/Create", new()
        {
            ["Estudio.CcPer"] = "79123456",
            ["Estudio.IdProf"] = "1",
            ["Estudio.Univer"] = "Universidad de los Andes",
            ["Estudio.Fecha"] = manana
        });
        var telefono = await navegador.Enviar("/Telefono/Create", new()
        {
            ["Telefono.Num"] = "",
            ["Telefono.Oper"] = "WOM",
            ["Telefono.Duenio"] = "52987654"
        });

        Assert.Equal("La fecha no puede ser futura.", ErrorDe(estudio.Html, "Estudio.Fecha"));
        Assert.Contains("<option selected=\"selected\" value=\"79123456\">Carlos Gómez</option>", estudio.Html);
        Assert.Contains("<option value=\"2\">Medicina</option>", estudio.Html);
        Assert.Equal("El número es obligatorio.", ErrorDe(telefono.Html, "Telefono.Num"));
        Assert.Contains("<option selected=\"selected\" value=\"52987654\">Luisa Rodríguez</option>", telefono.Html);
        Assert.Equal(3, app.Datos.Estudios.Count);
        Assert.Equal(3, app.Datos.Telefonos.Count);
    }

    [Fact]
    public async Task Caso13_Pareja_repetida_en_Estudio_muestra_su_mensaje()
    {
        await using var app = new MvcFactory();

        var respuesta = await app.Navegador().Enviar("/Estudio/Create", new()
        {
            ["Estudio.CcPer"] = "1012345678",
            ["Estudio.IdProf"] = "1",
            ["Estudio.Univer"] = "Otra",
            ["Estudio.Fecha"] = ""
        });

        Assert.Equal(HttpStatusCode.OK, respuesta.Codigo);
        Assert.Contains("Esta persona ya tiene registrada esa profesión.", respuesta.Html);
        Assert.Equal("Pontificia Universidad Javeriana",
            app.Datos.Estudios.Single(e => e.IdProf == 1 && e.CcPer == 1012345678).Univer);
    }

    [Fact]
    public async Task Caso14_Lo_creado_en_la_vista_queda_en_el_repositorio_que_usa_la_API()
    {
        await using var app = new MvcFactory();

        await app.Navegador().Enviar("/Persona/Create", PersonaValida());

        // La API (rama feature/api-rest) consulta este mismo IPersonaRepository; la comprobación
        // contra GET /api/personas se hace en la rama de integración, donde están las dos partes.
        using var alcance = app.Services.CreateScope();
        var repositorio = alcance.ServiceProvider.GetRequiredService<IPersonaRepository>();
        var creada = await repositorio.GetByIdAsync(1098765432);
        Assert.Equal("Mateo", creada?.Nombre);
        Assert.Contains(await repositorio.GetAllAsync(), p => p.Cc == 1098765432);
    }

    [Fact]
    public async Task Caso15_Ningun_enlace_del_menu_ni_del_inicio_esta_roto()
    {
        await using var app = new MvcFactory();
        var navegador = app.Navegador();

        var inicio = await navegador.Abrir("/");
        var enlaces = EnlaceRegex().Matches(inicio.Html).Select(m => m.Groups[1].Value)
            .Where(href => href.StartsWith('/') && !href.StartsWith("/lib/") && !href.StartsWith("/css/") && !href.Contains(".css"))
            .Distinct()
            .ToList();

        Assert.Contains("/Persona", enlaces);
        Assert.Contains("/Profesion", enlaces);
        Assert.Contains("/Estudio", enlaces);
        Assert.Contains("/Telefono", enlaces);
        Assert.Contains("/swagger", enlaces);

        // Swagger lo configura la rama feature/api-rest; aquí solo se comprueba que el menú lo enlaza
        foreach (var enlace in enlaces.Where(e => e != "/swagger"))
        {
            var pagina = await navegador.Abrir(enlace);
            Assert.True(pagina.Codigo == HttpStatusCode.OK, $"El enlace {enlace} respondió {(int)pagina.Codigo}.");
        }
    }

    [GeneratedRegex("<a [^>]*href=\"([^\"]+)\"")]
    private static partial Regex EnlaceRegex();
}
