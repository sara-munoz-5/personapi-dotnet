using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Models.DTOs;

namespace personapi_dotnet.Controllers.ApiControllers;

[ApiController]
[Route("api/profesiones")]
[Produces("application/json")]
public class ProfesionesApiController : ApiControllerBase
{
    private readonly IProfesionService _profesiones;

    public ProfesionesApiController(IProfesionService profesiones) => _profesiones = profesiones;

    [HttpGet]
    public async Task<ActionResult<List<ProfesionDto>>> ObtenerTodos() => Ok(await _profesiones.ObtenerTodosAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProfesionDto>> ObtenerPorId(int id)
    {
        var profesion = await _profesiones.ObtenerPorIdAsync(id);
        return profesion == null ? RecursoNoEncontrado($"No existe la profesión con id {id}.") : Ok(profesion);
    }

    [HttpPost]
    public async Task<ActionResult<ProfesionDto>> Crear(ProfesionCrearDto dto)
    {
        try
        {
            var profesion = await _profesiones.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = profesion.Id }, profesion);
        }
        catch (ConflictoNegocioException ex) when (ex.Tipo == TipoConflicto.LlaveDuplicada)
        {
            return Conflicto(ex);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, ProfesionActualizarDto dto)
    {
        if (!await _profesiones.ActualizarAsync(id, dto))
            return RecursoNoEncontrado($"No existe la profesión con id {id}.");
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            if (!await _profesiones.EliminarAsync(id))
                return RecursoNoEncontrado($"No existe la profesión con id {id}.");
            return NoContent();
        }
        catch (ConflictoNegocioException ex) when (ex.Tipo == TipoConflicto.BorradoConReferencias)
        {
            return Conflicto(ex);
        }
    }
}
