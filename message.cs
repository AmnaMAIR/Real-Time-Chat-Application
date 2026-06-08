using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace real_time_chat.Models
{
    public class Message
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("senderId")]
        public string SenderId { get; set; } = string.Empty;

        [BsonElement("receiverId")]
        public string ReceiverId { get; set; } = string.Empty;

        [BsonElement("groupId")]
        public string GroupId { get; set; } = string.Empty;

        [BsonElement("messageText")]
        public string MessageText { get; set; } = string.Empty;

        [BsonElement("fileUrl")]
        public string FileUrl { get; set; } = string.Empty;

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [BsonElement("status")]
        public string Status { get; set; } = "Sent";
    }
}