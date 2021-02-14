using MongoDB.Bson;
using System;

namespace ExtraMessenger.Models
{
    public class Message
    {
        public ObjectId Id { get; set; }

        public string Content { get; set; }

        public DateTime DateSent { get; set; }

        public string Sender { get; set; }

        public bool Seen { get; set; }
    }
}