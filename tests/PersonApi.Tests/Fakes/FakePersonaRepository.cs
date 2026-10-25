using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Interfaces;
using personapi_dotnet.Models.Entities;

namespace PersonApi.Tests.Fakes;

/// <summary>Repositorio de personas en memoria; GetByIdAsync trae estudios y teléfonos, como el real.</summary>
public class FakePersonaRepository : IPersonaRepository
{
    private readonly FakeDatos _datos;

    public FakePersonaRepository(FakeDatos datos)
    {
        _datos = datos;
    }

    public Task<List<Persona>> GetAllAsync() =>
        Task.FromResult(_datos.Personas.ToList());

    public Task<Persona?> GetByIdAsync(int cc) =>
        Task.FromResult(_datos.Personas.FirstOrDefault(p => p.Cc == cc));

    public Task AddAsync(Persona persona)
    {
        _datos.VerificarQuePuedeGuardar();
        if (_datos.Personas.Any(p => p.Cc == persona.Cc))
        {
            throw new DbUpdateException($"PK: la cédula {persona.Cc} ya existe.");
        }

        _datos.Personas.Add(persona);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Persona persona)
    {
        _datos.VerificarQuePuedeGuardar();
        var actual = _datos.Personas.FirstOrDefault(p => p.Cc == persona.Cc)
            ?? throw new DbUpdateConcurrencyException($"No existe la persona {persona.Cc}.");

        actual.Nombre = persona.Nombre;
        actual.Apellido = persona.Apellido;
        actual.Genero = persona.Genero;
        actual.Edad = persona.Edad;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int cc)
    {
        _datos.VerificarQuePuedeGuardar();
        var persona = _datos.Personas.FirstOrDefault(p => p.Cc == cc)
            ?? throw new DbUpdateConcurrencyException($"No existe la persona {cc}.");

        // ON DELETE CASCADE: se van sus estudios y teléfonos
        foreach (var estudio in persona.Estudios.ToList())
        {
            _datos.QuitarEstudio(estudio);
        }
        foreach (var telefono in persona.Telefonos.ToList())
        {
            _datos.QuitarTelefono(telefono);
        }

        _datos.Personas.Remove(persona);
        return Task.CompletedTask;
    }
}
