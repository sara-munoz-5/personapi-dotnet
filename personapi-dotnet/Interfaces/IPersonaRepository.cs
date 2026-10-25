using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.Interfaces;

/// <summary>
/// Contrato DAO de la tabla persona (sección 5.3). Lo usan PersonaController y PersonaApiController.
/// </summary>
public interface IPersonaRepository
{
    /// <summary>Lista todas las personas.</summary>
    Task<List<Persona>> GetAllAsync();

    /// <summary>
    /// Busca una persona por su cédula. Incluye sus estudios (con la profesión de cada uno)
    /// y sus teléfonos, para la vista Details y el aviso de dependientes en Delete.
    /// </summary>
    /// <returns>La persona, o <c>null</c> si no existe.</returns>
    Task<Persona?> GetByIdAsync(int cc);

    /// <summary>Crea una persona. Lanza una excepción si la cédula ya existe.</summary>
    Task AddAsync(Persona persona);

    /// <summary>Actualiza una persona existente.</summary>
    Task UpdateAsync(Persona persona);

    /// <summary>Elimina una persona; la base borra en cascada sus estudios y teléfonos.</summary>
    Task DeleteAsync(int cc);
}
