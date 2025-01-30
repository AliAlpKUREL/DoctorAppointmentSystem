using DoctorAppointmentSystem.Common;
using DoctorAppointmentSystem.EmailWorkerService;
using DoctorAppointmentSystem.Service;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<RabbitMqService>();

builder.Services.AddHttpClient<PrescriptionService>(client =>
{
    var baseAddress = builder.Configuration.GetValue<string>("BaseAddress")!;
    client.BaseAddress = new Uri(baseAddress);
});

builder.Services.AddHttpClient<PharmacyService>(client =>
{
    var baseAddress = builder.Configuration.GetValue<string>("BaseAddress")!;
    client.BaseAddress = new Uri(baseAddress);
});
builder.Services.AddSingleton<EmailService>();

var host = builder.Build();
host.Run();
