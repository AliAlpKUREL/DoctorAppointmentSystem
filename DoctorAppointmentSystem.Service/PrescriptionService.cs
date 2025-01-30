using DoctorAppointmentSystem.Model;
using MongoDB.Bson.IO;
using System.Net.Http;
using System.Net.Http.Json;

namespace DoctorAppointmentSystem.Service;
public class PrescriptionService
{
    private readonly HttpClient _httpClient;

    public PrescriptionService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Prescription>> GetPrescriptionsAsync()
    {
        var response = await _httpClient.GetAsync("api/prescription");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<Prescription>>();
        }
        return new List<Prescription>(); 
    }

    public async Task<Prescription> GetPrescriptionByIdAsync(string id)
    {
        var response = await _httpClient.GetAsync($"api/prescription/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Prescription>();
        }
        return null;  
    }

    public async Task<Prescription> CreatePrescriptionAsync(string token, Prescription prescription)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.PostAsJsonAsync("api/prescription", prescription);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Prescription>();
        }
        return null;
    }

    public async Task<Prescription> UpdatePrescriptionAsync(string token, string id, Prescription prescription)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.PutAsJsonAsync($"api/prescription/{id}", prescription);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Prescription>();
        }
        return null;
    }

    public async Task<bool> DeletePrescriptionAsync(string id)
    {
        var response = await _httpClient.DeleteAsync($"api/prescription/{id}");
        return response.IsSuccessStatusCode; 
    }
}