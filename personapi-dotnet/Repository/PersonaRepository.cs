using personapi_dotnet.Interfaces;
using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.Repository;

/// <summary>
/// Implementación DAO de <see cref="IPersonaRepository"/> sobre <see cref="PersonaDbContext"/>.
/// PENDIENTE: la implementa el integrante 1.
/// </summary>
public class PersonaRepository : IPersonaRepository
{
    private readonly PersonaDbContext _context;

    public PersonaRepository(PersonaDbContext context)
    {
        _context = context;
    }

    public Task<List<Persona>> GetAllAsync() =>
        throw new NotImplementedException("Pendiente: integrante 1");

    public Task<Persona?> GetByIdAsync(int cc) =>
        throw new NotImplementedException("Pendiente: integrante 1");

    public Task AddAsync(Persona persona) =>
        throw new NotImplementedException("Pendiente: integrante 1");

    public Task UpdateAsync(Persona persona) =>
        throw new NotImplementedException("Pendiente: integrante 1");

    public Task DeleteAsync(int cc) =>
        throw new NotImplementedException("Pendiente: integrante 1");
}
