using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Interfaces;
using personapi_dotnet.Models.Entities;

namespace PersonApi.Tests.Fakes;

/// <summary>Repositorio de profesiones en memoria; GetByIdAsync trae sus estudios, como el real.</summary>
public class FakeProfesionRepository : IProfesionRepository
{
    private readonly FakeDatos _datos;

    public FakeProfesionRepository(FakeDatos datos)
    {
        _datos = datos;
    }

    public Task<List<Profesion>> GetAllAsync() =>
        Task.FromResult(_datos.Profesiones.ToList());

    public Task<Profesion?> GetByIdAsync(int id) =>
        Task.FromResult(_datos.Profesiones.FirstOrDefault(p => p.Id == id));

    public Task AddAsync(Profesion profesion)
    {
        _datos.VerificarQuePuedeGuardar();
        if (_datos.Profesiones.Any(p => p.Id == profesion.Id))
        {
            throw new DbUpdateException($"PK: la profesión {profesion.Id} ya existe.");
        }

        _datos.Profesiones.Add(profesion);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Profesion profesion)
    {
        _datos.VerificarQuePuedeGuardar();
        var actual = _datos.Profesiones.FirstOrDefault(p => p.Id == profesion.Id)
            ?? throw new DbUpdateConcurrencyException($"No existe la profesión {profesion.Id}.");

        actual.Nom = profesion.Nom;
        actual.Des = profesion.Des;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        _datos.VerificarQuePuedeGuardar();
        var profesion = _datos.Profesiones.FirstOrDefault(p => p.Id == id)
            ?? throw new DbUpdateConcurrencyException($"No existe la profesión {id}.");

        // ON DELETE CASCADE: se van los estudios que la usan
        foreach (var estudio in profesion.Estudios.ToList())
        {
            _datos.QuitarEstudio(estudio);
        }

        _datos.Profesiones.Remove(profesion);
        return Task.CompletedTask;
    }
}
