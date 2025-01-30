using DoctorAppointmentSystem.Model;
using System.Net.Http.Json;

namespace DoctorAppointmentSystem.Service;

public class PharmacyService
{
    private readonly HttpClient _httpClient;

    public PharmacyService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Pharmacy>> GetPharmaciesAsync()
    {
        var response = await _httpClient.GetAsync("api/pharmacy");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<Pharmacy>>();
        }
        return new List<Pharmacy>(); 
    }

    public async Task<Pharmacy> GetPharmacyByIdAsync(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.GetAsync($"api/pharmacy/");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Pharmacy>();
        }
        return null; 
    }

    public async Task<Pharmacy> CreatePharmacyAsync(Pharmacy pharmacy)
    {
        var response = await _httpClient.PostAsJsonAsync("api/pharmacy", pharmacy);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Pharmacy>();
        }
        return null;  
    }

    public async Task<string> LoginAsync(LoginModel loginModel)
    {
        var response = await _httpClient.PostAsJsonAsync("api/pharmacy/login", loginModel);
        if (response.IsSuccessStatusCode)
        {
            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
            return tokenResponse?.Token;
        }
        return null;
    }
}
