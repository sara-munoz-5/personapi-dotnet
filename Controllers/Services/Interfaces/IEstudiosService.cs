// PROPUESTA del integrante 3 según contratos §4B; pendiente de aprobación del integrante 2
using personapi_dotnet.Models.DTOs;

namespace personapi_dotnet.Controllers.Services.Interfaces
{
    /// <summary>Reglas de negocio de estudios; la llave es compuesta (idProf, ccPer).</summary>
    public interface IEstudiosService
    {
        Task<List<EstudiosDto>> ObtenerTodosAsync();

        /// <returns>El estudio, o null si no existe.</returns>
        Task<EstudiosDto?> ObtenerPorIdAsync(int idProf, int ccPer);

        /// <exception cref="ConflictoNegocioException">
        /// LlaveDuplicada si la combinación ya existe; ReferenciaInexistente si la persona o la profesión no existen.
        /// </exception>
        Task<EstudiosDto> CrearAsync(EstudiosCrearDto dto);

        /// <returns>false si el estudio no existe. Solo cambia fecha y universidad.</returns>
        Task<bool> ActualizarAsync(int idProf, int ccPer, EstudiosActualizarDto dto);

        /// <returns>false si el estudio no existe.</returns>
        Task<bool> EliminarAsync(int idProf, int ccPer);
    }
}
