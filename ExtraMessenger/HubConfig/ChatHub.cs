using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using ExtraMessenger.DTOs;
using ExtraMessenger.Data;
using ExtraMessenger.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Linq;

namespace ExtraMessenger.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly static ConnectionMapping<ObjectId> _connections = new ConnectionMapping<ObjectId>();

        private readonly IConfiguration _config;
        private readonly MongoService _context;

        public ChatHub(IConfiguration config, MongoService context)
        {
            _config = config;
            _context = context;
        }

        public async Task SendMessage(string receiverId, SendingMessageDTO message)
        {
            string senderName = Context.User.FindFirst(ClaimTypes.Name).Value;
            ObjectId senderId = ObjectId.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            ObjectId receiver = ObjectId.Parse(receiverId);
            ObjectId.TryParse(message.ChatInteractionId, out ObjectId chatInteractionId);

            var data = _context.GetDb;

            Message newMessage = new Message { Id = ObjectId.GenerateNewId(), Content = message.Message, 
                                               Seen = false, DateSent = DateTime.Now, Sender = senderName };
            var chatCollection = data.GetCollection<ChatInteraction>("ChatInteractions");
            var userCollection = data.GetCollection<User>("Users");
            
            var filterForSenderUser = Builders<User>.Filter.Eq("_id", senderId);
            var senderUser = (await userCollection.FindAsync<User>(filterForSenderUser)).First();
            var filterForReceiverUser = Builders<User>.Filter.Eq("_id", receiver);
            var receiverUser = (await userCollection.FindAsync(filterForReceiverUser)).FirstOrDefault();

            if (chatInteractionId == null)
            {
                // check receiver contacts if they contain sender
                var chatInteractionWithSender = receiverUser.Contacts?.Where(contact => contact.Name == senderName).FirstOrDefault();
                if (chatInteractionWithSender != null)
                    chatInteractionId = chatInteractionWithSender.ChatInteractionReference;
            }

            if (chatInteractionId == null)
            {
                // New chat interaction: 
                List<Message> messages = new List<Message> { newMessage };
                ChatInteraction newChatInteraction = new ChatInteraction { Id = ObjectId.GenerateNewId(), Messages = messages };
                await chatCollection.InsertOneAsync(newChatInteraction);

                Contact contact;
                // New friend request for receiver:
                contact = new Contact
                {
                    ChatInteractionReference = newChatInteraction.Id,
                    Id = ObjectId.GenerateNewId(),
                    Name = senderName,
                    Status = "Request",
                    OtherUserId = senderId
                };

                UpdateDefinition<User> updateDefinition;
                if (receiverUser.Contacts == null)
                    updateDefinition = Builders<User>.Update.Set("Contacts", new List<Contact> { contact });
                else
                    updateDefinition = Builders<User>.Update.Push<Contact>("Contacts", contact);
                await userCollection.UpdateOneAsync(filterForReceiverUser, updateDefinition);

                // New friend contact for sender:
                contact = new Contact
                {
                    ChatInteractionReference = newChatInteraction.Id,
                    Id = ObjectId.GenerateNewId(),
                    Name = receiverUser.Username,
                    Status = "Friend",
                    OtherUserId = receiver
                };

                if (senderUser.Contacts == null)
                    updateDefinition = Builders<User>.Update.Set("Contacts", new List<Contact> { contact });
                else
                    updateDefinition = Builders<User>.Update.Push<Contact>("Contacts", contact);
                await userCollection.UpdateOneAsync(filterForSenderUser, updateDefinition);
            }
            else
            {
                // Just add message:
                var filter = Builders<ChatInteraction>.Filter.Eq("_id", chatInteractionId);
                await chatCollection.UpdateOneAsync(filter, Builders<ChatInteraction>.Update.Push<Message>("Messages", newMessage));
            }

            var userConnections = _connections.GetConnections(receiver);

            foreach (var connectionId in userConnections)
            {
                await Clients.Client(connectionId).SendAsync("receivedMessage", new { SenderId = senderId.ToString(), Message = message.Message }) ;
            }

            var senderConnections = _connections.GetConnections(senderId);

            foreach (var connectionId in senderConnections)
            {
                await Clients.Client(connectionId).SendAsync("receivedMessage", new { SenderId = senderId.ToString(), Message = message.Message });
            }

        }

        public override async Task OnConnectedAsync()
        {
            ObjectId user = ObjectId.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            _connections.Add(user, Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            ObjectId user = ObjectId.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            _connections.Remove(user, Context.ConnectionId);

            await base.OnDisconnectedAsync(ex);
        }
    }
}
