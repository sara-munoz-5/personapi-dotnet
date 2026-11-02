using personapi_dotnet.Models.EntitiesGeneradas;

namespace personapi_dotnet.Controllers.Repositories.Interfaces
{
    public interface IPersonaDAO
    {
        Task<List<Persona>> ObtenerTodosAsync();
        Task<Persona?> ObtenerPorIdAsync(int cc);
        Task<Persona> CrearAsync(Persona persona);
        Task<bool> ActualizarAsync(Persona persona);
        Task<bool> EliminarAsync(int cc);
    }
}