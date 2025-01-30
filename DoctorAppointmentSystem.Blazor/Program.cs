using Blazored.LocalStorage;
using Blazored.Toast;
using DoctorAppointmentSystem.Blazor;
using DoctorAppointmentSystem.Service;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
var baseAddress = builder.Configuration.GetValue<string>("BaseAddress")!;
builder.Services.AddHttpClient<DoctorService>(client => client.BaseAddress = new Uri(baseAddress));
builder.Services.AddHttpClient<PharmacyService>(client => client.BaseAddress = new Uri(baseAddress));
builder.Services.AddHttpClient<PatientService>(client => client.BaseAddress = new Uri(baseAddress)); 
builder.Services.AddHttpClient<PrescriptionService>(client => client.BaseAddress = new Uri(baseAddress)); 
builder.Services.AddHttpClient<MedicineService>(client => client.BaseAddress = new Uri(baseAddress));
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredToast();
await builder.Build().RunAsync();
