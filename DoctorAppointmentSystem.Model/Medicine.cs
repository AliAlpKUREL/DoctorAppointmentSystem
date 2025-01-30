using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DoctorAppointmentSystem.Model;

public class Medicine
{
    [BsonId]
    [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("price"), BsonRepresentation(BsonType.Double)]
    public double Price { get; set; }
}
