using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DoctorAppointmentSystem.Model;

public class Doctor
{
    [BsonId]
    [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    [BsonElement("fullname")]
    public string FullName { get; set; }
    
    [BsonElement("specialty")]
    public string Specialty { get; set; }

    [BsonElement("email")]
    public string Email { get; set; }
    
    [BsonElement("password_hash")]
    public string PasswordHash { get; set; }

    [BsonElement("patient")]
    public List<string> Patients { get; set; } = [];
}
