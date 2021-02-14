using ExtraMessenger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtraMessenger.DTOs
{
    public class MessageReturnDTO
    {
        public string Id { get; set; }

        public string Content { get; set; }

        public DateTime DateSent { get; set; }

        public string Sender { get; set; }

        public bool Seen { get; set; }

        public MessageReturnDTO()
        {

        }

        public MessageReturnDTO(Message message)
        {
            Id = message.Id.ToString();
            Content = message.Content;
            DateSent = message.DateSent;
            Sender = message.Sender;
            Seen = message.Seen;
        }
    }
}
