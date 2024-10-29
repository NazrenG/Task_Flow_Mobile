using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;


namespace Task_Flow.WebAPI.Hubs
{
    public class ConnectionHub : Hub
    {

        private readonly UserManager<CustomUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserService _userService;
        public ConnectionHub(UserManager<CustomUser> userManager, IHttpContextAccessor contextAccessor, IUserService userService)
        {
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _userService = userService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                var item = await _userService.GetUserById(userId);
                if (item != null)
                {
                    item.IsOnline = true;
                    await _userService.Update(item);
                    await _userManager.UpdateAsync(item);

                    await Clients.All.SendAsync("ReceiveConnectInfo", $"{item.UserName} has connected");
                }
            }
            await base.OnConnectedAsync();
        }



        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                var item = await _userService.GetUserById(userId);
                if (item != null)
                {
                    item.IsOnline = false;
                    await _userService.Update(item);
                    await _userManager.UpdateAsync(item);

                    await Clients.All.SendAsync("DisconnectInfo", $"{item.UserName} has disconnected");

                }
            }
            await base.OnDisconnectedAsync(new Exception());

        }
    }


}
