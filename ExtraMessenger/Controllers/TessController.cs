using ExtraMessenger.Data;
using ExtraMessenger.DTOs;
using ExtraMessenger.Hubs;
using ExtraMessenger.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtraMessenger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TessController : ControllerBase
    {
        private readonly ChatHub _hub;
        private readonly MongoService _context;

        public TessController(MongoService context, ChatHub hub)
        {
            _hub = hub;
            _context = context;
        }

        [HttpGet("edit/")]
        public async Task<IActionResult> TestEdit()
        {
            /*
            await _hub.EditMessage("60286f9f5e7da329ba5eb813",
                                    new EditMessageDTO { ChatInteractionId = "602923c3301b700389cdb3e1", 
                                                         Message = "TestEdit",
                                                         Id = "602923c3301b700389cdb3e0"});
            return Ok();
            */
            string senderName = "tes"; //Context.User.FindFirst(ClaimTypes.Name).Value;
            ObjectId senderId = ObjectId.Parse("60286f7c5e7da329ba5eb812");//ObjectId.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            ObjectId receiver = ObjectId.Parse("60286f9f5e7da329ba5eb813");

            ObjectId? chatInteractionId = null;
            EditMessageDTO message = new EditMessageDTO { ChatInteractionId = "602923c3301b700389cdb3e1", 
                                                         Message = "TestEdit",
                                                         Id = "602923c3301b700389cdb3e0"};
            if (message.ChatInteractionId != null)
                chatInteractionId = ObjectId.Parse(message.ChatInteractionId);

            var data = _context.GetDb;

            var chatCollection = data.GetCollection<ChatInteraction>("ChatInteractions");
            var userCollection = data.GetCollection<User>("Users");

            var filterForSenderUser = Builders<User>.Filter.Eq("_id", senderId);

            var filterForReceiverUser = Builders<User>.Filter.Eq("_id", receiver);

            if (chatInteractionId != null)
            {
                // Edit message:
                var filterChat = Builders<ChatInteraction>.Filter.Eq("_id", chatInteractionId);
                var filterMessage = Builders<Message>.Filter.Eq("_id", message.Id);
                var filterMessageList = Builders<ChatInteraction>.Filter.ElemMatch("Messages", filterMessage);
                var filter = Builders<ChatInteraction>.Filter.And(filterChat, filterMessageList);

                var update = Builders<ChatInteraction>.Update.Set(chatOrigin => chatOrigin.Messages[-1].Content, message.Message);
                await chatCollection.UpdateOneAsync(filter, update);
            }
            else
            {
                var receiverUser = (await userCollection.FindAsync(filterForReceiverUser)).FirstOrDefault();

                // check receiver contacts if they contain sender
                var chatInteractionWithSender = receiverUser.Contacts?.Where(contact => contact.Name == senderName).FirstOrDefault();
                if (chatInteractionWithSender != null)
                {
                    // Edit message:
                    var filterChat = Builders<ChatInteraction>.Filter.Eq("_id", chatInteractionId);
                    var filterMessage = Builders<Message>.Filter.Eq("_id", message.Id);
                    var filterMessageList = Builders<ChatInteraction>.Filter.ElemMatch("Messages", filterMessage);
                    var filter = Builders<ChatInteraction>.Filter.And(filterChat, filterMessageList);

                    var update = Builders<ChatInteraction>.Update.Set(chatOrigin => chatOrigin.Messages[-1].Content, message.Message);
                    await chatCollection.UpdateOneAsync(filter, update);
                }
                else
                {
                    return BadRequest();
                }
            }

            return Ok();
        }

        [HttpGet("delete/")]
        public async Task<IActionResult> TestDelete()
        {
            /*await _hub.EditMessage("60286f9f5e7da329ba5eb813",
                                    new EditMessageDTO
                                    {
                                        ChatInteractionId = "602923c3301b700389cdb3e1",
                                        Id = "602923c3301b700389cdb3e0"
                                    });
            return Ok();*/

            string senderName = "tes"; //Context.User.FindFirst(ClaimTypes.Name).Value;
            ObjectId senderId = ObjectId.Parse("60286f7c5e7da329ba5eb812");//ObjectId.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            ObjectId receiver = ObjectId.Parse("60286f9f5e7da329ba5eb813");

            ObjectId? chatInteractionId = null;
            EditMessageDTO message = new EditMessageDTO
            {
                ChatInteractionId = "60287f294060e2f632006ca2",
                Message = "TestEdit",
                Id = "6029ab7a00f5dc00eeeed3cb"
            };
            if (message.ChatInteractionId != null)
                chatInteractionId = ObjectId.Parse(message.ChatInteractionId);

            var data = _context.GetDb;

            var chatCollection = data.GetCollection<ChatInteraction>("ChatInteractions");
            var userCollection = data.GetCollection<User>("Users");

            var filterForSenderUser = Builders<User>.Filter.Eq("_id", senderId);

            var filterForReceiverUser = Builders<User>.Filter.Eq("_id", receiver);

            if (chatInteractionId != null)
            {
                // Delete message:
                var filter = Builders<ChatInteraction>.Filter.Eq("_id", chatInteractionId);
                var innerFilter = Builders<Message>.Filter.Eq("_id", ObjectId.Parse(message.Id));

                var update = Builders<ChatInteraction>.Update.PullFilter("Messages", innerFilter);
                await chatCollection.UpdateOneAsync(filter, update);
            }
            else
            {
                var receiverUser = (await userCollection.FindAsync(filterForReceiverUser)).FirstOrDefault();

                // check receiver contacts if they contain sender
                var chatInteractionWithSender = receiverUser.Contacts?.Where(contact => contact.Name == senderName).FirstOrDefault();
                if (chatInteractionWithSender != null)
                {
                    // Delete message:
                    var filter = Builders<ChatInteraction>.Filter.Eq("_id", chatInteractionId);
                    var innerFilter = Builders<Message>.Filter.Eq("_id", ObjectId.Parse(message.Id));

                    var update = Builders<ChatInteraction>.Update.PullFilter("Messages", innerFilter);
                    await chatCollection.UpdateOneAsync(filter, update);
                }
                else
                {
                    return BadRequest();
                }
            }

            return Ok();
        }
    }
}
