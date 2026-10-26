using personapi_dotnet.Models.EntitiesGeneradas;

namespace personapi_dotnet.Controllers.Repositories.Interfaces
{
    public interface ITelefonoDAO
    {
        Task<List<Telefono>> ObtenerTodos();
        Task<Telefono?> ObtenerPorId(string num);
        Task<Telefono> Crear(Telefono telefono);
        Task<bool> Actualizar(Telefono telefono);
        Task<bool> Eliminar(string num);
    }
}