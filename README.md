# personapi-dotnet

Monolito ASP.NET Core MVC con patrón DAO, servicios de negocio y API REST para administrar personas, profesiones, estudios y teléfonos.

## Tecnologías

- ASP.NET Core MVC y API REST
- .NET 10 y Entity Framework Core 10 (versión actual del repositorio)
- SQL Server 2022
- Swagger/OpenAPI 3
- Bootstrap y vistas Razor

> El enunciado original menciona .NET 7/.NET 6. El proyecto está configurado actualmente para .NET 10; se debe confirmar con el profesor si es necesario cambiar la versión.

## Configuración del ambiente

1. Instalar el [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0) y comprobarlo:

   ```powershell
   dotnet --version   # 10.0.x
   ```

2. Instalar SQL Server 2019/2022 (Express o Developer). Para ejecutar los scripts, instalar también
   SQL Server Management Studio o `sqlcmd`.
3. Opcional: Visual Studio 2022 (o un editor compatible con .NET) para abrir `personapi-dotnet.csproj`.
4. Clonar el repositorio:

   ```powershell
   git clone https://github.com/sara-munoz-5/personapi-dotnet.git
   cd personapi-dotnet
   ```

5. Crear la base con los scripts y revisar la cadena de conexión, como se explica en la sección siguiente.

Los paquetes NuGet (EF Core 10.0.14, Swashbuckle 10.3.1) se descargan solos con `dotnet restore`.

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

### 1. Publicar

```powershell
dotnet publish personapi-dotnet.csproj -c Release -o publicado
```

La carpeta `publicado` queda con todo lo necesario: `personapi-dotnet.dll`, sus dependencias, `appsettings.json`,
las vistas compiladas y `wwwroot`. El servidor solo necesita el runtime de ASP.NET Core 10, no el SDK.

### 2. Configurar la conexión en el servidor

Usar una de estas dos opciones:

- editar `publicado/appsettings.json` y cambiar `Server` en `PersonaConnection`;
- o definir la variable de entorno antes de iniciar la aplicación, sin tocar el archivo:

  ```powershell
  $env:ConnectionStrings__PersonaConnection = "Server=MI_SERVIDOR\SQLEXPRESS;Database=persona_db;Trusted_Connection=True;TrustServerCertificate=True;"
  ```

La base debe existir en ese servidor, creada con los scripts de `Database/`.

### 3. Ejecutar

```powershell
cd publicado
$env:ASPNETCORE_URLS = "http://localhost:5080"
dotnet personapi-dotnet.dll
```

Para comprobar el despliegue, abrir `http://localhost:5080/` (menú), `http://localhost:5080/Personas` (lista leída de
la base) y `http://localhost:5080/swagger`. Para detenerla, `Ctrl+C`.

La versión publicada corre en el ambiente `Production`. En ese ambiente, los errores muestran una página genérica en
español y la API responde `ProblemDetails` sin datos internos. Swagger queda disponible en `/swagger`.

### Opcional: IIS

1. Instalar el *ASP.NET Core Hosting Bundle* de .NET 10 en el servidor.
2. En IIS, crear un sitio cuya ruta física sea la carpeta `publicado` (el `web.config` lo genera `dotnet publish`).
3. En el grupo de aplicaciones, elegir "Sin código administrado".
4. Con `Trusted_Connection=True`, dar acceso a `persona_db` en SQL Server al usuario del grupo de aplicaciones
   (`IIS APPPOOL\<nombre del grupo>`), o usar en la cadena un usuario de SQL Server.

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
