using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using ExtraMessenger.DTOs;

namespace ExtraMessenger.Hubs
{
    // [Authorize]
    public class ChatHub : Hub
    {
        private readonly static ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();

        private readonly IConfiguration _config;

        public ChatHub(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendMessage(string reciever, SendingMessageDTO message)
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
