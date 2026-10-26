using personapi_dotnet.Models.EntitiesGeneradas;

namespace personapi_dotnet.Controllers.Repositories.Interfaces
{
    public interface IEstudiosDAO
    {
        Task<List<Estudio>> ObtenerTodos();
        Task<Estudio?> ObtenerPorId(int idProf, int ccPer);
        Task<Estudio> Crear(Estudio estudio);
        Task<bool> Actualizar(Estudio estudio);
        Task<bool> Eliminar(int idProf, int ccPer);
    }
}