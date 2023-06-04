using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ChatServerWebApi.Models
{
    public class ChatMessage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Content { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
