using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Models.DTOs;

namespace personapi_dotnet.Controllers.ApiControllers;

[ApiController]
[Route("api/personas")]
[Produces("application/json")]
public class PersonaApiController : ControllerBase
{
    private readonly IPersonaService _personas;

    public PersonaApiController(IPersonaService personas) => _personas = personas;

    [HttpGet]
    public async Task<ActionResult<List<PersonaDto>>> ObtenerTodos() => Ok(await _personas.ObtenerTodosAsync());

    [HttpGet("{cc:int}")]
    public async Task<ActionResult<PersonaDto>> ObtenerPorId(int cc)
    {
        var persona = await _personas.ObtenerPorIdAsync(cc);
        return persona == null ? NotFound() : Ok(persona);
    }

    [HttpPost]
    public async Task<ActionResult<PersonaDto>> Crear(PersonaCrearDto dto)
    {
        try
        {
            var persona = await _personas.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { cc = persona.Cc }, persona);
        }
        catch (ConflictoNegocioException ex) when (ex.Tipo == TipoConflicto.LlaveDuplicada)
        {
            return Conflict(new { mensaje = ex.Message, tipo = ex.Tipo.ToString() });
        }
    }

    [HttpPut("{cc:int}")]
    public async Task<IActionResult> Actualizar(int cc, PersonaActualizarDto dto)
    {
        if (!await _personas.ActualizarAsync(cc, dto))
            return NotFound();
        return NoContent();
    }

    [HttpDelete("{cc:int}")]
    public async Task<IActionResult> Eliminar(int cc)
    {
        try
        {
            if (!await _personas.EliminarAsync(cc))
                return NotFound();
            return NoContent();
        }
        catch (ConflictoNegocioException ex) when (ex.Tipo == TipoConflicto.BorradoConReferencias)
        {
            return Conflict(new { mensaje = ex.Message, tipo = ex.Tipo.ToString() });
        }
    }
}
