using DoctorAppointmentSystem.Model;
using System.Net.Http.Json;

namespace DoctorAppointmentSystem.Service;
public class DoctorService
{
    private readonly HttpClient _httpClient;

    public DoctorService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Doctor> GetDoctorByIdAsync(string token)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/doctor");
        requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(requestMessage);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Doctor>();
        }

        return null;
    }

    public async Task<string> LoginAsync(LoginModel loginModel)
    {
        var response = await _httpClient.PostAsJsonAsync("api/doctor/login", loginModel);
        if (response.IsSuccessStatusCode)
        {
            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
            return tokenResponse?.Token;  
        }
        return null;  
    }

    public async Task<bool> AddPatientToDoctorAsync(string token, string patientId)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.PostAsJsonAsync($"api/doctor/add-patient-to-doctor", patientId);
        return response.IsSuccessStatusCode;
    }
}
