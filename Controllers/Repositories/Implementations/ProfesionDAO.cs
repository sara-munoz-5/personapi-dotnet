using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Models.EntitiesGeneradas;
using personapi_dotnet.Controllers.Repositories.Interfaces;

namespace personapi_dotnet.Controllers.Repositories.Implementations
{
    public class ProfesionDAO : IProfesionDAO
    {
        private readonly PersonaDbContext _context;

        public ProfesionDAO(PersonaDbContext context)
        {
            _context = context;
        }

        public async Task<List<Profesion>> ObtenerTodosAsync()
        {
            return await _context.Profesions
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Profesion?> ObtenerPorIdAsync(int id)
        {
            return await _context.Profesions
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Profesion> CrearAsync(Profesion profesion)
        {
            _context.Profesions.Add(profesion);
            await _context.SaveChangesAsync();
            return profesion;
        }

        public async Task<bool> ActualizarAsync(Profesion profesion)
        {
            var existente = await _context.Profesions
                .FirstOrDefaultAsync(p => p.Id == profesion.Id);

            if (existente == null)
                return false;

            existente.Nom = profesion.Nom;
            existente.Des = profesion.Des;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var profesion = await _context.Profesions
                .FirstOrDefaultAsync(p => p.Id == id);

            if (profesion == null)
                return false;

            _context.Profesions.Remove(profesion);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}