using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.Interfaces;

/// <summary>
/// Contrato DAO de la tabla telefono (sección 5.3). Lo usan TelefonoController y TelefonoApiController.
/// </summary>
public interface ITelefonoRepository
{
    /// <summary>
    /// Lista todos los teléfonos. Incluye el dueño (DuenioNavigation) de cada uno,
    /// para mostrar su nombre en lugar de la cédula.
    /// </summary>
    Task<List<Telefono>> GetAllAsync();

    /// <summary>Busca un teléfono por su número, con su dueño.</summary>
    /// <returns>El teléfono, o <c>null</c> si no existe.</returns>
    Task<Telefono?> GetByIdAsync(string num);

    /// <summary>Crea un teléfono. Lanza una excepción si el número ya existe.</summary>
    Task AddAsync(Telefono telefono);

    /// <summary>Actualiza un teléfono existente.</summary>
    Task UpdateAsync(Telefono telefono);

    /// <summary>Elimina un teléfono por su número.</summary>
    Task DeleteAsync(string num);
}
