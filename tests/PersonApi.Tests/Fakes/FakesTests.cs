using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Models.Entities;

namespace PersonApi.Tests.Fakes;

/// <summary>
/// Confirma que el proyecto compila contra las interfaces DAO y que los repositorios falsos
/// se comportan como la base: relaciones cargadas, llave repetida y borrado en cascada.
/// </summary>
public class FakesTests
{
    [Fact]
    public async Task Los_datos_de_ejemplo_traen_las_relaciones_cargadas()
    {
        var datos = FakeDatos.ConEjemplos();
        var personas = new FakePersonaRepository(datos);
        var estudios = new FakeEstudioRepository(datos);

        var ana = await personas.GetByIdAsync(1012345678);
        var estudio = await estudios.GetByIdAsync(1, 1012345678);

        Assert.NotNull(ana);
        Assert.Equal(2, ana.Estudios.Count);
        Assert.Equal(2, ana.Telefonos.Count);
        Assert.Equal("Ingeniería de Sistemas", estudio?.IdProfNavigation.Nom);
        Assert.Equal("Ana", estudio?.CcPerNavigation.Nombre);
    }

    [Fact]
    public async Task Crear_con_llave_repetida_lanza_DbUpdateException()
    {
        var personas = new FakePersonaRepository(FakeDatos.ConEjemplos());

        await Assert.ThrowsAsync<DbUpdateException>(() =>
            personas.AddAsync(new Persona { Cc = 1012345678, Nombre = "Otra", Apellido = "Persona", Genero = "F" }));
    }

    [Fact]
    public async Task Borrar_una_persona_borra_en_cascada_sus_estudios_y_telefonos()
    {
        var datos = FakeDatos.ConEjemplos();
        var personas = new FakePersonaRepository(datos);

        await personas.DeleteAsync(1012345678);

        Assert.Null(await personas.GetByIdAsync(1012345678));
        Assert.DoesNotContain(datos.Estudios, e => e.CcPer == 1012345678);
        Assert.DoesNotContain(datos.Telefonos, t => t.Duenio == 1012345678);
        // La profesión 1 solo la había estudiado Ana: su colección queda vacía
        Assert.Empty(datos.Profesiones.Single(p => p.Id == 1).Estudios);
    }

    [Fact]
    public async Task Cada_instancia_tiene_sus_propios_datos()
    {
        var primera = FakeDatos.ConEjemplos();
        await new FakeProfesionRepository(primera).DeleteAsync(1);

        var segunda = FakeDatos.ConEjemplos();

        Assert.NotNull(await new FakeProfesionRepository(segunda).GetByIdAsync(1));
    }
}
