using TerminplanerApi.Configuration;
using TerminplanerApi.Models;
using TerminplanerApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAppointmentRepository(builder.Configuration);

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

// Perform initial sync for Hybrid repository
await app.InitializeRepositoryAsync(builder.Configuration);

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
