using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Controllers.Repositories.Interfaces;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Models.DTOs;
using personapi_dotnet.Models.EntitiesGeneradas;

namespace personapi_dotnet.Controllers.Services.Implementations;

public class EstudiosService : IEstudiosService
{
    private readonly IEstudiosDAO _estudios;
    private readonly IPersonaDAO _personas;
    private readonly IProfesionDAO _profesiones;

    public EstudiosService(IEstudiosDAO estudios, IPersonaDAO personas, IProfesionDAO profesiones)
    {
        _estudios = estudios;
        _personas = personas;
        _profesiones = profesiones;
    }

    public async Task<List<EstudiosDto>> ObtenerTodosAsync() =>
        (await _estudios.ObtenerTodosAsync()).Select(Mapear).ToList();

    public async Task<EstudiosDto?> ObtenerPorIdAsync(int idProf, int ccPer)
    {
        var estudio = await _estudios.ObtenerPorIdAsync(idProf, ccPer);
        return estudio == null ? null : Mapear(estudio);
    }

    public async Task<EstudiosDto> CrearAsync(EstudiosCrearDto dto)
    {
        if (await _estudios.ObtenerPorIdAsync(dto.IdProf, dto.CcPer) != null)
            throw new ConflictoNegocioException(TipoConflicto.LlaveDuplicada,
                "La combinación de persona y profesión ya está registrada.");

        if (await _personas.ObtenerPorIdAsync(dto.CcPer) == null ||
            await _profesiones.ObtenerPorIdAsync(dto.IdProf) == null)
            throw new ConflictoNegocioException(TipoConflicto.ReferenciaInexistente,
                "La persona o la profesión no existe.");

        try
        {
            var estudio = await _estudios.CrearAsync(new Estudio
            {
                IdProf = dto.IdProf, CcPer = dto.CcPer, Fecha = dto.Fecha, Univer = TextoOpcional(dto.Univer)
            });
            return Mapear(estudio);
        }
        catch (DbUpdateException ex)
        {
            // Otra petición guardó la misma combinación o borró la persona o la profesión.
            if (await _estudios.ObtenerPorIdAsync(dto.IdProf, dto.CcPer) != null)
                throw new ConflictoNegocioException(TipoConflicto.LlaveDuplicada,
                    "La combinación de persona y profesión ya está registrada.", ex);
            throw new ConflictoNegocioException(TipoConflicto.ReferenciaInexistente,
                "La persona o la profesión no existe.", ex);
        }
    }

    public Task<bool> ActualizarAsync(int idProf, int ccPer, EstudiosActualizarDto dto) =>
        _estudios.ActualizarAsync(new Estudio
        {
            IdProf = idProf, CcPer = ccPer, Fecha = dto.Fecha, Univer = TextoOpcional(dto.Univer)
        });

    public Task<bool> EliminarAsync(int idProf, int ccPer) => _estudios.EliminarAsync(idProf, ccPer);

    /// <summary>Texto opcional: vacío o solo espacios se guarda como NULL.</summary>
    private static string? TextoOpcional(string? texto) => string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();

    private static EstudiosDto Mapear(Estudio estudio) => new()
    {
        IdProf = estudio.IdProf, CcPer = estudio.CcPer, Fecha = estudio.Fecha, Univer = estudio.Univer
    };
}
