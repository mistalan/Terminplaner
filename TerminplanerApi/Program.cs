using Microsoft.Azure.Cosmos;
using TerminplanerApi.Models;
using TerminplanerApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var repositoryType = builder.Configuration.GetValue<string>("RepositoryType") ?? "InMemory";

if (repositoryType == "CosmosDb")
{
    // Configure Cosmos DB
    var cosmosConnectionString = builder.Configuration.GetValue<string>("CosmosDb:ConnectionString");
    var databaseId = builder.Configuration.GetValue<string>("CosmosDb:DatabaseId");
    var containerId = builder.Configuration.GetValue<string>("CosmosDb:ContainerId");

    if (string.IsNullOrEmpty(cosmosConnectionString) || string.IsNullOrEmpty(databaseId) || string.IsNullOrEmpty(containerId))
    {
        throw new InvalidOperationException("CosmosDb configuration is missing in appsettings.json");
    }

    builder.Services.AddSingleton<CosmosClient>(sp => new CosmosClient(cosmosConnectionString));
    builder.Services.AddSingleton<IAppointmentRepository>(sp =>
    {
        var cosmosClient = sp.GetRequiredService<CosmosClient>();
        return new CosmosAppointmentRepository(cosmosClient, databaseId, containerId);
    });
}
else
{
    // Use in-memory repository (default)
    builder.Services.AddSingleton<IAppointmentRepository, InMemoryAppointmentRepository>();
}

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// API Endpoints
app.MapGet("/api/appointments", async (IAppointmentRepository repository) =>
{
    var appointments = await repository.GetAllAsync();
    return Results.Ok(appointments);
})
.WithName("GetAppointments");

app.MapGet("/api/appointments/{id}", async (string id, IAppointmentRepository repository) =>
{
    var appointment = await repository.GetByIdAsync(id);
    return appointment is not null ? Results.Ok(appointment) : Results.NotFound();
})
.WithName("GetAppointment");

app.MapPost("/api/appointments", async (Appointment appointment, IAppointmentRepository repository) =>
{
    var created = await repository.CreateAsync(appointment);
    return Results.Created($"/api/appointments/{created.Id}", created);
})
.WithName("CreateAppointment");

app.MapPut("/api/appointments/{id}", async (string id, Appointment appointment, IAppointmentRepository repository) =>
{
    var updated = await repository.UpdateAsync(id, appointment);
    return updated is not null ? Results.Ok(updated) : Results.NotFound();
})
.WithName("UpdateAppointment");

app.MapDelete("/api/appointments/{id}", async (string id, IAppointmentRepository repository) =>
{
    var deleted = await repository.DeleteAsync(id);
    return deleted ? Results.NoContent() : Results.NotFound();
})
.WithName("DeleteAppointment");

app.MapPut("/api/appointments/priorities", async (Dictionary<string, int> priorities, IAppointmentRepository repository) =>
{
    await repository.UpdatePrioritiesAsync(priorities);
    return Results.Ok();
})
.WithName("UpdatePriorities");

app.Run();

// Make Program class accessible for integration tests
public partial class Program { }
