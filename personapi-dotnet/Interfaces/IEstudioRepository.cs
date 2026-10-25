using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.Interfaces;

/// <summary>
/// Contrato DAO de la tabla estudios (sección 5.3). La llave es compuesta: (idProf, ccPer).
/// Lo usan EstudioController y EstudioApiController.
/// </summary>
public interface IEstudioRepository
{
    /// <summary>
    /// Lista todos los estudios. Incluye la persona (CcPerNavigation) y la profesión
    /// (IdProfNavigation) de cada uno, para mostrar nombres en lugar de números.
    /// </summary>
    Task<List<Estudio>> GetAllAsync();

    /// <summary>Busca un estudio por su llave compuesta, con su persona y su profesión.</summary>
    /// <returns>El estudio, o <c>null</c> si no existe.</returns>
    Task<Estudio?> GetByIdAsync(int idProf, int ccPer);

    /// <summary>Crea un estudio. Lanza una excepción si la pareja (idProf, ccPer) ya existe.</summary>
    Task AddAsync(Estudio estudio);

    /// <summary>Actualiza la fecha y la universidad de un estudio existente.</summary>
    Task UpdateAsync(Estudio estudio);

    /// <summary>Elimina un estudio por su llave compuesta.</summary>
    Task DeleteAsync(int idProf, int ccPer);
}
