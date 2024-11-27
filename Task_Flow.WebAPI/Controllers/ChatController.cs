using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Hubs;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IFriendService friendService;
        private readonly IChatMessageService chatMessageService;
        private readonly IChatService chatService;
        private readonly IHubContext<ConnectionHub> hubContext;

        public ChatController(IUserService userService, IFriendService friendService, IHubContext<ConnectionHub> hubContext, IChatService chatService, IChatMessageService chatMessageService)
        {
            this.userService = userService;
            this.friendService = friendService;
            this.hubContext = hubContext;
            this.chatService = chatService;
            this.chatMessageService = chatMessageService;
        }

        [Authorize]
        [HttpGet("AllChatsWithFriends")]
        public async Task<IActionResult> GetAllFriends()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }

            var myfriends = await friendService.GetFriends(userId);
            var sorted = new List<FriendForMessageDto>();


            foreach (var item in myfriends)
            {
                var chat = await chatService.GetByRecieverAndSenderId(userId, item.UserFriendId);
                ChatMessage latestmessage = null;
                bool isReciever = false;
                if (chat != null)
                {
                    latestmessage = await chatMessageService.GetLatestMessageByChatIdAsync(chat.Id);
                    isReciever = latestmessage.SenderId !=userId;
                }
                var user = await userService.GetUserById(item.UserFriendId);
                sorted.Add(new FriendForMessageDto
                {
                    FriendFullname = user.Firstname + " " + user.Lastname,
                    FriendEmail = user.Email,
                    FriendImg = user.Image,
                    isReciever = isReciever,
                    RecentMessage = (latestmessage==null?"":latestmessage.Content),

                });

            }


            sorted =  sorted.OrderBy(x => x.isReciever).ToList();

            return Ok(new { List = sorted });


        }

    }
}
