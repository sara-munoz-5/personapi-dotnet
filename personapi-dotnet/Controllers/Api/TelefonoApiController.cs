using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Interfaces;
using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.Controllers.Api;

/// <summary>
/// Servicios REST del CRUD de teléfonos.
/// </summary>
[ApiController]
[Route("api/telefonos")]
[Produces("application/json")]
public class TelefonoApiController : ControllerBase
{
    private readonly ITelefonoRepository _telefonos;

    public TelefonoApiController(ITelefonoRepository telefonos)
    {
        _telefonos = telefonos;
    }

    /// <summary>Lista todos los teléfonos con su dueño.</summary>
    /// <response code="200">Lista de teléfonos.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<Telefono>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Telefono>>> GetAll()
    {
        return Ok(await _telefonos.GetAllAsync());
    }

    /// <summary>Consulta un teléfono por su número.</summary>
    /// <param name="num">Número del teléfono.</param>
    /// <response code="200">El teléfono encontrado.</response>
    /// <response code="404">No existe un teléfono con ese número.</response>
    [HttpGet("{num}")]
    [ProducesResponseType(typeof(Telefono), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Telefono>> GetById(string num)
    {
        var telefono = await _telefonos.GetByIdAsync(num);
        if (telefono is null)
        {
            return NotFound();
        }
        return Ok(telefono);
    }

    /// <summary>Crea un teléfono.</summary>
    /// <param name="telefono">Datos del teléfono; duenio es la cédula de una persona existente.</param>
    /// <response code="201">Teléfono creado; el encabezado Location apunta a él.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="409">Ya existe un teléfono con ese número.</response>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Telefono), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Telefono>> Create(Telefono telefono)
    {
        if (await _telefonos.GetByIdAsync(telefono.Num) is not null)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "El número de teléfono ya existe.");
        }

        try
        {
            await _telefonos.AddAsync(telefono);
        }
        catch (Exception ex) when (ex is not NotImplementedException)
        {
            // El repositorio lanza error si la llave quedó repetida entre la consulta y el guardado
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "El número de teléfono ya existe.", detail: ex.Message);
        }

        return CreatedAtAction(nameof(GetById), new { num = telefono.Num }, telefono);
    }

    /// <summary>Actualiza un teléfono existente.</summary>
    /// <param name="num">Número del teléfono a actualizar.</param>
    /// <param name="telefono">Datos nuevos; su número debe coincidir con el de la URL.</param>
    /// <response code="204">Teléfono actualizado.</response>
    /// <response code="400">El número de la URL no coincide con el del cuerpo, o datos inválidos.</response>
    /// <response code="404">No existe un teléfono con ese número.</response>
    [HttpPut("{num}")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string num, Telefono telefono)
    {
        if (num != telefono.Num)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "El número de la URL no coincide con el del cuerpo.");
        }
        if (await _telefonos.GetByIdAsync(num) is null)
        {
            return NotFound();
        }

        await _telefonos.UpdateAsync(telefono);
        return NoContent();
    }

    /// <summary>Elimina un teléfono.</summary>
    /// <param name="num">Número del teléfono.</param>
    /// <response code="204">Teléfono eliminado.</response>
    /// <response code="404">No existe un teléfono con ese número.</response>
    [HttpDelete("{num}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string num)
    {
        if (await _telefonos.GetByIdAsync(num) is null)
        {
            return NotFound();
        }

        await _telefonos.DeleteAsync(num);
        return NoContent();
    }
}
