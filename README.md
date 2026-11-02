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

Ejecutar los scripts en SQL Server Management Studio (o con `sqlcmd`), en este orden:

1. `Database/DDL/01_crear_base_datos.sql`: crea `persona_db` si no existe.
2. `Database/DDL/02_crear_tablas.sql`: crea las tablas `persona`, `profesion`, `estudios` y `telefono` con sus llaves
   (las llaves foráneas no borran en cascada).
3. `Database/DML/01_insertar_datos.sql` y `Database/DML/02_insertar_datos.sql`: datos ficticios mínimos.
4. Opcional: `Database/DML/03_datos_prueba_ampliados.sql`, que carga más personas, profesiones, estudios y teléfonos.
   Se puede ejecutar varias veces sin duplicar datos.

Después, revisar la cadena `PersonaConnection` en `appsettings.json`:

```json
"PersonaConnection": "Server=localhost\\SQLEXPRESS;Database=persona_db;Trusted_Connection=True;TrustServerCertificate=True;"
```

Para otra instancia, cambiar únicamente el valor de `Server`. La cadena usa autenticación de Windows; no se guardan
contraseñas en el repositorio. Para otra cadena sin tocar el archivo, se puede usar la variable de entorno
`ConnectionStrings__PersonaConnection`.

## Compilar y ejecutar

```powershell
dotnet restore
dotnet build personapi-dotnet.csproj
dotnet run --project personapi-dotnet.csproj --launch-profile http
```

Con el perfil `http` la aplicación queda en `http://localhost:5269`:

| Qué | URL |
|---|---|
| Inicio y menú | `http://localhost:5269/` |
| Swagger UI | `http://localhost:5269/swagger` |
| Documento OpenAPI 3 | `http://localhost:5269/swagger/v1/swagger.json` |
| API REST | `http://localhost:5269/api/personas`, `/api/profesiones`, `/api/estudios`, `/api/telefonos` |

Desde Visual Studio, elegir el perfil `http` y ejecutar (F5).

## Despliegue

Publicar en una carpeta y ejecutar el `.dll` publicado:

```powershell
dotnet publish personapi-dotnet.csproj -c Release -o publicado
cd publicado
$env:ASPNETCORE_URLS = "http://localhost:5080"
dotnet personapi-dotnet.dll
```

La versión publicada corre en el ambiente `Production`. En ese ambiente, los errores muestran una página genérica en
español y la API responde `ProblemDetails` sin datos internos. Swagger queda disponible en `/swagger`.

## Vistas

El menú lleva a las listas de cada entidad. Cada lista tiene Crear, Detalle, Editar y Eliminar (20 vistas, un
controlador y un ViewModel por vista):

| Entidad | Rutas |
|---|---|
| Personas | `/Personas`, `/Personas/Crear`, `/Personas/{Detalle,Editar,Eliminar}/{cc}` |
| Profesiones | `/Profesiones`, `/Profesiones/Crear`, `/Profesiones/{Detalle,Editar,Eliminar}/{id}` |
| Estudios | `/Estudios`, `/Estudios/Crear`, `/Estudios/{Detalle,Editar,Eliminar}/{idProf}/{ccPer}` |
| Teléfonos | `/Telefonos`, `/Telefonos/Crear`, `/Telefonos/{Detalle,Editar,Eliminar}/{num}` |

Una persona o profesión con registros relacionados no se elimina: la vista explica el motivo. El detalle está en
[docs/vistas-mvc.md](docs/vistas-mvc.md).

## Pruebas

- Pruebas de aceptación del contrato (sección 8) contra SQL Server 2022 con los scripts de `Database/`:
  - interfaz web: 100 de 100 casos;
  - API REST: 62 de 62 casos (códigos 200, 201, 204, 400, 404 y 409, y Swagger).
- Resultados y casos de borde: [docs/pruebas.md](docs/pruebas.md).
- Capturas: `docs/evidencias/`.

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

La especificación completa de endpoints está en [docs/api.md](docs/api.md). La descripción de la arquitectura está en [docs/arquitectura.md](docs/arquitectura.md) y las reglas de Services en [docs/servicios.md](docs/servicios.md). Los diagramas de capas, del modelo de datos y de una petición están en [docs/diagramas](docs/diagramas/README.md), y las capturas, en [docs/evidencias](docs/evidencias/README.md).

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
