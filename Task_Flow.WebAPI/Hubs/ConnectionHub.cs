using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Task_Flow.DataAccess.Abstract;

namespace Task_Flow.WebAPI.Hubs
{
    public class ConnectionHub : Hub
    {
        private readonly IUserService _userService;

        public ConnectionHub(IUserService userService)
        {
            _userService = userService;
        }

        public async Task SendOnlineStatus(string userId, bool isOnline)
        {
            var Id = userId;
            if (userId != null)
            {
                var user = await _userService.GetUserById(userId);
                if (user != null)
                {
                    user.IsOnline = isOnline;
                    await _userService.Update(user);
                    await Clients.All.SendAsync("ReceiveOnlineStatus", userId, isOnline);
                }
               
            }
        }
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                var user = await _userService.GetUserById(userId);
                if (user != null)
                {
                    user.IsOnline = true;
                    await _userService.Update(user);
                    await Clients.All.SendAsync("ReceiveConnectInfo", $"{user.UserName} has connected");
                }
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                var user = await _userService.GetUserById(userId);
                if (user != null)
                {
                    user.IsOnline = false;
                    await _userService.Update(user);
                    await Clients.All.SendAsync("DisconnectInfo", $"{user.UserName} has disconnected");
                }
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
