using ExtraMessenger.DTOs;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExtraMessenger.Models
{
    public class Ticket
    {
        [Required]
        public ObjectId Id { get; set; }
        [Required]
        public string Title { get; set; }
        public List<TicketPart> Parts { get; set; }
        public string Introduction { get; set; }
        public List<string> Topics { get; set; }
        public string Difficulty { get; set; }
        public string Owner { get; set; }
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }
    }
}
