using personapi_dotnet.Models.EntitiesGeneradas;

namespace personapi_dotnet.Controllers.Repositories.Interfaces
{
    public interface IEstudiosDAO
    {
        Task<List<Estudio>> ObtenerTodosAsync();
        Task<Estudio?> ObtenerPorIdAsync(int idProf, int ccPer);
        Task<Estudio> CrearAsync(Estudio estudio);
        Task<bool> ActualizarAsync(Estudio estudio);
        Task<bool> EliminarAsync(int idProf, int ccPer);
    }
}