using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtraMessenger.Models
{
    public class User
    {
        public ObjectId Id { get; set; }

        public string Username { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        
        public List<Contact> Contacts { get; set; }

        public string OAuthToken { get; set; }
        public string CSRF { get; set; }

        public DateTime? LastFetchedRepo { get; set; }
        public DateTime? LastFetchedIssue { get; set; }
        public List<Repository> Repositories { get; set; }
    }
}
