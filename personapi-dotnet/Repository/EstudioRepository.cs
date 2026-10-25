using personapi_dotnet.Interfaces;
using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.Repository;

/// <summary>
/// Implementación DAO de <see cref="IEstudioRepository"/> sobre <see cref="PersonaDbContext"/>.
/// PENDIENTE: la implementa el integrante 1.
/// </summary>
public class EstudioRepository : IEstudioRepository
{
    private readonly PersonaDbContext _context;

    public EstudioRepository(PersonaDbContext context)
    {
        _context = context;
    }

    public Task<List<Estudio>> GetAllAsync() =>
        throw new NotImplementedException("Pendiente: integrante 1");

    public Task<Estudio?> GetByIdAsync(int idProf, int ccPer) =>
        throw new NotImplementedException("Pendiente: integrante 1");

    public Task AddAsync(Estudio estudio) =>
        throw new NotImplementedException("Pendiente: integrante 1");

    public Task UpdateAsync(Estudio estudio) =>
        throw new NotImplementedException("Pendiente: integrante 1");

    public Task DeleteAsync(int idProf, int ccPer) =>
        throw new NotImplementedException("Pendiente: integrante 1");
}
