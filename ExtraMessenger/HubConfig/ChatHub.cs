using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using ExtraMessenger.DTOs;
using MongoDB.Bson;

namespace ExtraMessenger.Hubs
{
    // [Authorize]
    public class ChatHub : Hub
    {
        private readonly static ConnectionMapping<ObjectId> _connections =
            new ConnectionMapping<ObjectId>();

        private readonly IConfiguration _config;

        public ChatHub(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendMessage(ObjectId reciever, SendingMessageDTO message)
        {
            //Laza Comes Here
            var userConnections = _connections.GetConnections(reciever);

            foreach (var connectionId in userConnections)
            {
                await Clients.Client(connectionId).SendAsync("recievedMessage", message.Message);
            }

        }

        public override async Task OnConnectedAsync()
        {
            //ObjectId user = ObjectId.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            //_connections.Add(user, Context.ConnectionId);

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
