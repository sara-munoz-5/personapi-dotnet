using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Models.EntitiesGeneradas;
using personapi_dotnet.Controllers.Repositories.Interfaces;

namespace personapi_dotnet.Controllers.Repositories.Implementations
{
    public class PersonaDAO : IPersonaDAO
    {
        private readonly PersonaDbContext _context;

        public PersonaDAO(PersonaDbContext context)
        {
            _context = context;
        }

        public async Task<List<Persona>> ObtenerTodos()
        {
            return await _context.Personas
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Persona?> ObtenerPorId(int cc)
        {
            return await _context.Personas
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Cc == cc);
        }

        public async Task<Persona> Crear(Persona persona)
        {
            _context.Personas.Add(persona);
            await _context.SaveChangesAsync();
            return persona;
        }

        public async Task<bool> Actualizar(Persona persona)
        {
            var existente = await _context.Personas
                .FirstOrDefaultAsync(p => p.Cc == persona.Cc);

            if (existente == null)
                return false;

            existente.Nombre = persona.Nombre;
            existente.Apellido = persona.Apellido;
            existente.Genero = persona.Genero;
            existente.Edad = persona.Edad;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Eliminar(int cc)
        {
            var persona = await _context.Personas
                .FirstOrDefaultAsync(p => p.Cc == cc);

            if (persona == null)
                return false;

            _context.Personas.Remove(persona);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}