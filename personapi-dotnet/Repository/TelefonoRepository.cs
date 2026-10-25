using personapi_dotnet.Interfaces;
using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.Repository;

/// <summary>
/// Implementación DAO de <see cref="ITelefonoRepository"/> sobre <see cref="PersonaDbContext"/>.
/// PENDIENTE: la implementa el integrante 1.
/// </summary>
public class TelefonoRepository : ITelefonoRepository
{
    private readonly PersonaDbContext _context;

    public TelefonoRepository(PersonaDbContext context)
    {
        _context = context;
    }

    public Task<List<Telefono>> GetAllAsync() =>
        throw new NotImplementedException("Pendiente: integrante 1");

    public Task<Telefono?> GetByIdAsync(string num) =>
        throw new NotImplementedException("Pendiente: integrante 1");

    public Task AddAsync(Telefono telefono) =>
        throw new NotImplementedException("Pendiente: integrante 1");

    public Task UpdateAsync(Telefono telefono) =>
        throw new NotImplementedException("Pendiente: integrante 1");

    public Task DeleteAsync(string num) =>
        throw new NotImplementedException("Pendiente: integrante 1");
}
