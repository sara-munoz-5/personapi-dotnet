using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Models.EntitiesGeneradas;
using personapi_dotnet.Controllers.Repositories.Interfaces;
using personapi_dotnet.Controllers.Repositories.Implementations;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Controllers.Services.Implementations;
using personapi_dotnet.Controllers.ApiControllers;
using Microsoft.OpenApi;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<PersonaDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("PersonaConnection")
    )
);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    // Mensajes del enlace de modelos en español (formularios MVC, ruta, query y cuerpo vacío de la API).
    var mensajes = options.ModelBindingMessageProvider;
    mensajes.SetAttemptedValueIsInvalidAccessor((valor, campo) => $"El valor '{valor}' no es válido para {campo}.");
    mensajes.SetNonPropertyAttemptedValueIsInvalidAccessor(valor => $"El valor '{valor}' no es válido.");
    mensajes.SetUnknownValueIsInvalidAccessor(campo => $"El valor no es válido para {campo}.");
    mensajes.SetNonPropertyUnknownValueIsInvalidAccessor(() => "El valor no es válido.");
    mensajes.SetValueIsInvalidAccessor(valor => $"El valor '{valor}' no es válido.");
    mensajes.SetValueMustBeANumberAccessor(campo => $"El campo {campo} debe ser un número.");
    mensajes.SetNonPropertyValueMustBeANumberAccessor(() => "El campo debe ser un número.");
    mensajes.SetValueMustNotBeNullAccessor(valor => $"El valor '{valor}' no es válido.");
    mensajes.SetMissingBindRequiredValueAccessor(campo => $"Falta un valor para {campo}.");
    mensajes.SetMissingKeyOrValueAccessor(() => "Falta un valor.");
    mensajes.SetMissingRequestBodyRequiredValueAccessor(() => "Se requiere un cuerpo JSON válido.");
})
.ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = RespuestaValidacionApi.Crear;
    options.ClientErrorMapping[StatusCodes.Status400BadRequest].Title = "Solicitud no válida.";
    options.ClientErrorMapping[StatusCodes.Status404NotFound].Title = "Recurso no encontrado";
    options.ClientErrorMapping[StatusCodes.Status405MethodNotAllowed].Title = "Método no permitido.";
    options.ClientErrorMapping[StatusCodes.Status406NotAcceptable].Title = "Formato de respuesta no admitido.";
    options.ClientErrorMapping[StatusCodes.Status409Conflict].Title = "Conflicto de negocio";
    options.ClientErrorMapping[StatusCodes.Status415UnsupportedMediaType].Title = "Tipo de contenido no admitido.";
    options.ClientErrorMapping[StatusCodes.Status500InternalServerError].Title = "Error interno del servidor.";
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Person API",
        Version = "v1",
        Description = "API REST del monolito MVC y DAO para personas, profesiones, estudios y teléfonos."
    });
});

builder.Services.AddScoped<IPersonaDAO, PersonaDAO>();
builder.Services.AddScoped<IProfesionDAO, ProfesionDAO>();
builder.Services.AddScoped<IEstudiosDAO, EstudiosDAO>();
builder.Services.AddScoped<ITelefonoDAO, TelefonoDAO>();
builder.Services.AddScoped<IPersonaService, PersonaService>();
builder.Services.AddScoped<IProfesionService, ProfesionService>();
builder.Services.AddScoped<IEstudiosService, EstudiosService>();
builder.Services.AddScoped<ITelefonoService, TelefonoService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// API: un error inesperado responde 500 con ProblemDetails genérico (detalle solo en el log, contratos §5.3),
// y un 404 sin cuerpo (por ejemplo /api/personas/abc) también responde con ProblemDetails.
app.UseWhen(contexto => contexto.Request.Path.StartsWithSegments("/api"), api =>
{
    api.UseExceptionHandler(manejador => manejador.Run(contexto =>
        Results.Problem(
            title: "Error interno del servidor.",
            detail: "Ocurrió un error inesperado. Intente de nuevo más tarde.",
            statusCode: StatusCodes.Status500InternalServerError).ExecuteAsync(contexto)));
    api.UseStatusCodePages(contexto => contexto.HttpContext.Response.StatusCode == StatusCodes.Status404NotFound
        ? Results.Problem(title: "Recurso no encontrado", detail: "La ruta solicitada no existe.",
            statusCode: StatusCodes.Status404NotFound).ExecuteAsync(contexto.HttpContext)
        : Task.CompletedTask);
});

app.UseHttpsRedirection();
app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Person API v1");
    options.RoutePrefix = "swagger";
});

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
