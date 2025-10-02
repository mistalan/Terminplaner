using System.Net.Http.Json;
using System.Text.Json;
using TerminplanerMaui.Models;

namespace TerminplanerMaui.Services;

public class AppointmentApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public AppointmentApiService()
    {
        _httpClient = new HttpClient();
        // For Android emulator, use 10.0.2.2 instead of localhost
        // For iOS simulator, use localhost
        // For Windows/Desktop, use localhost
        _baseUrl = DeviceInfo.Platform == DevicePlatform.Android 
            ? "http://10.0.2.2:5215" 
            : "http://localhost:5215";
    }

    public async Task<List<Appointment>> GetAllAppointmentsAsync()
    {
        try
        {
            var appointments = await _httpClient.GetFromJsonAsync<List<Appointment>>($"{_baseUrl}/api/appointments");
            return appointments ?? new List<Appointment>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching appointments: {ex.Message}");
            return new List<Appointment>();
        }
    }

    public async Task<Appointment?> GetAppointmentAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Appointment>($"{_baseUrl}/api/appointments/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching appointment: {ex.Message}");
            return null;
        }
    }

    public async Task<Appointment?> CreateAppointmentAsync(Appointment appointment)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/appointments", appointment);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Appointment>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating appointment: {ex.Message}");
            return null;
        }
    }

    public async Task<Appointment?> UpdateAppointmentAsync(int id, Appointment appointment)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/api/appointments/{id}", appointment);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Appointment>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating appointment: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteAppointmentAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/api/appointments/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting appointment: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdatePrioritiesAsync(Dictionary<int, int> priorities)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/api/appointments/priorities", priorities);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating priorities: {ex.Message}");
            return false;
        }
    }
}
