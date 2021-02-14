using ExtraMessenger.Data;
using ExtraMessenger.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ExtraMessenger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessengerController:ControllerBase
    {
        private readonly MongoService _context;

        public MessengerController(MongoService context)
        {
            _context = context;
        }

        [HttpGet("contacts/{username}")]
        public async Task<IActionResult> GetContacts(string username)
        {
            string real = User.FindFirst(ClaimTypes.Name).Value;
            if (!real.Equals(username)) { return Unauthorized(); }

            var data = _context.GetDb;

            var filter = Builders<User>.Filter.Eq<string>("Username", username);
            var user = (await data.GetCollection<User>("Users").FindAsync<User>(filter)).FirstOrDefault();
            if (user == null) { return BadRequest(); }

            return new JsonResult(user.Contacts);
        }

        [HttpGet("messages/{id}/{page}/{row}")]
        public async Task<IActionResult> GetMessages(ObjectId id, int page, int row)
        {
            string real = User.FindFirst(ClaimTypes.Name).Value;

            var data = _context.GetDb;

            var message = (data.GetCollection<ChatInteraction>("ChatInteractions").Aggregate()
                                                                               .Match<ChatInteraction>(Ci => Ci.Id == id)
                                                                               .Unwind<ChatInteraction, Message>(x => x.Messages)
                                                                               .Sort("{DateSent: -1}")
                                                                               .Skip(page * row)
                                                                               .Limit(row));

            return new JsonResult(message);
        }
        /*ovo ne ovde
        [HttpPost("send/")]
        public async Task<IActionResult> SendMessage(SendMessageDTO dto)
        {
            
        }
        */
    }
}
