using personapi_dotnet.Interfaces;
using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.Repository;

/// <summary>
/// Implementación DAO de <see cref="IProfesionRepository"/> sobre <see cref="PersonaDbContext"/>.
/// PENDIENTE: la implementa el integrante 1.
/// </summary>
public class ProfesionRepository : IProfesionRepository
{
    private readonly PersonaDbContext _context;

    public ProfesionRepository(PersonaDbContext context)
    {
        _context = context;
    }

    public Task<List<Profesion>> GetAllAsync() =>
        throw new NotImplementedException("Pendiente: integrante 1");

    public Task<Profesion?> GetByIdAsync(int id) =>
        throw new NotImplementedException("Pendiente: integrante 1");

    public Task AddAsync(Profesion profesion) =>
        throw new NotImplementedException("Pendiente: integrante 1");

    public Task UpdateAsync(Profesion profesion) =>
        throw new NotImplementedException("Pendiente: integrante 1");

    public Task DeleteAsync(int id) =>
        throw new NotImplementedException("Pendiente: integrante 1");
}
