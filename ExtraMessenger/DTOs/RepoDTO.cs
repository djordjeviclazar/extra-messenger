using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExtraMessenger.DTOs
{
    public class RepoDTO
    {
        [Required]
        public long Id { get; set; }

        public string Owner { get; set; }

        public string OwnerId { get; set; }

        public string Name { get; set; }

        public string Language { get; set; }
    }
}
