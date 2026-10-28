using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Models.DTOs;

namespace personapi_dotnet.Controllers.ApiControllers;

[ApiController]
[Route("api/telefonos")]
[Produces("application/json")]
public class TelefonosApiController : ApiControllerBase
{
    private readonly ITelefonoService _telefonos;

    public TelefonosApiController(ITelefonoService telefonos) => _telefonos = telefonos;

    [HttpGet]
    public async Task<ActionResult<List<TelefonoDto>>> ObtenerTodos() => Ok(await _telefonos.ObtenerTodosAsync());

    [HttpGet("{num}")]
    public async Task<ActionResult<TelefonoDto>> ObtenerPorId(string num)
    {
        var telefono = await _telefonos.ObtenerPorIdAsync(num);
        return telefono == null ? RecursoNoEncontrado($"No existe el teléfono {num}.") : Ok(telefono);
    }

    [HttpPost]
    public async Task<ActionResult<TelefonoDto>> Crear(TelefonoCrearDto dto)
    {
        try
        {
            var telefono = await _telefonos.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { num = telefono.Num }, telefono);
        }
        catch (ConflictoNegocioException ex)
        {
            return Conflicto(ex);
        }
    }

    [HttpPut("{num}")]
    public async Task<IActionResult> Actualizar(string num, TelefonoActualizarDto dto)
    {
        try
        {
            if (!await _telefonos.ActualizarAsync(num, dto))
                return RecursoNoEncontrado($"No existe el teléfono {num}.");
            return NoContent();
        }
        catch (ConflictoNegocioException ex) when (ex.Tipo == TipoConflicto.ReferenciaInexistente)
        {
            return Conflicto(ex);
        }
    }

    [HttpDelete("{num}")]
    public async Task<IActionResult> Eliminar(string num)
    {
        if (!await _telefonos.EliminarAsync(num))
            return RecursoNoEncontrado($"No existe el teléfono {num}.");
        return NoContent();
    }
}
