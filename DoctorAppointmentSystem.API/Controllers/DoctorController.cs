using DoctorAppointmentSystem.API.Data;
using DoctorAppointmentSystem.API.Services;
using DoctorAppointmentSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DoctorAppointmentSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorController : ControllerBase
    {
        private readonly IMongoCollection<Doctor> _doctorCollection;
        private readonly JwtService _jwtService;

        public DoctorController(MongoDbService mongoDbService, JwtService jwtService)
        {
            _doctorCollection = mongoDbService.Doctors;
            _jwtService = jwtService;
        }

        [HttpGet]
        public async Task<ActionResult<Doctor>> GetById()
        {
            var doctorId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;


            if (string.IsNullOrEmpty(doctorId))
            {
                return Unauthorized("Geçerli bir doktor kimliği bulunamadı.");
            }
            var doctor = await _doctorCollection.Find(d => d.Id == doctorId).FirstOrDefaultAsync();
            return doctor != null ? Ok(doctor) : NotFound();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel request)
        {
            var doctor = await _doctorCollection.Find(d => d.Email == request.Email && d.PasswordHash == request.Password).FirstOrDefaultAsync();
            if (doctor == null)
            {
                return Unauthorized();
            }

            var token = _jwtService.GenerateJwtToken(doctor); 
            return Ok(new { token });
        }

        [HttpPost("add-patient-to-doctor")]
        [Authorize(Roles = "Doctor")] 
        public async Task<IActionResult> AddPatientToDoctor([FromBody] string patientId)
        {
            var doctorId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(doctorId))
            {
                return Unauthorized("Geçerli bir doktor kimliği bulunamadı.");
            }

            var doctorFilter = Builders<Doctor>.Filter.Eq(d => d.Id, doctorId);
            var update = Builders<Doctor>.Update.AddToSet(d => d.Patients, patientId);

            var result = await _doctorCollection.UpdateOneAsync(doctorFilter, update);

            if (result.ModifiedCount > 0)
            {
                return Ok("Hasta başarıyla doktora eklendi.");
            }

            return BadRequest("Hasta eklenirken bir hata oluştu.");
        }
    }

  
}
