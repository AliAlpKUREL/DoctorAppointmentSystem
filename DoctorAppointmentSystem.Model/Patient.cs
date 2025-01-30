using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DoctorAppointmentSystem.Model;

public class Patient
{
    [BsonId]
    [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("tc_id")]
    public string TCId { get; set; }
    
    [BsonElement("fullname")]
    public string FullName { get; set; }

    [BsonElement("prescriptions")]
    public List<string> Prescriptions { get; set; } = [];
}
