using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using RobotMaintenanceApi.Data;
using RobotMaintenanceApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add controllers.
builder.Services.AddControllers();

// Add JWT bearer authentication.
//
// The bearer handler validates incoming JWTs and creates
// the authenticated user from the token's claims.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Keep original JWT claim names, such as "sub".
        options.MapInboundClaims = false;
    });

// Add authorization services.
builder.Services.AddAuthorization();

// Add OpenAPI document generation and JWT support for Swagger.
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

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

// Register health checks for monitoring the API.
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
    // Generates /openapi/v1.json.
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

// Authentication must run before authorization.
// First establish who the user is, then check what they may access.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Expose the API health status at /health.
app.MapHealthChecks("/health");

app.Run();

internal sealed class BearerSecuritySchemeTransformer(
    IAuthenticationSchemeProvider authenticationSchemeProvider)
    : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var authenticationSchemes =
            await authenticationSchemeProvider.GetAllSchemesAsync();

        bool bearerIsRegistered = authenticationSchemes.Any(
            scheme =>
                scheme.Name == JwtBearerDefaults.AuthenticationScheme);

        if (!bearerIsRegistered)
        {
            return;
        }

        document.Components ??= new OpenApiComponents();

        document.Components.SecuritySchemes =
            new Dictionary<string, IOpenApiSecurityScheme>
            {
                ["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    In = ParameterLocation.Header,
                    BearerFormat = "JWT",
                    Description =
                        "Enter the JWT without the 'Bearer' prefix."
                }
            };

        foreach (var path in document.Paths.Values)
        {
            if (path.Operations is null)
            {
                continue;
            }

            foreach (var operation in path.Operations)
            {
                operation.Value.Security ??= [];

                operation.Value.Security.Add(
                    new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference(
                            "Bearer",
                            document)] = []
                    });
            }
        }
    }
}