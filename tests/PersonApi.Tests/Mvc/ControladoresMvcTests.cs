using System.Net;

namespace PersonApi.Tests.Mvc;

/// <summary>
/// Puntos de las secciones 4.6 y 6.3 a 6.6 que la Tabla 21 no cubre: antifalsificación, errores de la base,
/// llave compuesta en los enlaces, nombres en lugar de llaves y el CRUD de las otras tres entidades.
/// </summary>
public class ControladoresMvcTests
{
    [Theory]
    [InlineData("/Persona/Create")]
    [InlineData("/Persona/Edit/79123456")]
    [InlineData("/Persona/Delete/79123456")]
    [InlineData("/Profesion/Create")]
    [InlineData("/Estudio/Delete?idProf=2&ccPer=79123456")]
    [InlineData("/Telefono/Delete/3001234567")]
    public async Task Un_POST_sin_token_antifalsificacion_se_rechaza(string url)
    {
        await using var app = new MvcFactory();

        var respuesta = await app.Navegador().EnviarSinToken(url, new() { ["Nombre"] = "Intruso" });

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.Codigo);
        Assert.Equal(3, app.Datos.Personas.Count);
        Assert.Equal(3, app.Datos.Estudios.Count);
        Assert.Equal(3, app.Datos.Telefonos.Count);
    }

    [Fact]
    public async Task Si_la_base_rechaza_el_guardado_el_formulario_muestra_el_error()
    {
        await using var app = new MvcFactory();
        app.Datos.FallarAlGuardar = true;

        var respuesta = await app.Navegador().Enviar("/Profesion/Create", new() { ["Id"] = "4", ["Nom"] = "Arquitectura", ["Des"] = "" });

        Assert.Equal(HttpStatusCode.OK, respuesta.Codigo);
        Assert.Contains("No se pudo guardar la profesión. Verifique que el id no esté repetido.", respuesta.Html);
        Assert.Contains("value=\"Arquitectura\"", respuesta.Html);
    }

    [Fact]
    public async Task Si_la_base_rechaza_el_borrado_vuelve_a_la_confirmacion_con_el_error()
    {
        await using var app = new MvcFactory();
        var navegador = app.Navegador();
        app.Datos.FallarAlGuardar = true;

        var respuesta = await navegador.Enviar("/Telefono/Delete/3001234567", []);
        var confirmacion = await navegador.Seguir(respuesta);

        Assert.Equal("/Telefono/Delete/3001234567", respuesta.Redireccion);
        Assert.Contains("No se pudo eliminar el teléfono. Intente de nuevo.", confirmacion.Html);
        Assert.Contains(app.Datos.Telefonos, t => t.Num == "3001234567");
    }

    [Fact]
    public async Task Detalle_de_persona_muestra_sus_estudios_y_telefonos()
    {
        await using var app = new MvcFactory();

        var detalle = await app.Navegador().Abrir("/Persona/Details/1012345678");

        Assert.Contains("Ana Pérez", detalle.Html);
        Assert.Contains("Ingeniería de Sistemas", detalle.Html);
        Assert.Contains("Derecho", detalle.Html);
        Assert.Contains("15/06/2020", detalle.Html);
        Assert.Contains("3001234567", detalle.Html);
        Assert.Contains("3155551234", detalle.Html);
    }

    [Fact]
    public async Task Las_listas_vacias_muestran_un_mensaje()
    {
        await using var app = new MvcFactory();
        app.Datos.Telefonos.Clear();
        app.Datos.Estudios.Clear();
        var navegador = app.Navegador();

        Assert.Contains("No hay teléfonos registrados.", (await navegador.Abrir("/Telefono")).Html);
        Assert.Contains("No hay estudios registrados.", (await navegador.Abrir("/Estudio")).Html);
        Assert.Contains("No tiene estudios registrados.", (await navegador.Abrir("/Persona/Details/52987654")).Html);
    }

    [Fact]
    public async Task Profesion_crear_editar_y_borrar_con_su_conteo_de_estudios()
    {
        await using var app = new MvcFactory();
        var navegador = app.Navegador();

        var creada = await navegador.Seguir(await navegador.Enviar("/Profesion/Create",
            new() { ["Id"] = "4", ["Nom"] = "Arquitectura", ["Des"] = "Diseño de edificaciones" }));
        Assert.Contains("Profesión creada.", creada.Html);

        var repetida = await navegador.Enviar("/Profesion/Create", new() { ["Id"] = "1", ["Nom"] = "Otra", ["Des"] = "" });
        Assert.Contains("El id de la profesión ya existe.", repetida.Html);

        var editada = await navegador.Seguir(await navegador.Enviar("/Profesion/Edit/4",
            new() { ["Id"] = "99", ["Nom"] = "Arquitectura", ["Des"] = "Diseño urbano" }));
        Assert.Contains("Profesión actualizada.", editada.Html);
        Assert.Equal("Diseño urbano", app.Datos.Profesiones.Single(p => p.Id == 4).Des);
        Assert.DoesNotContain(app.Datos.Profesiones, p => p.Id == 99);

        var confirmacion = await navegador.Abrir("/Profesion/Delete/1");
        Assert.Contains("1 estudio(s)", confirmacion.Html);
        var borrada = await navegador.Seguir(await navegador.Enviar("/Profesion/Delete/1", []));
        Assert.Contains("Profesión eliminada junto con sus estudios.", borrada.Html);
        Assert.DoesNotContain(app.Datos.Estudios, e => e.IdProf == 1);
    }

    [Fact]
    public async Task Estudio_muestra_nombres_y_enlaza_con_las_dos_llaves()
    {
        await using var app = new MvcFactory();

        var lista = await app.Navegador().Abrir("/Estudio");

        Assert.Contains("Ana Pérez", lista.Html);
        Assert.Contains("Ingeniería de Sistemas", lista.Html);
        Assert.Contains("href=\"/Estudio/Details?idProf=1&ccPer=1012345678\"", lista.Html);
        Assert.Contains("href=\"/Estudio/Edit?idProf=1&ccPer=1012345678\"", lista.Html);
        Assert.Contains("href=\"/Estudio/Delete?idProf=1&ccPer=1012345678\"", lista.Html);
    }

    [Fact]
    public async Task Estudio_crear_y_editar_solo_cambia_fecha_y_universidad()
    {
        await using var app = new MvcFactory();
        var navegador = app.Navegador();

        var creado = await navegador.Seguir(await navegador.Enviar("/Estudio/Create", new()
        {
            ["Estudio.CcPer"] = "79123456",
            ["Estudio.IdProf"] = "1",
            ["Estudio.Univer"] = "Universidad de los Andes",
            ["Estudio.Fecha"] = "2010-07-01"
        }));
        Assert.Contains("Estudio creado.", creado.Html);

        var formulario = await navegador.Abrir("/Estudio/Edit?idProf=1&ccPer=79123456");
        Assert.Contains("value=\"Carlos Gómez\"", formulario.Html); // persona y profesión de solo lectura
        Assert.DoesNotContain("name=\"Estudio.CcPer\"", formulario.Html);

        // Se intenta cambiar también la llave: se ignora
        var editado = await navegador.Seguir(await navegador.Enviar("/Estudio/Edit?idProf=1&ccPer=79123456", new()
        {
            ["Estudio.CcPer"] = "52987654",
            ["Estudio.IdProf"] = "3",
            ["Estudio.Univer"] = "Universidad EAFIT",
            ["Estudio.Fecha"] = "2011-08-02"
        }));
        Assert.Contains("Estudio actualizado.", editado.Html);
        var estudio = app.Datos.Estudios.Single(e => e.IdProf == 1 && e.CcPer == 79123456);
        Assert.Equal("Universidad EAFIT", estudio.Univer);
        Assert.Equal(new DateOnly(2011, 8, 2), estudio.Fecha);
        Assert.DoesNotContain(app.Datos.Estudios, e => e.CcPer == 52987654);
    }

    [Fact]
    public async Task Estudio_borrar_con_la_llave_compuesta()
    {
        await using var app = new MvcFactory();
        var navegador = app.Navegador();

        var lista = await navegador.Seguir(await navegador.Enviar("/Estudio/Delete?idProf=3&ccPer=1012345678", []));

        Assert.Contains("Estudio eliminado.", lista.Html);
        Assert.DoesNotContain(app.Datos.Estudios, e => e.IdProf == 3 && e.CcPer == 1012345678);
        Assert.Contains(app.Datos.Estudios, e => e.IdProf == 1 && e.CcPer == 1012345678);
    }

    [Fact]
    public async Task Telefono_muestra_el_nombre_del_duenio_y_se_puede_cambiar_de_duenio()
    {
        await using var app = new MvcFactory();
        var navegador = app.Navegador();

        var lista = await navegador.Abrir("/Telefono");
        Assert.Contains("Carlos Gómez", lista.Html);

        var formulario = await navegador.Abrir("/Telefono/Edit/3155551234");
        Assert.Contains("<option selected=\"selected\" value=\"1012345678\">Ana Pérez</option>", formulario.Html);

        var editado = await navegador.Seguir(await navegador.Enviar("/Telefono/Edit/3155551234", new()
        {
            ["Telefono.Num"] = "9999999999",
            ["Telefono.Oper"] = "WOM",
            ["Telefono.Duenio"] = "52987654"
        }));
        Assert.Contains("Teléfono actualizado.", editado.Html);

        var telefono = app.Datos.Telefonos.Single(t => t.Num == "3155551234");
        Assert.Equal(52987654, telefono.Duenio);
        Assert.Equal("WOM", telefono.Oper);
        Assert.DoesNotContain(app.Datos.Telefonos, t => t.Num == "9999999999"); // el número es la llave
        // El teléfono pasó de la colección de Ana a la de Luisa
        Assert.Single(app.Datos.Personas.Single(p => p.Cc == 1012345678).Telefonos);
        Assert.Contains("3155551234", (await navegador.Abrir("/Persona/Details/52987654")).Html);
    }

    [Fact]
    public async Task Telefono_crear_con_numero_repetido_avisa()
    {
        await using var app = new MvcFactory();

        var respuesta = await app.Navegador().Enviar("/Telefono/Create", new()
        {
            ["Telefono.Num"] = "3001234567",
            ["Telefono.Oper"] = "WOM",
            ["Telefono.Duenio"] = "52987654"
        });

        Assert.Equal(HttpStatusCode.OK, respuesta.Codigo);
        Assert.Contains("El número de teléfono ya existe.", respuesta.Html);
        Assert.Equal("Claro", app.Datos.Telefonos.Single(t => t.Num == "3001234567").Oper);
    }
}
