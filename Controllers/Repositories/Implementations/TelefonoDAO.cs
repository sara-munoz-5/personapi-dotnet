using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Models.EntitiesGeneradas;
using personapi_dotnet.Controllers.Repositories.Interfaces;

namespace personapi_dotnet.Controllers.Repositories.Implementations
{
    public class TelefonoDAO : ITelefonoDAO
    {
        private readonly PersonaDbContext _context;

        public TelefonoDAO(PersonaDbContext context)
        {
            _context = context;
        }

        public async Task<List<Telefono>> ObtenerTodosAsync()
        {
            return await _context.Telefonos
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Telefono?> ObtenerPorIdAsync(string num)
        {
            return await _context.Telefonos
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Num == num);
        }

        public async Task<Telefono> CrearAsync(Telefono telefono)
        {
            _context.Telefonos.Add(telefono);
            await _context.SaveChangesAsync();
            return telefono;
        }

        public async Task<bool> ActualizarAsync(Telefono telefono)
        {
            var existente = await _context.Telefonos
                .FirstOrDefaultAsync(t => t.Num == telefono.Num);

            if (existente == null)
                return false;

            existente.Oper = telefono.Oper;
            existente.Duenio = telefono.Duenio;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(string num)
        {
            var telefono = await _context.Telefonos
                .FirstOrDefaultAsync(t => t.Num == num);

            if (telefono == null)
                return false;

            _context.Telefonos.Remove(telefono);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}