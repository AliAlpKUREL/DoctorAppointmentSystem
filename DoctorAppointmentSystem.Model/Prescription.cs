using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DoctorAppointmentSystem.Model;

public class Prescription
{
    [BsonId]
    [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("prescription_id")]
    public string PrescriptionId { get; set; }

    [BsonElement("doctor_id"), BsonRepresentation(BsonType.ObjectId)]
    public string DoctorId { get; set; }

    [BsonElement("patient_id"), BsonRepresentation(BsonType.ObjectId)]  
    public string PatientId { get; set; }

    [BsonElement("medicines")]
    public List<Medicine> Medicines { get; set; } = [];

    [BsonElement("is_completed"), BsonRepresentation(BsonType.Boolean)]
    public bool IsCompleted { get; set; } = false;

    [BsonElement("created_at"), BsonRepresentation(BsonType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
