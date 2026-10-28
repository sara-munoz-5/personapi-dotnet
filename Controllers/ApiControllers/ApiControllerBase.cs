using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Controllers.Services.Interfaces;

namespace personapi_dotnet.Controllers.ApiControllers;

public abstract class ApiControllerBase : ControllerBase
{
    protected NotFoundObjectResult RecursoNoEncontrado(string detalle) =>
        NotFound(new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Recurso no encontrado",
            Detail = detalle
        });

    protected ConflictObjectResult Conflicto(ConflictoNegocioException exception) =>
        Conflict(new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title = "Conflicto de negocio",
            Detail = exception.Message,
            Extensions = { ["tipo"] = exception.Tipo.ToString() }
        });
}
