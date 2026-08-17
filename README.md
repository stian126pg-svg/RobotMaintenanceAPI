# Robot Maintenance API

A small REST API built with ASP.NET Core Controllers for managing robot maintenance information.

The project was created as a backend development assignment focused on REST APIs, controllers, asynchronous method design, validation, HTTP status codes, dependency injection, and API documentation.

## Features

* Retrieve all robots
* Retrieve a robot by ID
* Filter robots by status
* Paginate GET results
* Create new robots
* Validate incoming request data
* Return appropriate HTTP status codes
* Structured Problem Details error responses
* OpenAPI documentation
* In-memory data storage
* Task-based service methods prepared for later database integration

## Project Structure

```text
RobotMaintenanceAPI/
│
├── Controllers/
│   └── RobotsController.cs
│
├── Dtos/
│   └── CreateRobotRequest.cs
│
├── Model/
│   └── Robot.cs
│
├── Services/
│   ├── IRobotService.cs
│   └── RobotService.cs
│
├── Program.cs
├── RobotMaintenanceAPI.csproj
└── README.md
```

## Model

A robot contains the following properties:

```text
Id
Name
Model
Status
LastMaintenance
NextMaintenance
```

Supported statuses are:

```text
Operational
NeedsMaintenance
OutOfService
```

`NextMaintenance` may be null if no future maintenance date has been scheduled.

## API Endpoints

### GET `/api/Robots`

Returns a list of robots.

Example:

```bash
curl http://localhost:5244/api/robots
```

Successful response:

```text
200 OK
```

### Filtering

The list can be filtered by status.

Example:

```bash
curl "http://localhost:5244/api/robots?status=Operational"
```

Status filtering is case-insensitive.

For example, this also works:

```bash
curl "http://localhost:5244/api/robots?status=oPeRaTiOnAl"
```

### Pagination

The endpoint supports `page` and `pageSize` query parameters.

Example:

```bash
curl "http://localhost:5244/api/robots?page=2&pageSize=2"
```

Default values are:

```text
page = 1
pageSize = 10
```

A page or page size below `1` returns:

```text
400 Bad Request
```

---

### GET `/api/Robots/{id}`

Returns a single robot by ID.

Example:

```bash
curl http://localhost:5244/api/robots/3
```

If the robot exists:

```text
200 OK
```

If no matching robot exists:

```text
404 Not Found
```

---

### POST `/api/Robots`

Creates a new robot.

Example request:

```bash
curl -X POST http://localhost:5244/api/robots \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Sentinel",
    "model": "ST-9",
    "status": "Operational",
    "lastMaintenance": "2026-08-10T00:00:00",
    "nextMaintenance": "2026-12-10T00:00:00"
  }'
```

The client does not provide the robot ID. The service assigns the next available ID.

A successful request returns:

```text
201 Created
```

The response includes the created robot and a `Location` header pointing to the new resource.

Example:

```text
Location: http://localhost:5244/api/Robots/5
```

## Validation

`CreateRobotRequest` is used as a DTO for incoming POST requests.

This keeps the API input model separate from the stored `Robot` model and prevents clients from controlling properties such as the robot ID.

Data Annotations are used for basic validation:

```csharp
[Required]
[StringLength(100)]
```

`Name`, `Model`, and `Status` are required.

`Name` and `Model` have a maximum length of 100 characters.

The API also performs domain validation for robot status.

Only the following statuses are accepted:

```text
Operational
NeedsMaintenance
OutOfService
```

Invalid requests return:

```text
400 Bad Request
```

Errors are returned using ASP.NET Core Problem Details responses.

## HTTP Status Codes

The API uses the following status codes:

```text
200 OK
Successful GET request

201 Created
Robot successfully created

400 Bad Request
Invalid query parameters or request data

404 Not Found
Requested robot does not exist
```

## Asynchronous Design

Controller actions and service methods use Task-based asynchronous method signatures.

Example:

```csharp
Task<Robot?> GetByIdAsync(int id);
```

The current implementation stores robots in memory, so there is no real I/O operation to await.

For this reason, the service currently returns completed tasks using:

```csharp
Task.FromResult(...)
```

This keeps the application contract asynchronous without wrapping synchronous work in unnecessary `Task.Run` calls.

If the service is later changed to use a database, these methods can be replaced with genuine asynchronous database operations such as:

```csharp
await context.Robots.FirstOrDefaultAsync(...);
```

This allows asynchronous behavior to propagate through the application:

```text
Controller
    ↓
Service
    ↓
Database
```

The project does not use `.Result` or `.Wait()` in asynchronous code.

## Dependency Injection

The controller depends on the `IRobotService` interface instead of directly creating a `RobotService`.

The service is registered in `Program.cs`:

```csharp
builder.Services.AddSingleton<IRobotService, RobotService>();
```

ASP.NET Core's dependency injection system supplies the service when constructing `RobotsController`.

The singleton lifetime is used because the current service stores data in memory and the same collection needs to remain available between HTTP requests.

## Data Storage

The current implementation uses an in-memory:

```csharp
List<Robot>
```

The application starts with several seeded robots.

Because the storage is in memory, robots created with POST only exist while the application is running.

Restarting the application resets the data to the original seeded values.

## Future SQL / Entity Framework Core Support

The project has been structured so that the current in-memory implementation can later be replaced with database-backed storage.

A possible future architecture is:

```text
Controller
    ↓
Service
    ↓
Entity Framework Core
    ↓
SQL Database
```

A possible SQL table could contain:

```text
Robots

Id               Primary Key
Name             Required
Model            Required
Status           Required
LastMaintenance  Required
NextMaintenance  Nullable
```

Entity Framework Core could be used to map the `Robot` model to the database and perform asynchronous database operations.

Potential database methods could use:

```csharp
ToListAsync()
FirstOrDefaultAsync()
AddAsync()
SaveChangesAsync()
```

Database configuration and migrations are not included in the current version.

## OpenAPI

The project uses ASP.NET Core's built-in OpenAPI support.

When running in the Development environment, the generated OpenAPI document is available at:

```text
http://localhost:5244/openapi/v1.json
```

The document describes:

```text
GET  /api/Robots
GET  /api/Robots/{id}
POST /api/Robots
```

including request schemas and expected HTTP response codes.

## Running the Project

Requirements:

```text
.NET 10 SDK
```

From the project directory:

```bash
dotnet restore
dotnet build
dotnet run
```

The exact local port is shown in the terminal after startup.

Example:

```text
Now listening on: http://localhost:5244
```

Use the port shown by the application if it differs from the examples in this README.

## Manual Testing

The API was manually verified using cURL.

Example commands:

```bash
curl http://localhost:5244/api/robots
```

```bash
curl http://localhost:5244/api/robots/3
```

```bash
curl "http://localhost:5244/api/robots?status=Operational"
```

```bash
curl "http://localhost:5244/api/robots?page=1&pageSize=2"
```

Using `-i` also displays HTTP response headers:

```bash
curl -i http://localhost:5244/api/robots/3
```

Manual testing verified successful responses as well as validation errors, `404 Not Found`, pagination errors, and `201 Created` responses.
