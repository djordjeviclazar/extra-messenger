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

namespace ExtraMessenger.Hubs
{
    // [Authorize]
    public class ChatHub : Hub
    {
        private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>();

        private readonly IConfiguration _config;
        private readonly MongoService _context;

        public ChatHub(IConfiguration config, MongoService context)
        {
            _config = config;
            _context = context;
        }

        public async Task SendMessage(string recieverName, SendingMessageDTO message)
        {
            //Laza Comes Here
            string name = Context.User.FindFirst(ClaimTypes.Name).Value;
            ObjectId id = ObjectId.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var data = _context.GetDb;

            Message newMessage = new Message { Id = ObjectId.GenerateNewId(), Content = message.Message, 
                                               Seen = false, DateSent = DateTime.Now, Sender = name };
            var chatCollection = data.GetCollection<ChatInteraction>("ChatInteractions");
            var userCollection = data.GetCollection<User>("Users");

            if (message.ChatInteractionId == null)
            {
                // New chat interaction: 
                List<Message> messages = new List<Message>();
                messages.Add(newMessage);
                ChatInteraction newChatInteraction = new ChatInteraction { Id = ObjectId.GenerateNewId(), Messages = messages};
                await chatCollection.InsertOneAsync(newChatInteraction);

                // New friend contact for first user:
                Contact contact = new Contact { ChatInteractionReference = newChatInteraction.Id, Id = ObjectId.GenerateNewId(), 
                                                Name = recieverName, Status = "Friend"};

                await userCollection.UpdateOneAsync($"{{_id = {id}}}", contact.ToBsonDocument());

                // New friend request for second user:
                contact = new Contact
                {
                    ChatInteractionReference = newChatInteraction.Id,
                    Id = ObjectId.GenerateNewId(),
                    Name = name,
                    Status = "Request"
                };

                await userCollection.UpdateOneAsync($"{{Username = {recieverName}}}", contact.ToBsonDocument());
            }
            else
            {
                // Just add message:
                var filter = Builders<ChatInteraction>.Filter.Eq("_id", message.ChatInteractionId);
                
                var chat = (await chatCollection.FindAsync<ChatInteraction>(filter)).FirstOrDefault();
                chat.Messages.Add(newMessage);
                await chatCollection.UpdateOneAsync($"{{_id = {message.ChatInteractionId}}}", chat.ToBsonDocument());
            }

            var userConnections = _connections.GetConnections(recieverName);

            foreach (var connectionId in userConnections)
            {
                await Clients.Client(connectionId).SendAsync("recievedMessage", message.Message);
            }

        }

        public override async Task OnConnectedAsync()
        {
            string username = Context.User.FindFirst(ClaimTypes.Name).Value;

            _connections.Add(username, Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            string username = Context.User.FindFirst(ClaimTypes.Name).Value;

            _connections.Remove(username, Context.ConnectionId);

            await base.OnDisconnectedAsync(ex);
        }
    }
}
