using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Controllers.Repositories.Interfaces;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Models.DTOs;
using personapi_dotnet.Models.EntitiesGeneradas;

namespace personapi_dotnet.Controllers.Services.Implementations;

public class PersonaService : IPersonaService
{
    private readonly IPersonaDAO _personas;
    private readonly IEstudiosDAO _estudios;
    private readonly ITelefonoDAO _telefonos;

    public PersonaService(IPersonaDAO personas, IEstudiosDAO estudios, ITelefonoDAO telefonos)
    {
        _personas = personas;
        _estudios = estudios;
        _telefonos = telefonos;
    }

    public async Task<List<PersonaDto>> ObtenerTodosAsync() =>
        (await _personas.ObtenerTodos()).Select(Mapear).ToList();

    public async Task<PersonaDto?> ObtenerPorIdAsync(int cc)
    {
        var persona = await _personas.ObtenerPorId(cc);
        return persona == null ? null : Mapear(persona);
    }

    public async Task<PersonaDto> CrearAsync(PersonaCrearDto dto)
    {
        if (await _personas.ObtenerPorId(dto.Cc) != null)
            throw new ConflictoNegocioException(TipoConflicto.LlaveDuplicada, "La cédula ya está registrada.");

        try
        {
            var persona = await _personas.Crear(new Persona
            {
                Cc = dto.Cc, Nombre = dto.Nombre.Trim(), Apellido = dto.Apellido.Trim(),
                Genero = dto.Genero, Edad = dto.Edad
            });
            return Mapear(persona);
        }
        catch (DbUpdateException ex)
        {
            throw new ConflictoNegocioException(TipoConflicto.LlaveDuplicada, "La cédula ya está registrada.", ex);
        }
    }

    public Task<bool> ActualizarAsync(int cc, PersonaActualizarDto dto) =>
        _personas.Actualizar(new Persona
        {
            Cc = cc, Nombre = dto.Nombre.Trim(), Apellido = dto.Apellido.Trim(),
            Genero = dto.Genero, Edad = dto.Edad
        });

    public async Task<bool> EliminarAsync(int cc)
    {
        if (await _personas.ObtenerPorId(cc) == null)
            return false;

        var tieneEstudios = (await _estudios.ObtenerTodos()).Any(e => e.CcPer == cc);
        var tieneTelefonos = (await _telefonos.ObtenerTodos()).Any(t => t.Duenio == cc);
        if (tieneEstudios || tieneTelefonos)
            throw new ConflictoNegocioException(TipoConflicto.BorradoConReferencias,
                "La persona tiene registros relacionados.");

        return await _personas.Eliminar(cc);
    }

    private static PersonaDto Mapear(Persona persona) => new()
    {
        Cc = persona.Cc, Nombre = persona.Nombre, Apellido = persona.Apellido,
        Genero = persona.Genero, Edad = persona.Edad
    };
}
