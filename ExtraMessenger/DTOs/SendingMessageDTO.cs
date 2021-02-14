using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExtraMessenger.DTOs
{
    public class SendingMessageDTO
    {
        public string Message { get; set; }

        public ObjectId? ChatInteractionId { get; set; }
    }
}
