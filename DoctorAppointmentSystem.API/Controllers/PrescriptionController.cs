using DoctorAppointmentSystem.Common;
using DoctorAppointmentSystem.API.Data;
using DoctorAppointmentSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace DoctorAppointmentSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrescriptionController : ControllerBase
    {
        private readonly IMongoCollection<Prescription> _prescriptionCollection;
        private readonly RabbitMqService _rabbitMqService;

        public PrescriptionController(MongoDbService mongoDbService, RabbitMqService rabbitMqService)
        {
            _prescriptionCollection = mongoDbService.Prescriptions;
            _rabbitMqService = rabbitMqService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Prescription>>> GetPrescriptions()
        {
            var prescriptions = await _prescriptionCollection.Find(_ => true).ToListAsync();
            return Ok(prescriptions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Prescription>> GetById(string id)
        {
            var prescription = await _prescriptionCollection.Find(p => p.PrescriptionId == id).FirstOrDefaultAsync();
            return prescription != null ? Ok(prescription) : NotFound("Reçete bulunamadı.");
        }

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Create(Prescription prescription)
        {
            await _prescriptionCollection.InsertOneAsync(prescription);

            return CreatedAtAction(nameof(GetById), new { id = prescription.Id }, prescription);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Pharmacy")]
        public async Task<IActionResult> Update(string id, [FromBody] Prescription prescription)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Reçete ID boş olamaz.");

            if (prescription == null)
                return BadRequest("Güncelleme için reçete verisi boş olamaz.");

            var filter = Builders<Prescription>.Filter.Eq(p => p.Id, id);
            var update = Builders<Prescription>.Update
                .Set(p => p.IsCompleted, prescription.IsCompleted)
                .Set(p => p.Medicines, prescription.Medicines);
            var updatedPrescription = await _prescriptionCollection.FindOneAndUpdateAsync(
                   filter,
                   update,
                   new FindOneAndUpdateOptions<Prescription> { ReturnDocument = ReturnDocument.After }
               );

            if (updatedPrescription == null)
                return NotFound("Reçete güncellenemedi, ID kontrol edin.");

            var message = JsonConvert.SerializeObject(updatedPrescription);
            _rabbitMqService.SendMessage(message);

            return Ok(updatedPrescription);
        }

    }
}
