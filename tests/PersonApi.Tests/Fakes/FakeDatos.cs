using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Models.Entities;

namespace PersonApi.Tests.Fakes;

/// <summary>
/// "Base de datos" en memoria que comparten los cuatro repositorios falsos.
/// Imita lo que hará persona_db: llaves únicas, FK obligatorias y borrado en cascada.
/// Cada prueba crea su propia instancia, así que las pruebas no dependen del orden.
/// </summary>
public class FakeDatos
{
    public List<Profesion> Profesiones { get; } = new();

    public List<Persona> Personas { get; } = new();

    public List<Estudio> Estudios { get; } = new();

    public List<Telefono> Telefonos { get; } = new();

    /// <summary>
    /// Si es verdadero, crear, actualizar y eliminar lanzan <see cref="DbUpdateException"/>,
    /// como cuando la base rechaza el cambio (por ejemplo, una llave repetida en una carrera).
    /// </summary>
    public bool FallarAlGuardar { get; set; }

    /// <summary>Datos de ejemplo: 3 profesiones, 3 personas, 3 estudios y 3 teléfonos.</summary>
    public static FakeDatos ConEjemplos()
    {
        var datos = new FakeDatos();

        datos.Profesiones.AddRange(
        [
            new Profesion { Id = 1, Nom = "Ingeniería de Sistemas", Des = "Diseño y construcción de software" },
            new Profesion { Id = 2, Nom = "Medicina", Des = "Ciencias de la salud" },
            new Profesion { Id = 3, Nom = "Derecho" }
        ]);

        datos.Personas.AddRange(
        [
            new Persona { Cc = 1012345678, Nombre = "Ana", Apellido = "Pérez", Genero = "F", Edad = 28 },
            new Persona { Cc = 79123456, Nombre = "Carlos", Apellido = "Gómez", Genero = "M", Edad = 45 },
            new Persona { Cc = 52987654, Nombre = "Luisa", Apellido = "Rodríguez", Genero = "F" }
        ]);

        datos.AgregarEstudio(new Estudio { IdProf = 1, CcPer = 1012345678, Univer = "Pontificia Universidad Javeriana", Fecha = new DateOnly(2020, 6, 15) });
        datos.AgregarEstudio(new Estudio { IdProf = 2, CcPer = 79123456, Univer = "Universidad Nacional de Colombia", Fecha = new DateOnly(2005, 12, 10) });
        datos.AgregarEstudio(new Estudio { IdProf = 3, CcPer = 1012345678, Univer = "Universidad del Rosario" });

        datos.AgregarTelefono(new Telefono { Num = "3001234567", Oper = "Claro", Duenio = 1012345678 });
        datos.AgregarTelefono(new Telefono { Num = "3109876543", Oper = "Movistar", Duenio = 79123456 });
        datos.AgregarTelefono(new Telefono { Num = "3155551234", Oper = "Tigo", Duenio = 1012345678 });

        return datos;
    }

    internal void VerificarQuePuedeGuardar()
    {
        if (FallarAlGuardar)
        {
            throw new DbUpdateException("Error simulado al guardar en la base.");
        }
    }

    internal void AgregarEstudio(Estudio estudio)
    {
        var persona = Personas.FirstOrDefault(p => p.Cc == estudio.CcPer)
            ?? throw new DbUpdateException($"FK: no existe la persona {estudio.CcPer}.");
        var profesion = Profesiones.FirstOrDefault(p => p.Id == estudio.IdProf)
            ?? throw new DbUpdateException($"FK: no existe la profesión {estudio.IdProf}.");

        estudio.CcPerNavigation = persona;
        estudio.IdProfNavigation = profesion;
        persona.Estudios.Add(estudio);
        profesion.Estudios.Add(estudio);
        Estudios.Add(estudio);
    }

    internal void AgregarTelefono(Telefono telefono)
    {
        var duenio = Personas.FirstOrDefault(p => p.Cc == telefono.Duenio)
            ?? throw new DbUpdateException($"FK: no existe la persona {telefono.Duenio}.");

        telefono.DuenioNavigation = duenio;
        duenio.Telefonos.Add(telefono);
        Telefonos.Add(telefono);
    }

    internal void QuitarEstudio(Estudio estudio)
    {
        estudio.CcPerNavigation?.Estudios.Remove(estudio);
        estudio.IdProfNavigation?.Estudios.Remove(estudio);
        Estudios.Remove(estudio);
    }

    internal void QuitarTelefono(Telefono telefono)
    {
        telefono.DuenioNavigation?.Telefonos.Remove(telefono);
        Telefonos.Remove(telefono);
    }
}
