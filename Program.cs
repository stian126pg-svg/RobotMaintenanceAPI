using Microsoft.EntityFrameworkCore;
using RobotMaintenanceApi.Data;
using RobotMaintenanceApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers.
builder.Services.AddControllers();

// Add OpenAPI document generation.
builder.Services.AddOpenApi();

// Add EF Core with SQLite.
builder.Services.AddDbContext<RobotDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("RobotDatabase")));

// Register RobotService.
//
// Scoped is used instead of Singleton because RobotService
// will depend on the scoped RobotDbContext.
builder.Services.AddScoped<IRobotService, RobotService>();

var app = builder.Build();

// Development-only API documentation.
if (app.Environment.IsDevelopment())
{
    // Generates /openapi/v1.json
    app.MapOpenApi();

    // Interactive Swagger UI.
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "Robot Maintenance API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();