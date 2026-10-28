using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Models.EntitiesGeneradas;
using personapi_dotnet.Controllers.Repositories.Interfaces;
using personapi_dotnet.Controllers.Repositories.Implementations;
using personapi_dotnet.Controllers.Services.Interfaces;
using personapi_dotnet.Controllers.Services.Implementations;
using Microsoft.OpenApi;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<PersonaDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("PersonaConnection")
    )
);

// Add services to the container.
builder.Services.AddControllersWithViews();
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
