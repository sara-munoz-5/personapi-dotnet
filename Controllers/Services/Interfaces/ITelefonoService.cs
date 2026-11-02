// PROPUESTA del integrante 3 según contratos §4B; pendiente de aprobación del integrante 2
using personapi_dotnet.Models.DTOs;

namespace personapi_dotnet.Controllers.Services.Interfaces
{
    /// <summary>Reglas de negocio de teléfonos; lo usan los controladores de vista y los API Controller.</summary>
    public interface ITelefonoService
    {
        Task<List<TelefonoDto>> ObtenerTodosAsync();

        /// <returns>El teléfono, o null si no existe.</returns>
        Task<TelefonoDto?> ObtenerPorIdAsync(string num);

        /// <exception cref="ConflictoNegocioException">
        /// LlaveDuplicada si el número ya existe; ReferenciaInexistente si el dueño no existe.
        /// </exception>
        Task<TelefonoDto> CrearAsync(TelefonoCrearDto dto);

        /// <returns>false si el teléfono no existe. No cambia el número.</returns>
        /// <exception cref="ConflictoNegocioException">ReferenciaInexistente si el nuevo dueño no existe.</exception>
        Task<bool> ActualizarAsync(string num, TelefonoActualizarDto dto);

        /// <returns>false si el teléfono no existe.</returns>
        Task<bool> EliminarAsync(string num);
    }
}
