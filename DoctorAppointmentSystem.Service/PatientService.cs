using DoctorAppointmentSystem.Model;
using System.Net.Http.Json;

namespace DoctorAppointmentSystem.Service;
public class PatientService
{
    private readonly HttpClient _httpClient;

    public PatientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Patient> GetPatientByTcAsync(string tc)
    {
        var response = await _httpClient.GetAsync($"api/patient/search-by-tc/{tc}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Patient>();
        }
        return null; 
    }

    public async Task<Patient> GetPatientByIdAsync(string id)
    {
        var response = await _httpClient.GetAsync($"api/patient/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Patient>();
        }
        return null;  
    }

    public async Task<Patient> CreatePatientAsync(Patient patient)
    {
        var response = await _httpClient.PostAsJsonAsync("api/patient", patient);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Patient>();
        }
        return null;  
    }
}