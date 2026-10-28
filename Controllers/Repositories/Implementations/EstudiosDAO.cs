using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Models.EntitiesGeneradas;
using personapi_dotnet.Controllers.Repositories.Interfaces;

namespace personapi_dotnet.Controllers.Repositories.Implementations
{
    public class EstudiosDAO : IEstudiosDAO
    {
        private readonly PersonaDbContext _context;

        public EstudiosDAO(PersonaDbContext context)
        {
            _context = context;
        }

        public async Task<List<Estudio>> ObtenerTodosAsync()
        {
            return await _context.Estudios
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Estudio?> ObtenerPorIdAsync(int idProf, int ccPer)
        {
            return await _context.Estudios
                .AsNoTracking()
                .FirstOrDefaultAsync(e =>
                    e.IdProf == idProf && e.CcPer == ccPer);
        }

        public async Task<Estudio> CrearAsync(Estudio estudio)
        {
            _context.Estudios.Add(estudio);
            await _context.SaveChangesAsync();
            return estudio;
        }

        public async Task<bool> ActualizarAsync(Estudio estudio)
        {
            var existente = await _context.Estudios
                .FirstOrDefaultAsync(e =>
                    e.IdProf == estudio.IdProf &&
                    e.CcPer == estudio.CcPer);

            if (existente == null)
                return false;

            existente.Fecha = estudio.Fecha;
            existente.Univer = estudio.Univer;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(int idProf, int ccPer)
        {
            var estudio = await _context.Estudios
                .FirstOrDefaultAsync(e =>
                    e.IdProf == idProf && e.CcPer == ccPer);

            if (estudio == null)
                return false;

            _context.Estudios.Remove(estudio);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}