using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Interfaces;
using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.Controllers.Api;

/// <summary>
/// Servicios REST del CRUD de profesiones.
/// </summary>
[ApiController]
[Route("api/profesiones")]
[Produces("application/json")]
public class ProfesionApiController : ControllerBase
{
    private readonly IProfesionRepository _profesiones;

    public ProfesionApiController(IProfesionRepository profesiones)
    {
        _profesiones = profesiones;
    }

    /// <summary>Lista todas las profesiones.</summary>
    /// <response code="200">Lista de profesiones.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<Profesion>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Profesion>>> GetAll()
    {
        return Ok(await _profesiones.GetAllAsync());
    }

    /// <summary>Consulta una profesión por su id, con sus estudios.</summary>
    /// <param name="id">Id de la profesión.</param>
    /// <response code="200">La profesión encontrada.</response>
    /// <response code="404">No existe una profesión con ese id.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Profesion), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Profesion>> GetById(int id)
    {
        var profesion = await _profesiones.GetByIdAsync(id);
        if (profesion is null)
        {
            return NotFound();
        }
        return Ok(profesion);
    }

    /// <summary>Crea una profesión.</summary>
    /// <param name="profesion">Datos de la profesión; el id lo digita el usuario.</param>
    /// <response code="201">Profesión creada; el encabezado Location apunta a ella.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="409">Ya existe una profesión con ese id.</response>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Profesion), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Profesion>> Create(Profesion profesion)
    {
        if (await _profesiones.GetByIdAsync(profesion.Id) is not null)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "El id de la profesión ya existe.");
        }

        try
        {
            await _profesiones.AddAsync(profesion);
        }
        catch (DbUpdateException ex)
        {
            // EF Core lanza DbUpdateException si la llave quedó repetida entre la consulta y el guardado
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "El id de la profesión ya existe.", detail: ex.Message);
        }

        return CreatedAtAction(nameof(GetById), new { id = profesion.Id }, profesion);
    }

    /// <summary>Actualiza una profesión existente.</summary>
    /// <param name="id">Id de la profesión a actualizar.</param>
    /// <param name="profesion">Datos nuevos; su id debe coincidir con el de la URL.</param>
    /// <response code="204">Profesión actualizada.</response>
    /// <response code="400">El id de la URL no coincide con el del cuerpo, o datos inválidos.</response>
    /// <response code="404">No existe una profesión con ese id.</response>
    [HttpPut("{id:int}")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, Profesion profesion)
    {
        if (id != profesion.Id)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "El id de la URL no coincide con el del cuerpo.");
        }
        if (await _profesiones.GetByIdAsync(id) is null)
        {
            return NotFound();
        }

        await _profesiones.UpdateAsync(profesion);
        return NoContent();
    }

    /// <summary>Elimina una profesión; también se borran los estudios que la usan.</summary>
    /// <param name="id">Id de la profesión.</param>
    /// <response code="204">Profesión eliminada.</response>
    /// <response code="404">No existe una profesión con ese id.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        if (await _profesiones.GetByIdAsync(id) is null)
        {
            return NotFound();
        }

        await _profesiones.DeleteAsync(id);
        return NoContent();
    }
}
