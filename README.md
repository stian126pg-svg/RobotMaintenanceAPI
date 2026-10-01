# Robot Maintenance API

A REST API for managing robot maintenance data.

The application is built with ASP.NET Core Controllers and uses Entity Framework Core with PostgreSQL for persistent storage. Docker Compose runs the API, PostgreSQL, and pgAdmin as connected containers.

## Features

- Retrieve all robots
- Retrieve a robot by ID
- Filter robots by status
- Paginate robot results
- Register new robots
- Validate incoming requests
- Return appropriate HTTP status codes
- Persist robot data in PostgreSQL
- Apply EF Core migrations automatically
- Seed initial robot data
- Inspect stored data through pgAdmin
- Monitor API and PostgreSQL container health
- Preserve data through a Docker volume

## Technologies

- C#
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Npgsql
- Docker
- Docker Compose
- pgAdmin
- OpenAPI and Swagger UI
- Dependency Injection
- Async/await

## Project structure

```text
RobotMaintenanceAPI/
├── Controllers/
│   ├── AuthController.cs
│   └── RobotsController.cs
├── Data/
│   └── RobotDbContext.cs
├── Dtos/
│   └── CreateRobotRequest.cs
├── Migrations/
│   ├── InitialPostgreSql
│   └── RobotDbContextModelSnapshot.cs
├── Model/
│   └── Robot.cs
├── Services/
│   ├── IRobotService.cs
│   └── RobotService.cs
├── Properties/
│   └── launchSettings.json
├── .dockerignore
├── .env.example
├── .gitignore
├── adr.md
├── architecture-overview.md
├── docker-compose.yaml
├── Dockerfile
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
├── RobotMaintenanceAPI.csproj
└── README.md
```

## Architecture

The application separates HTTP handling, data operations, and persistence:

```text
HTTP request
    ↓
RobotsController
    ↓
IRobotService
    ↓
RobotService
    ↓
RobotDbContext
    ↓
Entity Framework Core
    ↓
PostgreSQL
```

`RobotsController` handles routes, query parameters, validation responses, and HTTP status codes.

`RobotService` performs robot operations asynchronously through Entity Framework Core.

`RobotDbContext` represents the connection between the application model and PostgreSQL.

## Robot model

A robot contains:

- `Id`
- `Name`
- `Model`
- `Status`
- `LastMaintenance`
- `NextMaintenance`

Example:

```json
{
  "id": 1,
  "name": "Atlas",
  "model": "XR-7",
  "status": "Operational",
  "lastMaintenance": "2026-08-01T00:00:00",
  "nextMaintenance": "2026-11-01T00:00:00"
}
```

Supported status values are:

- `Operational`
- `NeedsMaintenance`
- `OutOfService`

# API endpoints

## GET /api/robots

Returns a collection of robots.

```http
GET /api/robots
```

Optional query parameters:

| Parameter | Default | Description |
|---|---:|---|
| `status` | None | Filters robots by status |
| `page` | `1` | Selects the result page |
| `pageSize` | `10` | Controls the number of results |

Filtering is case-insensitive.

Example:

```http
GET /api/robots?status=Operational&page=1&pageSize=10
```

A `page` or `pageSize` value below `1` returns `400 Bad Request`.

## GET /api/robots/{id}

Returns one robot by ID.

```http
GET /api/robots/3
```

Possible responses:

- `200 OK`
- `404 Not Found`

## POST /api/robots

Creates a new robot.

```http
POST /api/robots
Content-Type: application/json
```

Example request:

```json
{
  "name": "Vaultkeeper",
  "model": "PG-17",
  "status": "Operational",
  "lastMaintenance": "2026-09-07T00:00:00",
  "nextMaintenance": "2027-01-07T00:00:00"
}
```

A successful request returns `201 Created`.

The response contains the created robot and its generated ID. The `Location` header points to the new resource.

# Validation

Robot creation uses a `CreateRobotRequest` DTO rather than accepting the database entity directly.

Validation includes:

- Name is required
- Model is required
- Maximum field lengths
- Status must contain a supported value

Invalid input returns `400 Bad Request`. Validation errors and invalid status values use ASP.NET Core problem responses.

# Asynchronous design

Database operations use asynchronous EF Core methods such as:

- `ToListAsync`
- `FirstOrDefaultAsync`
- `SaveChangesAsync`
- `MigrateAsync`

The application does not use blocking calls such as `.Result` or `.Wait()`.

# PostgreSQL and Entity Framework Core

Robot data is stored in PostgreSQL through Entity Framework Core and the Npgsql provider.

The PostgreSQL migration creates the `Robots` table and inserts four seed robots:

- Atlas
- Hammer
- Bishop
- Rustbucket

Pending migrations are automatically applied when the API starts:

```csharp
await dbContext.Database.MigrateAsync();
```

New robots created through the API remain available after the API container is restarted or recreated.

# Docker Compose

Docker Compose runs three services:

| Service | Purpose | Address |
|---|---|---|
| `api` | Robot Maintenance API | `http://localhost:8080` |
| `postgres` | PostgreSQL database | `localhost:5432` |
| `pgadmin` | PostgreSQL administration interface | `http://localhost:5050` |

Docker creates an internal network where the API and pgAdmin reach PostgreSQL using the service name `postgres`.

Database files are stored in the named volume `robot-postgres-data`.

## Requirements

- Docker Desktop
- Docker Compose

## Start the application

Copy `.env.example` to `.env` and replace both placeholder passwords with your own local values for starters. '.env' is ignored by Git and excluded from the Docker build context here.

```powershell
Copy-Item .env.example .env
# Edit .env and replace both placeholders here
```

Compose reads `.env` for variable substitution. The Compose file explicitly passes the database password to PostgreSQL and the API connection string, and the pgAdmin password to pgAdmin. Missing variables will cause an error. Avoid sharing `docker compose config` output. Its resolved configuration can contain passwords.

Check the configuration without printing the resolved passwords:

```powershell
git check-ignore .env
docker compose config --quiet
```

If you already have a `robot-postgres-data` volume, changing `POSTGRES_PASSWORD` in `.env` does not change the password stored in the existing PostgreSQL database. Use that database's current password in `.env`, or rotate the database user's password separately before using a new one. Do not remove the volume just to change a password: that would delete its data.

If you reuse an existing `pgadmin-data` volume, its saved login may still expect the password used when pgAdmin was first initialized. Use the current login for that volume unless you change it through pgAdmin.

```powershell
docker compose up -d --build
```


Check the services:

```powershell
docker compose ps
```

The API and PostgreSQL services should both report `healthy`.

## Stop the application

```powershell
docker compose down
```

This removes the containers and network but preserves the named volumes.

Do not use the following command unless you intentionally want to delete all stored database and pgAdmin data:

```powershell
docker compose down -v
```

# Health checks

PostgreSQL uses `pg_isready` to verify that the database accepts connections.

The API container calls:

```http
GET /health
```

The API health endpoint confirms that the application is responding. PostgreSQL health is checked separately by its own container health check.

Verify the API manually:

```powershell
Invoke-RestMethod `
    -Uri "http://localhost:8080/health"
```

Expected response:

```text
Healthy
```

# Testing with PowerShell

The robot endpoints require a bearer JWT. This project does not issue tokens. The examples below use the local Development profile at `http://localhost:5244`, with PostgreSQL running through Compose and a local `RobotDatabase` connection string configured through user secrets. Start the API with `dotnet run --launch-profile http` in another terminal. A development token created with `dotnet user-jwts` needs a `sub` claim such as `User A`. Set $token to a token accepted by the running API; never commit the token.

The Compose API runs in Production by default. It does not provide a token issuer or the local Development JWT settings, so a development token is not expected to work against `http://localhost:8080/api/robots` without further authentication configuration.

```powershell
$baseUrl = "http://localhost:5244"
```

## Get all robots

```powershell
Invoke-RestMethod `
    -Uri "$baseUrl/api/robots" `
    -Headers @{ Authorization = "Bearer $token" }
```

## Get a robot by ID

```powershell
Invoke-RestMethod `
    -Uri "$baseUrl/api/robots/1" `
    -Headers @{ Authorization = "Bearer $token" }
```

## Create a robot

```powershell
$body = @{
    name = "Vaultkeeper"
    model = "PG-17"
    status = "Operational"
    lastMaintenance = "2026-09-07T00:00:00"
    nextMaintenance = "2027-01-07T00:00:00"
} | ConvertTo-Json

$createdRobot = Invoke-RestMethod `
    -Uri "$baseUrl/api/robots" `
    -Method Post `
    -Headers @{ Authorization = "Bearer $token" } `
    -ContentType "application/json" `
    -Body $body

$createdRobot
```

# Verifying persistence

Create a robot using the Development API above, stop and then restart `dotnet run`, then retrieve the ID returned by the POST:

```powershell
Invoke-RestMethod `
    -Uri "$baseUrl/api/robots/$($createdRobot.id)" `
    -Headers @{ Authorization = "Bearer $token" }
```

Persistence can also be verified by removing and recreating the entire Compose stack:

```powershell
docker compose down
docker compose up -d
```

The robot remains stored because PostgreSQL uses the `robot-postgres-data` volume.

# pgAdmin

Open pgAdmin at:

```text
http://localhost:5050
```

Login credentials:

```text
Email: admin@robotmaintenance.com
Password: the value of PGADMIN_PASSWORD in your local .env
```

Register the PostgreSQL server using:

```text
Name: Robot PostgreSQL
Host: postgres
Port: 5432
Database: robotmaintenance
Username: robotadmin
Password: the value of POSTGRES_PASSWORD in your local .env
```

The host is `postgres`, rather than `localhost`, because pgAdmin connects through the internal Docker network.

The values in `.env.example` are placeholders. Replace these before starting the containers.

# OpenAPI and Swagger UI

When the API runs in the Development environment:

- Swagger UI is available at `/swagger`
- The generated OpenAPI document is available at `/openapi/v1.json`

The default Docker Compose environment is Production, so Swagger UI is not exposed through the container by default.

# HTTP status codes

| Status | Meaning |
|---|---|
| `200 OK` | Resource successfully retrieved |
| `201 Created` | Robot successfully created |
| `400 Bad Request` | Invalid input or query parameters |
| `401 Bad Request` | Unauthorized |
| `404 Not Found` | Robot does not exist |

# Build without Docker

Restore dependencies and compile the project:

```powershell
dotnet restore
dotnet build
```

Running the API directly requires an available PostgreSQL database and a valid `RobotDatabase` connection string.

# Possible future improvements

Possible extensions include:

- Update endpoints
- Delete endpoint
- Maintenance history
- Additional filtering and sorting
- Automated unit tests
- Integration tests

These are outside the current assignment scope.

# Assignment requirements covered

This project demonstrates:

- ASP.NET Core Controllers
- REST endpoints
- Input validation
- HTTP status codes
- Asynchronous database operations
- Dependency injection
- DTO usage
- Entity Framework Core
- PostgreSQL persistence
- EF Core migrations
- Seed data
- Docker image creation
- Docker Compose orchestration
- PostgreSQL container setup
- pgAdmin database inspection
- Named-volume persistence
- PostgreSQL health checking
- API health checking