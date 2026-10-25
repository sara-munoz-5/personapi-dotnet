using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Interfaces;
using personapi_dotnet.Models.Entities;

namespace PersonApi.Tests.Fakes;

/// <summary>Repositorio de teléfonos en memoria; cada teléfono trae su dueño cargado.</summary>
public class FakeTelefonoRepository : ITelefonoRepository
{
    private readonly FakeDatos _datos;

    public FakeTelefonoRepository(FakeDatos datos)
    {
        _datos = datos;
    }

    public Task<List<Telefono>> GetAllAsync() =>
        Task.FromResult(_datos.Telefonos.ToList());

    public Task<Telefono?> GetByIdAsync(string num) =>
        Task.FromResult(_datos.Telefonos.FirstOrDefault(t => t.Num == num));

    public Task AddAsync(Telefono telefono)
    {
        _datos.VerificarQuePuedeGuardar();
        if (_datos.Telefonos.Any(t => t.Num == telefono.Num))
        {
            throw new DbUpdateException($"PK: el teléfono {telefono.Num} ya existe.");
        }

        _datos.AgregarTelefono(telefono);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Telefono telefono)
    {
        _datos.VerificarQuePuedeGuardar();
        var actual = _datos.Telefonos.FirstOrDefault(t => t.Num == telefono.Num)
            ?? throw new DbUpdateConcurrencyException($"No existe el teléfono {telefono.Num}.");

        if (_datos.Personas.All(p => p.Cc != telefono.Duenio))
        {
            throw new DbUpdateException($"FK: no existe la persona {telefono.Duenio}.");
        }

        // Si cambia el dueño, se mueve el teléfono a la colección de la nueva persona
        _datos.QuitarTelefono(actual);
        actual.Oper = telefono.Oper;
        actual.Duenio = telefono.Duenio;
        _datos.AgregarTelefono(actual);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string num)
    {
        _datos.VerificarQuePuedeGuardar();
        var telefono = _datos.Telefonos.FirstOrDefault(t => t.Num == num)
            ?? throw new DbUpdateConcurrencyException($"No existe el teléfono {num}.");

        _datos.QuitarTelefono(telefono);
        return Task.CompletedTask;
    }
}
