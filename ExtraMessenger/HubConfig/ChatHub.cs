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
        private readonly static ConnectionMapping<int> _connections =
            new ConnectionMapping<int>();

        private readonly IConfiguration _config;

        public ChatHub(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendMessage(int recieverId, SendingMessageDTO message)
        {
            //Laza Comes Here
            var userConnections = _connections.GetConnections(recieverId);

            foreach (var connectionId in userConnections)
            {
                await Clients.Client(connectionId).SendAsync("recievedMessage", message.Message);
            }

        }

        public override async Task OnConnectedAsync()
        {
            int userId = int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            _connections.Add(userId, Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            int userId = int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            _connections.Remove(userId, Context.ConnectionId);

            await base.OnDisconnectedAsync(ex);
        }
    }
}
