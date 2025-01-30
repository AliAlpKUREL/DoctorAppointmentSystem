using DoctorAppointmentSystem.API.Data;
using DoctorAppointmentSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoctorAppointmentSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IMongoCollection<Patient> _patientCollection;

        public PatientController(MongoDbService mongoDbService)
        {
            _patientCollection = mongoDbService.Patients;
        }

        [HttpGet("search-by-tc/{tc}")]
        public async Task<ActionResult<Patient>> GetPatientByTc(string tc)
        {
            var filter = Builders<Patient>.Filter.Eq(p => p.TCId, tc);
            var patient = await _patientCollection.Find(filter).FirstOrDefaultAsync();
            var patients = await _patientCollection.Find(_ => true).ToListAsync();
            if (patient == null)
            {
                return NotFound("Hasta bulunamadı.");
            }

            return Ok(patient);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetById(string id)
        {
            var patient = await _patientCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
            return patient != null ? Ok(patient) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Patient patient)
        {
            await _patientCollection.InsertOneAsync(patient);
            return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
        }
    }
}
