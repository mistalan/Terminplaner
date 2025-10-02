using TerminplanerApi.Models;
using TerminplanerApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<AppointmentService>();
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
app.MapGet("/api/appointments", (AppointmentService service) =>
{
    return Results.Ok(service.GetAll());
})
.WithName("GetAppointments");

app.MapGet("/api/appointments/{id}", (int id, AppointmentService service) =>
{
    var appointment = service.GetById(id);
    return appointment is not null ? Results.Ok(appointment) : Results.NotFound();
})
.WithName("GetAppointment");

app.MapPost("/api/appointments", (Appointment appointment, AppointmentService service) =>
{
    var created = service.Create(appointment);
    return Results.Created($"/api/appointments/{created.Id}", created);
})
.WithName("CreateAppointment");

app.MapPut("/api/appointments/{id}", (int id, Appointment appointment, AppointmentService service) =>
{
    var updated = service.Update(id, appointment);
    return updated is not null ? Results.Ok(updated) : Results.NotFound();
})
.WithName("UpdateAppointment");

app.MapDelete("/api/appointments/{id}", (int id, AppointmentService service) =>
{
    var deleted = service.Delete(id);
    return deleted ? Results.NoContent() : Results.NotFound();
})
.WithName("DeleteAppointment");

app.MapPut("/api/appointments/priorities", (Dictionary<int, int> priorities, AppointmentService service) =>
{
    service.UpdatePriorities(priorities);
    return Results.Ok();
})
.WithName("UpdatePriorities");

app.Run();
