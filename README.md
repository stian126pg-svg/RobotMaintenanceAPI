# Robot Maintenance API

A REST API built with ASP.NET Core Controllers for managing robot maintenance data.

The project was created as part of a backend development assignment focused on REST APIs, HTTP conventions, asynchronous programming, validation, and clean project structure.

The API was later extended with:

- Entity Framework Core
- SQLite persistence
- EF Core migrations
- Seed data
- OpenAPI documentation
- Swagger UI

---

## Features

The API currently supports:

- Get all robots
- Get a robot by ID
- Filter robots by status
- Paginate robot results
- Create new robots
- Validate incoming data
- Return appropriate HTTP status codes
- Return `ProblemDetails` responses for API errors
- Persist robot data using SQLite
- Manage the database schema using EF Core migrations
- Seed initial robot data through EF Core
- Explore and test the API through Swagger UI

---

## Technology

The project uses:

- C#
- .NET 10
- ASP.NET Core Web API
- Controllers
- Entity Framework Core
- SQLite
- OpenAPI
- Swagger UI
- Dependency Injection
- Async/await

---

## Project Structure

```text
RobotMaintenanceAPI/
│
├── Controllers/
│   └── RobotsController.cs
│
├── Data/
│   └── RobotDbContext.cs
│
├── Migrations/
│   ├── InitialCreate
│   ├── SeedRobots
│   └── RobotDbContextModelSnapshot.cs
│
├── Model/
│   ├── Robot.cs
│   └── CreateRobotRequest.cs
│
├── Services/
│   ├── IRobotService.cs
│   └── RobotService.cs
│
├── Properties/
│   └── launchSettings.json
│
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
├── RobotMaintenanceAPI.csproj
└── README.md
```

The local SQLite database file is excluded from Git. The database can be recreated from the EF Core migrations.

---

## Architecture

The application separates HTTP handling, business/data access logic, and persistence.

```text
HTTP Request
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
SQLite
```

The controller is responsible for HTTP concerns such as:

- Routes
- Query parameters
- HTTP status codes
- Validation responses

The service handles robot operations and communicates asynchronously with EF Core.

`RobotDbContext` represents the connection between the application model and the SQLite database.

---

## Robot Model

A robot contains:

- `Id`
- `Name`
- `Model`
- `Status`
- `LastMaintenance`
- `NextMaintenance`

Example response:

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

Supported statuses are:

- `Operational`
- `NeedsMaintenance`
- `OutOfService`

---

# API Endpoints

## GET /api/Robots

Returns a list of robots.

Example:

```http
GET /api/Robots
```

### Filtering

Robots can be filtered by status:

```http
GET /api/Robots?status=Operational
```

Status filtering is case-insensitive.

### Pagination

The endpoint supports `page` and `pageSize`.

```http
GET /api/Robots?page=1&pageSize=2
```

Defaults:

```text
page = 1
pageSize = 10
```

Values below `1` return `400 Bad Request`.

---

## GET /api/Robots/{id}

Returns one robot by ID.

Example:

```http
GET /api/Robots/3
```

Possible responses:

```text
200 OK
404 Not Found
```

A missing robot returns a `ProblemDetails` response.

---

## POST /api/Robots

Creates a new robot.

Example request:

```json
{
  "name": "Sentinel",
  "model": "ST-9",
  "status": "Operational",
  "lastMaintenance": "2026-08-10T00:00:00",
  "nextMaintenance": "2026-12-10T00:00:00"
}
```

A successful request returns:

```text
201 Created
```

The response contains the newly created robot, including its generated ID.

The `Location` header points to the new resource, for example:

```text
/api/Robots/5
```

---

# Validation

Creation uses a separate `CreateRobotRequest` DTO instead of accepting the domain model directly.

Examples of validation include:

- Name is required
- Model is required
- Maximum field lengths
- Status must contain a supported value

Invalid model validation automatically produces an ASP.NET Core validation response using `ProblemDetails`.

Example:

```json
{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": [
      "The Name field is required."
    ]
  }
}
```

Invalid robot statuses also return `400 Bad Request` using `ProblemDetails`.

---

# Asynchronous Design

The API uses Task-based asynchronous methods through the application stack.

```text
Controller
    ↓ await
Service
    ↓ await
Entity Framework Core
    ↓
SQLite
```

Database operations use EF Core asynchronous methods rather than blocking calls.

This keeps the API ready for real I/O without relying on `.Result` or `.Wait()`.

---

# Entity Framework Core and SQLite

Robot data is persisted in a local SQLite database using Entity Framework Core.

The database schema is managed using migrations.

Current migrations include:

```text
InitialCreate
SeedRobots
```

`InitialCreate` creates the robot table.

`SeedRobots` inserts the initial robot data:

- Atlas
- Hammer
- Bishop
- Rustbucket

New robots created through the API are persisted to the database and remain available after the application restarts.

The local SQLite database itself is not committed to Git.

---

# Running the Project

## Requirements

You need:

- .NET 10 SDK
- EF Core command-line tools

Check your .NET installation:

```powershell
dotnet --version
```

If the EF CLI tool is not installed:

```powershell
dotnet tool install --global dotnet-ef
```

---

## 1. Clone the repository

Clone the project and navigate into the project directory.

```powershell
cd RobotMaintenanceAPI
```

## 2. Restore packages

```powershell
dotnet restore
```

## 3. Create/update the local database

Apply the included EF Core migrations:

```powershell
dotnet ef database update
```

This creates the local SQLite database and applies the seed data.

## 4. Run the API

```powershell
dotnet run
```

The development server will print the active address in the terminal.

For example:

```text
http://localhost:5244
```

The exact port may differ depending on the local launch configuration.

---

# Swagger UI

When running in the Development environment, Swagger UI is available at:

```text
/swagger
```

For the default local configuration this may be:

```text
http://localhost:5244/swagger
```

Swagger provides an interactive interface for:

- Viewing endpoints
- Entering query parameters
- Sending GET requests
- Sending POST requests
- Inspecting generated requests
- Viewing response bodies
- Viewing HTTP status codes

The generated OpenAPI document is available at:

```text
/openapi/v1.json
```

---

# Testing with cURL

Swagger UI is the easiest way to explore the API, but the endpoints can also be tested directly.

## Get all robots

```powershell
curl.exe http://localhost:5244/api/robots
```

## Filter by status

```powershell
curl.exe "http://localhost:5244/api/robots?status=Operational"
```

## Pagination

```powershell
curl.exe "http://localhost:5244/api/robots?page=1&pageSize=2"
```

## Get robot by ID

```powershell
curl.exe -i http://localhost:5244/api/robots/3
```

## Create a robot

PowerShell example:

```powershell
curl.exe -i -X POST http://localhost:5244/api/robots `
  -H "Content-Type: application/json" `
  -d '{\"name\":\"Sentinel\",\"model\":\"ST-9\",\"status\":\"Operational\",\"lastMaintenance\":\"2026-08-10T00:00:00\",\"nextMaintenance\":\"2026-12-10T00:00:00\"}'
```

---

# HTTP Status Codes

The API currently uses:

| Status | Meaning |
|---|---|
| `200 OK` | Resource successfully retrieved |
| `201 Created` | New robot successfully created |
| `400 Bad Request` | Invalid input or query parameters |
| `404 Not Found` | Requested robot does not exist |

---

# Database Development

When the model changes, a new EF Core migration can be created with:

```powershell
dotnet ef migrations add MigrationName
```

Apply pending migrations with:

```powershell
dotnet ef database update
```

The migration files are committed to Git so another developer can recreate the database schema locally.

SQLite database files are intentionally excluded through `.gitignore`.

---

# Possible Future Improvements

Possible extensions include:

- PUT/PATCH endpoint for updating robots
- DELETE endpoint
- Additional filtering and sorting
- More advanced maintenance history
- Repository layer
- Automated xUnit tests
- Integration tests
- SQL Server or PostgreSQL
- Authentication and authorization
- Docker support

These are outside the current project scope.

---

## Assignment Goals Covered

This project demonstrates:

- A meaningful domain model
- ASP.NET Core Controllers
- GET endpoints
- POST endpoint
- Filtering
- Pagination
- Input validation
- HTTP status codes
- ProblemDetails error responses
- Asynchronous method design
- Service layer
- Dependency injection
- DTO usage
- Entity Framework Core
- SQL-backed persistence with SQLite
- EF Core migrations
- Seed data
- OpenAPI documentation
- Swagger UI
- Manual API verification

The project now covers both the core assignment requirements and several of the optional extensions.