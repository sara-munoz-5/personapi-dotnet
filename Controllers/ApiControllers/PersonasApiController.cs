using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Models.DTOs;

namespace personapi_dotnet.Controllers.ApiControllers;

[ApiController]
[Route("api/personas")]
[Produces("application/json")]
public class PersonasApiController : ApiControllerBase
{
    private readonly IPersonaService _personas;

    public PersonasApiController(IPersonaService personas) => _personas = personas;

    [HttpGet]
    public async Task<ActionResult<List<PersonaDto>>> ObtenerTodos() => Ok(await _personas.ObtenerTodosAsync());

    [HttpGet("{cc:int}")]
    public async Task<ActionResult<PersonaDto>> ObtenerPorId(int cc)
    {
        var persona = await _personas.ObtenerPorIdAsync(cc);
        return persona == null ? RecursoNoEncontrado($"No existe la persona con cédula {cc}.") : Ok(persona);
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
            return Conflicto(ex);
        }
    }

    [HttpPut("{cc:int}")]
    public async Task<IActionResult> Actualizar(int cc, PersonaActualizarDto dto)
    {
        if (!await _personas.ActualizarAsync(cc, dto))
            return RecursoNoEncontrado($"No existe la persona con cédula {cc}.");
        return NoContent();
    }

    [HttpDelete("{cc:int}")]
    public async Task<IActionResult> Eliminar(int cc)
    {
        try
        {
            if (!await _personas.EliminarAsync(cc))
                return RecursoNoEncontrado($"No existe la persona con cédula {cc}.");
            return NoContent();
        }
        catch (ConflictoNegocioException ex) when (ex.Tipo == TipoConflicto.BorradoConReferencias)
        {
            return Conflicto(ex);
        }
    }
}
