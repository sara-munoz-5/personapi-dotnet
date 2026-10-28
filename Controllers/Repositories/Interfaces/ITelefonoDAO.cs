using personapi_dotnet.Models.EntitiesGeneradas;

namespace personapi_dotnet.Controllers.Repositories.Interfaces
{
    public interface ITelefonoDAO
    {
        Task<List<Telefono>> ObtenerTodosAsync();
        Task<Telefono?> ObtenerPorIdAsync(string num);
        Task<Telefono> CrearAsync(Telefono telefono);
        Task<bool> ActualizarAsync(Telefono telefono);
        Task<bool> EliminarAsync(string num);
    }
}