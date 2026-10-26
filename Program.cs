using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Models.EntitiesGeneradas;
using personapi_dotnet.Controllers.Repositories.Interfaces;
using personapi_dotnet.Controllers.Repositories.Implementations;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<PersonaDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("PersonaConnection")
    )
);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IPersonaDAO, PersonaDAO>();
builder.Services.AddScoped<IProfesionDAO, ProfesionDAO>();
builder.Services.AddScoped<IEstudiosDAO, EstudiosDAO>();
builder.Services.AddScoped<ITelefonoDAO, TelefonoDAO>();
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

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
