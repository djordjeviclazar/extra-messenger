using MongoDB.Bson;

namespace ExtraMessenger.Models
{
    public class Contact
    {
        public ObjectId Id { get; set; }

        public ObjectId ChatInteractionReference { get; set; }

        public string Status { get; set; }

        public string Name { get; set; }
    }
}