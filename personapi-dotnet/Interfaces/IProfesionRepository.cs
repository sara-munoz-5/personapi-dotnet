using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.Interfaces;

/// <summary>
/// Contrato DAO de la tabla profesion (sección 5.3). Lo usan ProfesionController,
/// ProfesionApiController y EstudioController (lista de profesiones).
/// </summary>
public interface IProfesionRepository
{
    /// <summary>Lista todas las profesiones.</summary>
    Task<List<Profesion>> GetAllAsync();

    /// <summary>
    /// Busca una profesión por su id. Incluye sus estudios, para mostrar en Delete
    /// cuántos registros dependientes se borrarán.
    /// </summary>
    /// <returns>La profesión, o <c>null</c> si no existe.</returns>
    Task<Profesion?> GetByIdAsync(int id);

    /// <summary>Crea una profesión. Lanza una excepción si el id ya existe.</summary>
    Task AddAsync(Profesion profesion);

    /// <summary>Actualiza una profesión existente.</summary>
    Task UpdateAsync(Profesion profesion);

    /// <summary>Elimina una profesión; la base borra en cascada sus estudios.</summary>
    Task DeleteAsync(int id);
}
