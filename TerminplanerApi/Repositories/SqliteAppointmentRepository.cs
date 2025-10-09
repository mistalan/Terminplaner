using Microsoft.Data.Sqlite;
using System.Text.Json;
using TerminplanerApi.Models;

namespace TerminplanerApi.Repositories;

public class SqliteAppointmentRepository : IAppointmentRepository
{
    private readonly string _connectionString;

    public SqliteAppointmentRepository(string connectionString)
    {
        _connectionString = connectionString;
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Appointments (
                Id TEXT PRIMARY KEY,
                Text TEXT NOT NULL,
                Category TEXT NOT NULL,
                Color TEXT NOT NULL,
                Priority INTEGER NOT NULL,
                CreatedAt TEXT NOT NULL,
                ScheduledDate TEXT,
                Duration TEXT,
                IsOutOfHome INTEGER NOT NULL
            )
        ";
        command.ExecuteNonQuery();
    }

    public async Task<Appointment> CreateAsync(Appointment appointment)
    {
        // Generate a new ID if not provided
        if (string.IsNullOrEmpty(appointment.Id))
        {
            appointment.Id = Guid.NewGuid().ToString();
        }

        // Only set CreatedAt if not already set (to preserve during sync)
        if (appointment.CreatedAt == default || appointment.CreatedAt == DateTime.MinValue)
        {
            appointment.CreatedAt = DateTime.UtcNow;
        }

        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Appointments (Id, Text, Category, Color, Priority, CreatedAt, ScheduledDate, Duration, IsOutOfHome)
            VALUES ($id, $text, $category, $color, $priority, $createdAt, $scheduledDate, $duration, $isOutOfHome)
        ";

        command.Parameters.AddWithValue("$id", appointment.Id);
        command.Parameters.AddWithValue("$text", appointment.Text);
        command.Parameters.AddWithValue("$category", appointment.Category);
        command.Parameters.AddWithValue("$color", appointment.Color);
        command.Parameters.AddWithValue("$priority", appointment.Priority);
        command.Parameters.AddWithValue("$createdAt", appointment.CreatedAt.ToString("O"));
        command.Parameters.AddWithValue("$scheduledDate", appointment.ScheduledDate?.ToString("O") ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("$duration", appointment.Duration ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("$isOutOfHome", appointment.IsOutOfHome ? 1 : 0);

        await command.ExecuteNonQueryAsync();

        return appointment;
    }

    public async Task<Appointment?> GetByIdAsync(string id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT Id, Text, Category, Color, Priority, CreatedAt, ScheduledDate, Duration, IsOutOfHome
            FROM Appointments
            WHERE Id = $id
        ";
        command.Parameters.AddWithValue("$id", id);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return ReadAppointment(reader);
        }

        return null;
    }

    public async Task<List<Appointment>> GetAllAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT Id, Text, Category, Color, Priority, CreatedAt, ScheduledDate, Duration, IsOutOfHome
            FROM Appointments
            ORDER BY Priority
        ";

        var appointments = new List<Appointment>();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            appointments.Add(ReadAppointment(reader));
        }

        return appointments;
    }

    public async Task<Appointment?> UpdateAsync(string id, Appointment updatedAppointment)
    {
        var existing = await GetByIdAsync(id);
        if (existing == null) return null;

        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Appointments
            SET Text = $text,
                Category = $category,
                Color = $color,
                Priority = $priority,
                ScheduledDate = $scheduledDate,
                Duration = $duration,
                IsOutOfHome = $isOutOfHome
            WHERE Id = $id
        ";

        command.Parameters.AddWithValue("$id", id);
        command.Parameters.AddWithValue("$text", updatedAppointment.Text);
        command.Parameters.AddWithValue("$category", updatedAppointment.Category);
        command.Parameters.AddWithValue("$color", updatedAppointment.Color);
        command.Parameters.AddWithValue("$priority", updatedAppointment.Priority);
        command.Parameters.AddWithValue("$scheduledDate", updatedAppointment.ScheduledDate?.ToString("O") ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("$duration", updatedAppointment.Duration ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("$isOutOfHome", updatedAppointment.IsOutOfHome ? 1 : 0);

        await command.ExecuteNonQueryAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            DELETE FROM Appointments
            WHERE Id = $id
        ";
        command.Parameters.AddWithValue("$id", id);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task UpdatePrioritiesAsync(Dictionary<string, int> priorities)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();

        try
        {
            foreach (var kvp in priorities)
            {
                var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = @"
                    UPDATE Appointments
                    SET Priority = $priority
                    WHERE Id = $id
                ";
                command.Parameters.AddWithValue("$id", kvp.Key);
                command.Parameters.AddWithValue("$priority", kvp.Value);

                await command.ExecuteNonQueryAsync();
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private static Appointment ReadAppointment(SqliteDataReader reader)
    {
        return new Appointment
        {
            Id = reader.GetString(0),
            Text = reader.GetString(1),
            Category = reader.GetString(2),
            Color = reader.GetString(3),
            Priority = reader.GetInt32(4),
            CreatedAt = DateTime.Parse(reader.GetString(5)),
            ScheduledDate = reader.IsDBNull(6) ? null : DateTime.Parse(reader.GetString(6)),
            Duration = reader.IsDBNull(7) ? null : reader.GetString(7),
            IsOutOfHome = reader.GetInt32(8) == 1
        };
    }
}
