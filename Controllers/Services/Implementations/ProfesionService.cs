using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Controllers.Repositories.Interfaces;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Models.DTOs;
using personapi_dotnet.Models.EntitiesGeneradas;

namespace personapi_dotnet.Controllers.Services.Implementations;

public class ProfesionService : IProfesionService
{
    private readonly IProfesionDAO _profesiones;
    private readonly IEstudiosDAO _estudios;

    public ProfesionService(IProfesionDAO profesiones, IEstudiosDAO estudios)
    {
        _profesiones = profesiones;
        _estudios = estudios;
    }

    public async Task<List<ProfesionDto>> ObtenerTodosAsync() =>
        (await _profesiones.ObtenerTodosAsync()).Select(Mapear).ToList();

    public async Task<ProfesionDto?> ObtenerPorIdAsync(int id)
    {
        var profesion = await _profesiones.ObtenerPorIdAsync(id);
        return profesion == null ? null : Mapear(profesion);
    }

    public async Task<ProfesionDto> CrearAsync(ProfesionCrearDto dto)
    {
        if (await _profesiones.ObtenerPorIdAsync(dto.Id) != null)
            throw new ConflictoNegocioException(TipoConflicto.LlaveDuplicada, "El id ya está registrado.");

        try
        {
            var profesion = await _profesiones.CrearAsync(new Profesion
            {
                Id = dto.Id, Nom = dto.Nom.Trim(), Des = TextoOpcional(dto.Des)
            });
            return Mapear(profesion);
        }
        catch (DbUpdateException ex)
        {
            throw new ConflictoNegocioException(TipoConflicto.LlaveDuplicada, "El id ya está registrado.", ex);
        }
    }

    public Task<bool> ActualizarAsync(int id, ProfesionActualizarDto dto) =>
        _profesiones.ActualizarAsync(new Profesion
        {
            Id = id, Nom = dto.Nom.Trim(), Des = TextoOpcional(dto.Des)
        });

    public async Task<bool> EliminarAsync(int id)
    {
        if (await _profesiones.ObtenerPorIdAsync(id) == null)
            return false;

        if ((await _estudios.ObtenerTodosAsync()).Any(e => e.IdProf == id))
            throw new ConflictoNegocioException(TipoConflicto.BorradoConReferencias,
                "La profesión tiene estudios relacionados.");

        try
        {
            return await _profesiones.EliminarAsync(id);
        }
        catch (DbUpdateException ex)
        {
            throw new ConflictoNegocioException(TipoConflicto.BorradoConReferencias,
                "La profesión tiene estudios relacionados.", ex);
        }
    }

    /// <summary>Texto opcional: vacío o solo espacios se guarda como NULL.</summary>
    private static string? TextoOpcional(string? texto) => string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();

    private static ProfesionDto Mapear(Profesion profesion) => new()
    {
        Id = profesion.Id, Nom = profesion.Nom, Des = profesion.Des
    };
}
