using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtraMessenger.Models
{
    public class CSRF
    {
        public ObjectId Id { get; set; }
        public string Csrf { get; set; }
        public ObjectId UserId { get; set; }
    }
}
