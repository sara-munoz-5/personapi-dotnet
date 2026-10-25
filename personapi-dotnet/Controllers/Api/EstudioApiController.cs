using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Interfaces;
using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.Controllers.Api;

/// <summary>
/// Servicios REST del CRUD de estudios. La llave es compuesta: /api/estudios/{idProf}/{ccPer}.
/// </summary>
[ApiController]
[Route("api/estudios")]
[Produces("application/json")]
public class EstudioApiController : ControllerBase
{
    private readonly IEstudioRepository _estudios;

    public EstudioApiController(IEstudioRepository estudios)
    {
        _estudios = estudios;
    }

    /// <summary>Lista todos los estudios con su persona y su profesión.</summary>
    /// <response code="200">Lista de estudios.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<Estudio>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Estudio>>> GetAll()
    {
        return Ok(await _estudios.GetAllAsync());
    }

    /// <summary>Consulta un estudio por su llave compuesta.</summary>
    /// <param name="idProf">Id de la profesión.</param>
    /// <param name="ccPer">Cédula de la persona.</param>
    /// <response code="200">El estudio encontrado.</response>
    /// <response code="404">No existe un estudio con esa pareja de llaves.</response>
    [HttpGet("{idProf:int}/{ccPer:int}")]
    [ProducesResponseType(typeof(Estudio), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Estudio>> GetById(int idProf, int ccPer)
    {
        var estudio = await _estudios.GetByIdAsync(idProf, ccPer);
        if (estudio is null)
        {
            return NotFound();
        }
        return Ok(estudio);
    }

    /// <summary>Crea un estudio: registra que una persona estudió una profesión.</summary>
    /// <param name="estudio">Datos del estudio; idProf y ccPer deben existir.</param>
    /// <response code="201">Estudio creado; el encabezado Location apunta a él.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="409">Esa persona ya tiene registrada esa profesión.</response>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Estudio), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Estudio>> Create(Estudio estudio)
    {
        if (await _estudios.GetByIdAsync(estudio.IdProf, estudio.CcPer) is not null)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "Esta persona ya tiene registrada esa profesión.");
        }

        try
        {
            await _estudios.AddAsync(estudio);
        }
        catch (Exception ex) when (ex is not NotImplementedException)
        {
            // El repositorio lanza error si la llave quedó repetida entre la consulta y el guardado
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "Esta persona ya tiene registrada esa profesión.", detail: ex.Message);
        }

        return CreatedAtAction(nameof(GetById), new { idProf = estudio.IdProf, ccPer = estudio.CcPer }, estudio);
    }

    /// <summary>Actualiza la fecha y la universidad de un estudio.</summary>
    /// <param name="idProf">Id de la profesión.</param>
    /// <param name="ccPer">Cédula de la persona.</param>
    /// <param name="estudio">Datos nuevos; idProf y ccPer deben coincidir con los de la URL.</param>
    /// <response code="204">Estudio actualizado.</response>
    /// <response code="400">La llave de la URL no coincide con la del cuerpo, o datos inválidos.</response>
    /// <response code="404">No existe un estudio con esa pareja de llaves.</response>
    [HttpPut("{idProf:int}/{ccPer:int}")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int idProf, int ccPer, Estudio estudio)
    {
        if (idProf != estudio.IdProf || ccPer != estudio.CcPer)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "La llave de la URL no coincide con la del cuerpo.");
        }
        if (await _estudios.GetByIdAsync(idProf, ccPer) is null)
        {
            return NotFound();
        }

        await _estudios.UpdateAsync(estudio);
        return NoContent();
    }

    /// <summary>Elimina un estudio.</summary>
    /// <param name="idProf">Id de la profesión.</param>
    /// <param name="ccPer">Cédula de la persona.</param>
    /// <response code="204">Estudio eliminado.</response>
    /// <response code="404">No existe un estudio con esa pareja de llaves.</response>
    [HttpDelete("{idProf:int}/{ccPer:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int idProf, int ccPer)
    {
        if (await _estudios.GetByIdAsync(idProf, ccPer) is null)
        {
            return NotFound();
        }

        await _estudios.DeleteAsync(idProf, ccPer);
        return NoContent();
    }
}
