using personapi_dotnet.Models.EntitiesGeneradas;

namespace personapi_dotnet.Controllers.Repositories.Interfaces
{
    public interface IProfesionDAO
    {
        Task<List<Profesion>> ObtenerTodosAsync();
        Task<Profesion?> ObtenerPorIdAsync(int id);
        Task<Profesion> CrearAsync(Profesion profesion);
        Task<bool> ActualizarAsync(Profesion profesion);
        Task<bool> EliminarAsync(int id);
    }
}