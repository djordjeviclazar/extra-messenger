using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtraMessenger.Models
{
    public class ChatInteraction
    {
        public ObjectId Id { get; set; }

        public List<Message> Messages { get; set; }
    }
}
