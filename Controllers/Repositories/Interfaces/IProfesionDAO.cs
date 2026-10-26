using personapi_dotnet.Models.EntitiesGeneradas;

namespace personapi_dotnet.Controllers.Repositories.Interfaces
{
    public interface IProfesionDAO
    {
        Task<List<Profesion>> ObtenerTodos();
        Task<Profesion?> ObtenerPorId(int id);
        Task<Profesion> Crear(Profesion profesion);
        Task<bool> Actualizar(Profesion profesion);
        Task<bool> Eliminar(int id);
    }
}