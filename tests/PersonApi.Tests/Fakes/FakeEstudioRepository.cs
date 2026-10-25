using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Interfaces;
using personapi_dotnet.Models.Entities;

namespace PersonApi.Tests.Fakes;

/// <summary>Repositorio de estudios en memoria con llave compuesta (idProf, ccPer) y relaciones cargadas.</summary>
public class FakeEstudioRepository : IEstudioRepository
{
    private readonly FakeDatos _datos;

    public FakeEstudioRepository(FakeDatos datos)
    {
        _datos = datos;
    }

    public Task<List<Estudio>> GetAllAsync() =>
        Task.FromResult(_datos.Estudios.ToList());

    public Task<Estudio?> GetByIdAsync(int idProf, int ccPer) =>
        Task.FromResult(_datos.Estudios.FirstOrDefault(e => e.IdProf == idProf && e.CcPer == ccPer));

    public Task AddAsync(Estudio estudio)
    {
        _datos.VerificarQuePuedeGuardar();
        if (_datos.Estudios.Any(e => e.IdProf == estudio.IdProf && e.CcPer == estudio.CcPer))
        {
            throw new DbUpdateException($"PK: el estudio ({estudio.IdProf}, {estudio.CcPer}) ya existe.");
        }

        _datos.AgregarEstudio(estudio);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Estudio estudio)
    {
        _datos.VerificarQuePuedeGuardar();
        var actual = _datos.Estudios.FirstOrDefault(e => e.IdProf == estudio.IdProf && e.CcPer == estudio.CcPer)
            ?? throw new DbUpdateConcurrencyException($"No existe el estudio ({estudio.IdProf}, {estudio.CcPer}).");

        actual.Fecha = estudio.Fecha;
        actual.Univer = estudio.Univer;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int idProf, int ccPer)
    {
        _datos.VerificarQuePuedeGuardar();
        var estudio = _datos.Estudios.FirstOrDefault(e => e.IdProf == idProf && e.CcPer == ccPer)
            ?? throw new DbUpdateConcurrencyException($"No existe el estudio ({idProf}, {ccPer}).");

        _datos.QuitarEstudio(estudio);
        return Task.CompletedTask;
    }
}
