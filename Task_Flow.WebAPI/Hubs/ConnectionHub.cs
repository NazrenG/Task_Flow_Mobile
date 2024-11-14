using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;

namespace Task_Flow.WebAPI.Hubs
{
    public class ConnectionHub : Hub
    {
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<CustomUser> _userManager;
        public ConnectionHub(IUserService userService, IHttpContextAccessor contextAccessor,UserManager<CustomUser>userManager)
        {
            _userService = userService;
            _contextAccessor = contextAccessor;
            _userManager = userManager;
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
        [Authorize]
        public override async Task OnConnectedAsync()
        {
            var user = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
            //if (userId != null)
            //{
            //    var user = await _userService.GetUserById(userId);
            //    if (user != null)
            //    {
            //        user.IsOnline = true;
            //        await _userService.Update(user);
            //        await Clients.All.SendAsync("ReceiveConnectInfo", $"{user.UserName} has connected");
            //    }
            //}
            if(user!=null)
                    await Clients.All.SendAsync("ReceiveConnectInfo", $"{user.UserName} has connected");
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
