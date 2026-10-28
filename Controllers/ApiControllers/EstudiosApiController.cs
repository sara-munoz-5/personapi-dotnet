using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Models.DTOs;

namespace personapi_dotnet.Controllers.ApiControllers;

[ApiController]
[Route("api/estudios")]
[Produces("application/json")]
public class EstudiosApiController : ControllerBase
{
    private readonly IEstudiosService _estudios;

    public EstudiosApiController(IEstudiosService estudios) => _estudios = estudios;

    [HttpGet]
    public async Task<ActionResult<List<EstudiosDto>>> ObtenerTodos() => Ok(await _estudios.ObtenerTodosAsync());

    [HttpGet("{idProf:int}/{ccPer:int}")]
    public async Task<ActionResult<EstudiosDto>> ObtenerPorId(int idProf, int ccPer)
    {
        var estudio = await _estudios.ObtenerPorIdAsync(idProf, ccPer);
        return estudio == null ? NotFound() : Ok(estudio);
    }

    [HttpPost]
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
            return Conflict(new { mensaje = ex.Message, tipo = ex.Tipo.ToString() });
        }
    }

    [HttpPut("{idProf:int}/{ccPer:int}")]
    public async Task<IActionResult> Actualizar(int idProf, int ccPer, EstudiosActualizarDto dto)
    {
        if (!await _estudios.ActualizarAsync(idProf, ccPer, dto))
            return NotFound();
        return NoContent();
    }

    [HttpDelete("{idProf:int}/{ccPer:int}")]
    public async Task<IActionResult> Eliminar(int idProf, int ccPer)
    {
        if (!await _estudios.EliminarAsync(idProf, ccPer))
            return NotFound();
        return NoContent();
    }
}
