using ExtraMessenger.Data;
using ExtraMessenger.DTOs;
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

            var filter = Builders<User>.Filter.Eq("Username", username);
            var user = (await data.GetCollection<User>("Users").FindAsync<User>(filter)).FirstOrDefault();
            if (user == null) { return BadRequest(); }

            return new JsonResult(user.Contacts.Select(c => new ContactsReturnDTO(c)));
        }

        [HttpGet("messages/{idString}/{page}/{row}")]
        public async Task<IActionResult> GetMessages(string idString, int page, int row)
        {
            string real = User.FindFirst(ClaimTypes.Name).Value;
            ObjectId id = new ObjectId(idString);

            var data = _context.GetDb;

            return NotFound();

            /*var message = (await data.GetCollection<ChatInteraction>("ChatInteractions").Aggregate()
                                                                               .Match(Ci => Ci.Id == id)
                                                                               .Unwind<ChatInteraction, ChatInteraction> (x => x.Messages)
                                                                               .Sort("{DateSent: -1}")
                                                                               //.Skip(page * row)
                                                                               //.Limit(row)
                                                                               .ToListAsync()
                                                                               );*/

            //return new JsonResult(message.Select(m => new MessageReturnDTO(m)));
        }

        [HttpGet("messages/{chatInteractionId}")]
        public async Task<IActionResult> GetMessages(string chatInteractionId)
        {
            string real = User.FindFirst(ClaimTypes.Name).Value;
            ObjectId id = new ObjectId(chatInteractionId);

            var data = _context.GetDb;

            var filter = Builders<ChatInteraction>.Filter.Eq("_id", id);
            var chatInteraction = await (await data.GetCollection<ChatInteraction>("ChatInteractions").FindAsync(filter)).FirstOrDefaultAsync();
            chatInteraction.Messages.Sort((x, y) => DateTime.Compare(y.DateSent, x.DateSent));

            return new JsonResult(chatInteraction.Messages.Select(m => new MessageReturnDTO(m)));
        }
        /*ovo ne ovde
        [HttpPost("send/")]
        public async Task<IActionResult> SendMessage(SendMessageDTO dto)
        {
            
        }
        */
    }
}
