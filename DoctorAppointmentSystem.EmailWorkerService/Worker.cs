using DoctorAppointmentSystem.Common;
using DoctorAppointmentSystem.Model;
using DoctorAppointmentSystem.Service;
using MongoDB.Bson.IO;
using Newtonsoft.Json;

namespace DoctorAppointmentSystem.EmailWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly EmailService _emailService;
        private readonly PharmacyService _pharmacyService;
        private readonly RabbitMqService _rabbitMqService;

        public Worker(
            ILogger<Worker> logger,
            EmailService emailService,
            PharmacyService pharmacyService,
            RabbitMqService rabbitMqService)
        {
            _logger = logger;
            _emailService = emailService;
            _pharmacyService = pharmacyService;
            _rabbitMqService = rabbitMqService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;

                //if (now.Hour == 1 && now.Minute == 0)
                {
                    _rabbitMqService.ConsumeMessages(async message =>
                    {
                        var prescription = Newtonsoft.Json.JsonConvert.DeserializeObject<Prescription>(message);
                        if (prescription != null && !prescription.IsCompleted)
                        {
                            await SendEmailIfNeeded(prescription);
                        }
                    });
                }
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        private async Task SendEmailIfNeeded(Prescription prescription)
        {
            try
            {
                var missingMedicines = prescription.Medicines
                                .Select(m => m.Name)
                                .ToList();

                if (missingMedicines.Count == 0) return;

                var pharmacy = await _pharmacyService.GetPharmacyByIdAsync(prescription.Id);
                if (pharmacy == null) return;

                string emailBody = $"Merhaba {pharmacy.Name},\n\n" +
                                   $"Reçete ID: {prescription.PrescriptionId} için eksik ilaçlar:\n" +
                                   $"{string.Join(", ", missingMedicines)}\n\n";

                await _emailService.SendEmailAsync(pharmacy.Email, "Eksik Reçeteler", emailBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing incomplete prescriptions.");
            }
        }
    }
}
