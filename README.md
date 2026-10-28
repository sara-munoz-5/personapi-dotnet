# personapi-dotnet

Monolito ASP.NET Core MVC con patrón DAO, servicios de negocio y API REST para administrar personas, profesiones, estudios y teléfonos.

## Tecnologías

- ASP.NET Core MVC y API REST
- .NET 10 y Entity Framework Core 10 (versión actual del repositorio)
- SQL Server 2022
- Swagger/OpenAPI 3
- Bootstrap y vistas Razor

> El enunciado original menciona .NET 7/.NET 6. El proyecto está configurado actualmente para .NET 10; se debe confirmar con el profesor si es necesario cambiar la versión.

## Requisitos

- .NET SDK 10
- SQL Server 2019/2022, Express o Developer
- Visual Studio o un editor compatible con .NET

## Configuración de la base de datos

1. Ejecutar `Database/DDL/01_crear_base_datos.sql` (crea `persona_db` si no existe).
2. Ejecutar `Database/DDL/02_crear_tablas.sql` sobre esa base de datos.
3. Ejecutar `Database/DML/01_insertar_datos.sql` y `Database/DML/02_insertar_datos.sql`.
4. Revisar la cadena `PersonaConnection` en `appsettings.json`:

```text
Server=localhost\\SQLEXPRESS;Database=persona_db;Trusted_Connection=True;TrustServerCertificate=True;
```

Para otra instancia, cambiar únicamente el valor de `Server`.

## Compilar y ejecutar

```powershell
dotnet restore
dotnet build personapi-dotnet.csproj
dotnet run --project personapi-dotnet.csproj
```

La aplicación MVC estará disponible en la dirección mostrada por ASP.NET Core. La documentación interactiva de la API estará en:

- Swagger UI: `/swagger`
- Documento OpenAPI: `/swagger/v1/swagger.json`

## Estructura principal

- `Models/Entities`: entidades generadas y `PersonaDbContext` (namespace `personapi_dotnet.Models.EntitiesGeneradas`).
- `Models/DTOs`: contratos de entrada y salida de los servicios y la API.
- `Controllers/Repositories`: interfaces e implementaciones DAO con EF Core.
- `Controllers/Services`: reglas de negocio y mapeo entre entidades y DTO.
- `Controllers/ApiControllers`: endpoints REST.
- `Controllers/ViewControllers`: controladores de las vistas MVC.
- `Views`: vistas Razor para los CRUD.
- `Database`: scripts DDL y DML.
- `docs`: documentación técnica y de pruebas.

## API

La especificación completa de endpoints está en [docs/api.md](docs/api.md). La descripción de la arquitectura está en [docs/arquitectura.md](docs/arquitectura.md) y las reglas de Services en [docs/servicios.md](docs/servicios.md).

## Flujo de la aplicación

```text
Vista Razor o cliente HTTP
          |
Controller MVC o API Controller
          |
       Service
          |
      DAO/EF Core
          |
      SQL Server
```

Los controladores no acceden directamente al `DbContext`. Las reglas de negocio se concentran en Services y los DAO se encargan de la persistencia.