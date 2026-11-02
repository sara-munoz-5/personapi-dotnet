using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Models.DTOs;

namespace personapi_dotnet.Controllers.ApiControllers;

[ApiController]
[Route("api/estudios")]
[Produces("application/json")]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
public class EstudiosApiController : ApiControllerBase
{
    private readonly IEstudiosService _estudios;

    public EstudiosApiController(IEstudiosService estudios) => _estudios = estudios;

    [HttpGet]
    public async Task<ActionResult<List<EstudiosDto>>> ObtenerTodos() => Ok(await _estudios.ObtenerTodosAsync());

    [HttpGet("{idProf:int}/{ccPer:int}")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EstudiosDto>> ObtenerPorId(int idProf, int ccPer)
    {
        var estudio = await _estudios.ObtenerPorIdAsync(idProf, ccPer);
        return estudio == null
            ? RecursoNoEncontrado($"No existe el estudio {idProf}/{ccPer}.")
            : Ok(estudio);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<EstudiosDto>> Crear(EstudiosCrearDto dto)
    {
        try
        {
            var estudio = await _estudios.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId),
                new { idProf = estudio.IdProf, ccPer = estudio.CcPer }, estudio);
        }
        catch (ConflictoNegocioException ex)
        {
            return Conflicto(ex);
        }
    }

    [HttpPut("{idProf:int}/{ccPer:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Actualizar(int idProf, int ccPer, EstudiosActualizarDto dto)
    {
        if (!await _estudios.ActualizarAsync(idProf, ccPer, dto))
            return RecursoNoEncontrado($"No existe el estudio {idProf}/{ccPer}.");
        return NoContent();
    }

    [HttpDelete("{idProf:int}/{ccPer:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(int idProf, int ccPer)
    {
        if (!await _estudios.EliminarAsync(idProf, ccPer))
            return RecursoNoEncontrado($"No existe el estudio {idProf}/{ccPer}.");
        return NoContent();
    }
}
