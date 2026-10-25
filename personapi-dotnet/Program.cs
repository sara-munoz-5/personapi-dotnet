using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using personapi_dotnet.Interfaces;
using personapi_dotnet.Models.Entities;
using personapi_dotnet.Repository;

var builder = WebApplication.CreateBuilder(args);

// Controladores MVC y API. Las navegaciones de las entidades (por ejemplo DuenioNavigation)
// no son anulables, así que se desactiva el [Required] implícito; las reglas de cada campo
// se declaran explícitamente en Models/Metadata.
builder.Services.AddControllersWithViews(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;

    // Mensajes del enlace de modelos en español
    var mensajes = options.ModelBindingMessageProvider;
    mensajes.SetValueMustNotBeNullAccessor(_ => "Este campo es obligatorio.");
    mensajes.SetMissingBindRequiredValueAccessor(campo => $"Falta el valor de {campo}.");
    mensajes.SetAttemptedValueIsInvalidAccessor((valor, campo) => $"El valor '{valor}' no es válido para {campo}.");
    mensajes.SetValueIsInvalidAccessor(valor => $"El valor '{valor}' no es válido.");
    mensajes.SetValueMustBeANumberAccessor(campo => $"{campo} debe ser un número.");
})
// Las navegaciones (Persona.Estudios -> Estudio.CcPerNavigation -> ...) forman ciclos;
// se cortan al serializar para que la API devuelva las entidades sin bucles infinitos.
.AddJsonOptions(options =>
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Swagger con Swashbuckle: documento OpenAPI 3.0 y comentarios XML de los controladores API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "personapi-dotnet API",
        Version = "v1",
        Description = "CRUD REST de personas, profesiones, estudios y teléfonos (Laboratorio 1, Arquitectura de Software)."
    });

    var archivoXml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, archivoXml));
});

// Acceso a datos: contexto de EF Core y repositorios DAO, todos Scoped (uno por petición)
builder.Services.AddDbContext<PersonaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PersonaDb")));

builder.Services.AddScoped<IPersonaRepository, PersonaRepository>();
builder.Services.AddScoped<IProfesionRepository, ProfesionRepository>();
builder.Services.AddScoped<IEstudioRepository, EstudioRepository>();
builder.Services.AddScoped<ITelefonoRepository, TelefonoRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
// Swagger UI en /swagger, también fuera de Development para documentar el despliegue
app.UseSwagger(options => options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0);
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "personapi-dotnet API v1");
    options.RoutePrefix = "swagger";
    options.DocumentTitle = "personapi-dotnet API";
});

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
