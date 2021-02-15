using ExtraMessenger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtraMessenger.DTOs
{
    public class ContactsReturnDTO
    {
        public string Id { get; set; }

        public string ChatInteractionReference { get; set; }

        public string Status { get; set; }

        public string Name { get; set; }

        public string OtherUserId { get; set; }

        public bool Seen { get; set; }

        public ContactsReturnDTO(Contact contact)
        {
            Id = contact.Id.ToString();
            ChatInteractionReference = contact.ChatInteractionReference.ToString();
            Status = contact.Status;
            Name = contact.Name;
            OtherUserId = contact.OtherUserId.ToString();
            Seen = contact.Seen;
        }
    }
}
