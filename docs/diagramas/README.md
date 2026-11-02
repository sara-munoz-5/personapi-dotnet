# Diagramas

GitHub muestra los diagramas Mermaid directamente.

## Capas del monolito

Las vistas y la API comparten los mismos Services. Ningún controlador accede a los DAO ni al `DbContext`.

```mermaid
flowchart TD
    N[Navegador] --> V[Vistas Razor]
    V --> CV["Controller View<br/>Controllers/ViewControllers (20)"]
    C[Cliente HTTP / Swagger UI] --> CA["API Controller<br/>Controllers/ApiControllers (4)"]
    CV --> IS["Interfaces Service<br/>Controllers/Services/Interfaces"]
    CA --> IS
    IS --> S["Services<br/>Controllers/Services/Implementations"]
    S --> ID["Interfaces DAO<br/>Controllers/Repositories/Interfaces"]
    ID --> D["DAO<br/>Controllers/Repositories/Implementations"]
    D --> EF["PersonaDbContext<br/>Entity Framework Core 10"]
    EF --> BD[("SQL Server<br/>persona_db")]
```

## Modelo de datos

Las llaves foráneas no borran en cascada (`NO ACTION`). Una persona con estudios o teléfonos, o una profesión con
estudios, no se puede eliminar.

```mermaid
erDiagram
    persona ||--o{ estudios : "cc_per"
    profesion ||--o{ estudios : "id_prof"
    persona ||--o{ telefono : "duenio"

    persona {
        int cc PK
        varchar nombre "varchar(45)"
        varchar apellido "varchar(45)"
        char genero "char(1), M o F"
        int edad ">= 0"
    }
    profesion {
        int id PK "no es IDENTITY"
        varchar nom "varchar(90)"
        varchar des "varchar(max), opcional"
    }
    estudios {
        int id_prof PK, FK
        int cc_per PK, FK
        date fecha "opcional"
        varchar univer "varchar(50), opcional"
    }
    telefono {
        varchar num PK "varchar(15)"
        varchar oper "varchar(45)"
        int duenio FK
    }
```

## Flujo de una petición: crear un estudio desde la vista

```mermaid
sequenceDiagram
    actor U as Usuario
    participant CV as EstudiosCrearController
    participant S as EstudiosService
    participant D as DAO
    participant BD as SQL Server
    U->>CV: POST /Estudios/Crear (formulario + token)
    CV->>CV: valida ModelState
    CV->>S: CrearAsync(EstudiosCrearDto)
    S->>D: ¿existe (idProf, ccPer)? ¿existen persona y profesión?
    D->>BD: SELECT
    alt llave repetida o referencia inexistente
        S-->>CV: ConflictoNegocioException
        CV-->>U: formulario con el mensaje en español
    else válido
        S->>D: CrearAsync(Estudio)
        D->>BD: INSERT
        CV-->>U: 302 a /Estudios con mensaje de confirmación
    end
```
