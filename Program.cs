using Microsoft.EntityFrameworkCore;
using RobotMaintenanceApi.Data;
using RobotMaintenanceApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers.
builder.Services.AddControllers();

// Add OpenAPI document generation.
builder.Services.AddOpenApi();

string connectionString =
    builder.Configuration.GetConnectionString("RobotDatabase")
    ?? throw new InvalidOperationException(
        "Connection string 'RobotDatabase' was not configured.");

// Add EF Core with PostgreSQL.
builder.Services.AddDbContext<RobotDbContext>(options =>
    options.UseNpgsql(
        connectionString,
        postgreSqlOptions =>
            postgreSqlOptions.EnableRetryOnFailure()));

// Register RobotService.
//
// Scoped is used because both RobotService and RobotDbContext
// should exist for the lifetime of a single request.
builder.Services.AddScoped<IRobotService, RobotService>();

builder.Services.AddHealthChecks();

var app = builder.Build();

// Apply pending database migrations when the API starts.
using (IServiceScope scope = app.Services.CreateScope())
{
    RobotDbContext dbContext =
        scope.ServiceProvider
            .GetRequiredService<RobotDbContext>();

    await dbContext.Database.MigrateAsync();
}

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

app.MapHealthChecks("/health");

app.Run();