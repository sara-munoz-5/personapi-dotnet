// PROPUESTA del integrante 3 según contratos §4B; pendiente de aprobación del integrante 2
using personapi_dotnet.Models.DTOs;

namespace personapi_dotnet.Controllers.Services.Interfaces
{
    /// <summary>Reglas de negocio de personas; lo usan los controladores de vista y los API Controller.</summary>
    public interface IPersonaService
    {
        Task<List<PersonaDto>> ObtenerTodosAsync();

        /// <returns>La persona, o null si no existe.</returns>
        Task<PersonaDto?> ObtenerPorIdAsync(int cc);

        /// <exception cref="ConflictoNegocioException">LlaveDuplicada si la cédula ya existe.</exception>
        Task<PersonaDto> CrearAsync(PersonaCrearDto dto);

        /// <returns>false si la persona no existe.</returns>
        Task<bool> ActualizarAsync(int cc, PersonaActualizarDto dto);

        /// <returns>false si la persona no existe.</returns>
        /// <exception cref="ConflictoNegocioException">BorradoConReferencias si tiene estudios o teléfonos.</exception>
        Task<bool> EliminarAsync(int cc);
    }
}
