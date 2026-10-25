using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Interfaces;
using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.Controllers.Api;

/// <summary>
/// Servicios REST del CRUD de personas.
/// </summary>
[ApiController]
[Route("api/personas")]
[Produces("application/json")]
public class PersonaApiController : ControllerBase
{
    private readonly IPersonaRepository _personas;

    public PersonaApiController(IPersonaRepository personas)
    {
        _personas = personas;
    }

    /// <summary>Lista todas las personas.</summary>
    /// <response code="200">Lista de personas.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<Persona>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Persona>>> GetAll()
    {
        return Ok(await _personas.GetAllAsync());
    }

    /// <summary>Consulta una persona por su cédula, con sus estudios y teléfonos.</summary>
    /// <param name="cc">Cédula de la persona.</param>
    /// <response code="200">La persona encontrada.</response>
    /// <response code="404">No existe una persona con esa cédula.</response>
    [HttpGet("{cc:int}")]
    [ProducesResponseType(typeof(Persona), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Persona>> GetById(int cc)
    {
        var persona = await _personas.GetByIdAsync(cc);
        if (persona is null)
        {
            return NotFound();
        }
        return Ok(persona);
    }

    /// <summary>Crea una persona.</summary>
    /// <param name="persona">Datos de la persona; la cédula la digita el usuario.</param>
    /// <response code="201">Persona creada; el encabezado Location apunta a ella.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="409">Ya existe una persona con esa cédula.</response>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Persona), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Persona>> Create(Persona persona)
    {
        if (await _personas.GetByIdAsync(persona.Cc) is not null)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "La cédula ya existe.");
        }

        try
        {
            await _personas.AddAsync(persona);
        }
        catch (DbUpdateException ex)
        {
            // EF Core lanza DbUpdateException si la llave quedó repetida entre la consulta y el guardado
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "La cédula ya existe.", detail: ex.Message);
        }

        return CreatedAtAction(nameof(GetById), new { cc = persona.Cc }, persona);
    }

    /// <summary>Actualiza una persona existente.</summary>
    /// <param name="cc">Cédula de la persona a actualizar.</param>
    /// <param name="persona">Datos nuevos; su cédula debe coincidir con la de la URL.</param>
    /// <response code="204">Persona actualizada.</response>
    /// <response code="400">La cédula de la URL no coincide con la del cuerpo, o datos inválidos.</response>
    /// <response code="404">No existe una persona con esa cédula.</response>
    [HttpPut("{cc:int}")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int cc, Persona persona)
    {
        if (cc != persona.Cc)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "La cédula de la URL no coincide con la del cuerpo.");
        }
        if (await _personas.GetByIdAsync(cc) is null)
        {
            return NotFound();
        }

        await _personas.UpdateAsync(persona);
        return NoContent();
    }

    /// <summary>Elimina una persona; también se borran sus estudios y teléfonos.</summary>
    /// <param name="cc">Cédula de la persona.</param>
    /// <response code="204">Persona eliminada.</response>
    /// <response code="404">No existe una persona con esa cédula.</response>
    [HttpDelete("{cc:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int cc)
    {
        if (await _personas.GetByIdAsync(cc) is null)
        {
            return NotFound();
        }

        await _personas.DeleteAsync(cc);
        return NoContent();
    }
}
