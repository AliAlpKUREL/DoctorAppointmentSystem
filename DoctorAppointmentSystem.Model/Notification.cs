using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DoctorAppointmentSystem.Model;

public class Notification
{
    [BsonId]
    [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("pharmarcy_id"), BsonRepresentation(BsonType.ObjectId)]
    public string PharmacyId { get; set; }

    [BsonElement("prescription_id"), BsonRepresentation(BsonType.ObjectId)]
    public string PrescriptionId { get; set; }

    [BsonElement("message")]
    public string Message { get; set; }

    [BsonElement("sent_at"), BsonRepresentation(BsonType.DateTime)]
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}
