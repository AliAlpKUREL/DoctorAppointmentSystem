using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DoctorAppointmentSystem.Model;

public class Pharmacy
{
    [BsonId]
    [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("email")]
    public string Email { get; set; }

    [BsonElement("password_hash")]
    public string PasswordHash { get; set; }
}
    