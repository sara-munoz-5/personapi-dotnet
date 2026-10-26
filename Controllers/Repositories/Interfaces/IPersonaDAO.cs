using personapi_dotnet.Models.EntitiesGeneradas;

namespace personapi_dotnet.Controllers.Repositories.Interfaces
{
    public interface IPersonaDAO
    {
        Task<List<Persona>> ObtenerTodos();
        Task<Persona?> ObtenerPorId(int cc);
        Task<Persona> Crear(Persona persona);
        Task<bool> Actualizar(Persona persona);
        Task<bool> Eliminar(int cc);
    }
}