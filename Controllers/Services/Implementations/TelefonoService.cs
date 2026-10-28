using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Controllers.Repositories.Interfaces;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Models.DTOs;
using personapi_dotnet.Models.EntitiesGeneradas;

namespace personapi_dotnet.Controllers.Services.Implementations;

public class TelefonoService : ITelefonoService
{
    private readonly ITelefonoDAO _telefonos;
    private readonly IPersonaDAO _personas;

    public TelefonoService(ITelefonoDAO telefonos, IPersonaDAO personas)
    {
        _telefonos = telefonos;
        _personas = personas;
    }

    public async Task<List<TelefonoDto>> ObtenerTodosAsync() =>
        (await _telefonos.ObtenerTodosAsync()).Select(Mapear).ToList();

    public async Task<TelefonoDto?> ObtenerPorIdAsync(string num)
    {
        var telefono = await _telefonos.ObtenerPorIdAsync(num);
        return telefono == null ? null : Mapear(telefono);
    }

    public async Task<TelefonoDto> CrearAsync(TelefonoCrearDto dto)
    {
        if (await _telefonos.ObtenerPorIdAsync(dto.Num) != null)
            throw new ConflictoNegocioException(TipoConflicto.LlaveDuplicada,
                "El número de teléfono ya está registrado.");

        if (await _personas.ObtenerPorIdAsync(dto.Duenio) == null)
            throw new ConflictoNegocioException(TipoConflicto.ReferenciaInexistente,
                "El dueño del teléfono no existe.");

        try
        {
            var telefono = await _telefonos.CrearAsync(new Telefono
            {
                Num = dto.Num.Trim(), Oper = dto.Oper.Trim(), Duenio = dto.Duenio
            });
            return Mapear(telefono);
        }
        catch (DbUpdateException ex)
        {
            throw new ConflictoNegocioException(TipoConflicto.ReferenciaInexistente,
                "El dueño del teléfono no existe.", ex);
        }
    }

    public async Task<bool> ActualizarAsync(string num, TelefonoActualizarDto dto)
    {
        if (await _telefonos.ObtenerPorIdAsync(num) == null)
            return false;

        if (await _personas.ObtenerPorIdAsync(dto.Duenio) == null)
            throw new ConflictoNegocioException(TipoConflicto.ReferenciaInexistente,
                "El dueño del teléfono no existe.");

        return await _telefonos.ActualizarAsync(new Telefono
        {
            Num = num, Oper = dto.Oper.Trim(), Duenio = dto.Duenio
        });
    }

    public Task<bool> EliminarAsync(string num) => _telefonos.EliminarAsync(num);

    private static TelefonoDto Mapear(Telefono telefono) => new()
    {
        Num = telefono.Num, Oper = telefono.Oper ?? string.Empty, Duenio = telefono.Duenio
    };
}
