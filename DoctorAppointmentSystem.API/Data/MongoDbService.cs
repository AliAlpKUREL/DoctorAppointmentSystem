using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using global::DoctorAppointmentSystem.Model;

namespace DoctorAppointmentSystem.API.Data
{
    public class MongoDbService
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoDatabase _database;

        public MongoDbService(IConfiguration configuration)
        {
            _configuration = configuration;
            var connectionString = _configuration.GetConnectionString("MongoDb");
            var mongoUrl = MongoUrl.Create(connectionString);
            var mongoClient = new MongoClient(mongoUrl);
            _database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }

        public IMongoCollection<Doctor> Doctors => GetCollection<Doctor>("Doctors");
        public IMongoCollection<Patient> Patients => GetCollection<Patient>("Patients");
        public IMongoCollection<Prescription> Prescriptions => GetCollection<Prescription>("Prescriptions");
        public IMongoCollection<Medicine> Medicines => GetCollection<Medicine>("Medicines");
        public IMongoCollection<Pharmacy> Pharmacies => GetCollection<Pharmacy>("Pharmacies");
    }
}

