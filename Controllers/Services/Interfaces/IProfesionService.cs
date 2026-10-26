// PROPUESTA del integrante 3 según contratos §4B; pendiente de aprobación del integrante 2
using personapi_dotnet.Models.DTOs;

namespace personapi_dotnet.Controllers.Services.Interfaces
{
    /// <summary>Reglas de negocio de profesiones; lo usan los controladores de vista y los API Controller.</summary>
    public interface IProfesionService
    {
        Task<List<ProfesionDto>> ObtenerTodosAsync();

        /// <returns>La profesión, o null si no existe.</returns>
        Task<ProfesionDto?> ObtenerPorIdAsync(int id);

        /// <exception cref="ConflictoNegocioException">LlaveDuplicada si el id ya existe.</exception>
        Task<ProfesionDto> CrearAsync(ProfesionCrearDto dto);

        /// <returns>false si la profesión no existe.</returns>
        Task<bool> ActualizarAsync(int id, ProfesionActualizarDto dto);

        /// <returns>false si la profesión no existe.</returns>
        /// <exception cref="ConflictoNegocioException">BorradoConReferencias si tiene estudios.</exception>
        Task<bool> EliminarAsync(int id);
    }
}
