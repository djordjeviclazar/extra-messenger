using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtraMessenger.DTOs
{
    public class UserProfileDTO
    {
        public string Username { get; set; }
        public List<string> LikedTopics { get; set; }
        public List<string> OtherTopics { get; set; }
        public List<TicketRes> TopTickets { get; set; }
        public int Rating { get; set; }
    }

    public class TicketRes
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int Rating { get; set; }
    }
}
