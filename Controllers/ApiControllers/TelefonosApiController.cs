using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Models.DTOs;

namespace personapi_dotnet.Controllers.ApiControllers;

[ApiController]
[Route("api/telefonos")]
[Produces("application/json")]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
public class TelefonosApiController : ApiControllerBase
{
    private readonly ITelefonoService _telefonos;

    public TelefonosApiController(ITelefonoService telefonos) => _telefonos = telefonos;

    [HttpGet]
    public async Task<ActionResult<List<TelefonoDto>>> ObtenerTodos() => Ok(await _telefonos.ObtenerTodosAsync());

    [HttpGet("{num}")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TelefonoDto>> ObtenerPorId(string num)
    {
        var telefono = await _telefonos.ObtenerPorIdAsync(num);
        return telefono == null ? RecursoNoEncontrado($"No existe el teléfono {num}.") : Ok(telefono);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(string num)
    {
        if (!await _telefonos.EliminarAsync(num))
            return RecursoNoEncontrado($"No existe el teléfono {num}.");
        return NoContent();
    }
}
