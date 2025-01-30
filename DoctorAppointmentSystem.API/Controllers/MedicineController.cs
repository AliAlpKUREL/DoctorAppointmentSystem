using DoctorAppointmentSystem.Common;
using DoctorAppointmentSystem.API.Data;
using DoctorAppointmentSystem.Model;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace DoctorAppointmentSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicineController : ControllerBase
    {
        private readonly IMongoCollection<Medicine> _medicineCollection;
        private readonly RedisCacheService _redisCacheService;

        public MedicineController(MongoDbService mongoDbService, RedisCacheService redisCacheService)
        {
            _medicineCollection = mongoDbService.Medicines;
            _redisCacheService = redisCacheService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Medicine>>> GetMedicines()
        {
            string cacheKey = "medicines";

            var cachedMedicines = _redisCacheService.GetCache(cacheKey);
            if (cachedMedicines is not null || !cachedMedicines.Any())
            {
                return Ok(Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Medicine>>(cachedMedicines));
            }

            var medicines = await _medicineCollection.Find(_ => true).ToListAsync();
            _redisCacheService.SetCache(cacheKey, Newtonsoft.Json.JsonConvert.SerializeObject(medicines), TimeSpan.FromMinutes(10));
            return Ok(medicines);
        }

        [HttpGet("search-by-name/{name}")]
        public async Task<ActionResult<IEnumerable<Medicine>>> GetMedicinesByName(string name)
        {
            var filter = Builders<Medicine>.Filter.Regex("Name", new BsonRegularExpression(new Regex(name, RegexOptions.IgnoreCase)));
            var medicines = await _medicineCollection.Find(filter).ToListAsync();

            if (medicines == null || medicines.Count == 0)
            {
                return NotFound("İlaçlar bulunamadı.");
            }

            return Ok(medicines); 
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Medicine>> GetById(string id)
        {
            var medicine = await _medicineCollection.Find(m => m.Id == id).FirstOrDefaultAsync();
            return medicine != null ? Ok(medicine) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<Medicine>> CreateMedicine(Medicine medicine)
        {
            await _medicineCollection.InsertOneAsync(medicine);

            string cacheKey = "medicines";
            var cachedMedicines = _redisCacheService.GetCache(cacheKey);

            if (cachedMedicines is not null)
            {
                var medicines = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Medicine>>(cachedMedicines);

                medicines.Add(medicine);

                _redisCacheService.SetCache(cacheKey, Newtonsoft.Json.JsonConvert.SerializeObject(medicines), TimeSpan.FromMinutes(10));
            }
            else
            {
                var medicines = new List<Medicine> { medicine };

                _redisCacheService.SetCache(cacheKey, Newtonsoft.Json.JsonConvert.SerializeObject(medicines), TimeSpan.FromMinutes(10));
            }

            return CreatedAtAction(nameof(GetMedicines), new { id = medicine.Id }, medicine);
        }
    }
}
