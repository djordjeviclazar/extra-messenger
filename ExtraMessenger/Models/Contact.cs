using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExtraMessenger.Models
{
    public class Contact
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public ObjectId ChatInteractionReference { get; set; }

        public string Status { get; set; }

        public string Name { get; set; }

        public ObjectId OtherUserId { get; set; }

        public bool Seen { get; set; }
    }
}