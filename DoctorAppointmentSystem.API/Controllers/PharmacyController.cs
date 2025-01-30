using DoctorAppointmentSystem.API.Data;
using DoctorAppointmentSystem.API.Services;
using DoctorAppointmentSystem.Model;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DoctorAppointmentSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PharmacyController : ControllerBase
    {
        private readonly IMongoCollection<Pharmacy> _pharmacyCollection;
        private readonly JwtService _jwtService;

        public PharmacyController(MongoDbService mongoDbService, JwtService jwtService)
        {
            _pharmacyCollection = mongoDbService.Pharmacies;
            _jwtService = jwtService;
        }

        //[HttpGet]
        //public async Task<IEnumerable<Pharmacy>> GetPharmacies()
        //{
        //    return await _pharmacyCollection.Find(_ => true).ToListAsync();
        //}

        [HttpGet]
        public async Task<ActionResult<Pharmacy>> GetById()
        {
            var pharmacId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(pharmacId))
            {
                return Unauthorized("Geçerli bir eczacı kimliği bulunamadı.");
            }
            var pharmacy = await _pharmacyCollection.Find(p => p.Id == pharmacId).FirstOrDefaultAsync();
            return pharmacy != null ? Ok(pharmacy) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Pharmacy pharmacy)
        {
            await _pharmacyCollection.InsertOneAsync(pharmacy);
            return CreatedAtAction(nameof(GetById), new { id = pharmacy.Id }, pharmacy);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel request)
        {
            var pharmacy = await _pharmacyCollection.Find(p => p.Email == request.Email && p.PasswordHash == request.Password).FirstOrDefaultAsync();
            if (pharmacy == null)
            {
                return Unauthorized();
            }

            var token = _jwtService.GenerateJwtToken(pharmacy); 
            return Ok(new { token });
        }

    }
}
