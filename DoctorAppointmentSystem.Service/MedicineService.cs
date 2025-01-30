using DoctorAppointmentSystem.Model;
using System.Net.Http.Json;

namespace DoctorAppointmentSystem.Service;

public class MedicineService
{
    private readonly HttpClient _httpClient;

    public MedicineService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Medicine>> GetMedicinesAsync()
    {
       
        var response = await _httpClient.GetAsync("api/medicine");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<IEnumerable<Medicine>>();
        }
        return null;
    }

    public async Task<IEnumerable<Medicine>> GetMedicinesByNameAsync(string name)
    {

        var response = await _httpClient.GetAsync($"api/medicine/search-by-name/{name}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<IEnumerable<Medicine>>();
        }
        return null;
    }

    public async Task<Medicine> GetMedicineByIdAsync(string id)
    {

        var response = await _httpClient.GetAsync($"api/medicine/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Medicine>();
        }
        return null;
    }

    public async Task<Medicine> CreateMedicineAsync(Medicine medicine)
    {

        var response = await _httpClient.PostAsJsonAsync("api/medicine", medicine);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Medicine>();
        }
        return null;
    }
}