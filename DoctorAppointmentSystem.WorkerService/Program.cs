using DoctorAppointmentSystem.Service;
using DoctorAppointmentSystem.WorkerService;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();

builder.Services.AddHttpClient<MedicineService>(client =>
{
    var baseAddress = builder.Configuration.GetValue<string>("BaseAddress")!;
    client.BaseAddress = new Uri(baseAddress);
});
var host = builder.Build();
host.Run();
