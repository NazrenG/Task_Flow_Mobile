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
using Task_Flow.WebAPI.Hubs;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendController : ControllerBase
    {
        private readonly IFriendService friendService;
        private readonly IUserService _userService;
        private readonly UserManager<CustomUser> _userManager;
        private readonly IRequestNotificationService _requestNotificationService;
        private readonly IHubContext<ConnectionHub> hubContext;

        public FriendController(IFriendService friendService,IHubContext<ConnectionHub> hubContext, IUserService userService, UserManager<CustomUser> userManager, IRequestNotificationService requestNotificationService)
        {
            this.friendService = friendService;
            _userService = userService;
            _userManager = userManager;
            _requestNotificationService = requestNotificationService;
            this.hubContext = hubContext;
        }

        [Authorize]
        [HttpGet("AllUser")]
        public async Task<IActionResult> GetAllUsers()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }

            var myrequests = await _requestNotificationService.GetNotificationsBySenderId(userId);

            var allfriends = await friendService.GetFriends(userId);

            var allusers = await _userService.GetUsers();
            var users = allusers.Where(u => u.Id != userId)
            .OrderByDescending(u => u.IsOnline)
            .Select(u => new FriendDto
            {
                Id = u.Id,
                FriendName = u.UserName,
                IsOnline = u.IsOnline,
                FriendPhoto = u.Image,
                FriendEmail = u.Email,
                HasRequestPending = (myrequests.FirstOrDefault(r => r.ReceiverId == u.Id&& r.NotificationType == "FriendRequest"&&!r.IsAccepted) != null),
                IsFriend = allfriends.FirstOrDefault(f => f.UserId == u.Id || f.UserFriendId == u.Id) != null
            }).ToList();

            return Ok(users);

           

        }

        [Authorize]
        [HttpDelete("DeleteRequest/{friendId}")]
        public async Task<IActionResult> DeleteRequest(string friendId)
        {
            try
            {
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return BadRequest(new { message = "User not authenticated." });
                }

                var sentRequests = await _requestNotificationService.GetNotificationsBySenderId(userId);
                var request = sentRequests.FirstOrDefault(r => r.ReceiverId == friendId && r.NotificationType == "FriendRequest");

                if (request == null)
                {
                    var receivedRequests = await _requestNotificationService.GetRequestNotifications(userId);
                    request = receivedRequests.FirstOrDefault(r => r.SenderId == friendId && r.NotificationType == "FriendRequest");
                }

                if (request == null)
                {
                    return NotFound(new { message = "Friend request not found or not deletable." });
                }

                await _requestNotificationService.Delete(request);

                await hubContext.Clients.User(userId).SendAsync("UpdateUserActivity");

                return Ok(new { message = "Friend request deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }






        // GET: api/<FriendController>
        [Authorize]
        [HttpGet("AllFriends")]
        public async Task<IActionResult> GetAllFriends()
        {

            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }
            var requestsList=await _requestNotificationService.GetNotificationsBySenderId(userId);
            var list = await friendService.GetFriends(userId);
            var result = new List<FriendDto>();

            foreach (var p in list)
            {
                var check = await friendService.CheckFriendship(p.UserId, p.UserFriendId);
                var hasPending = requestsList.Any(r => r.ReceiverId == p.UserFriendId && r.IsAccepted == false);

                result.Add(new FriendDto
                {
                    FriendName = p.UserFriend.Firstname + " " + p.UserFriend.Lastname,
                    FriendEmail = p.UserFriend.Email,
                    FriendOccupation = p.UserFriend.Occupation,
                    FriendPhone = p.UserFriend.PhoneNumber,
                    FriendPhoto = p.UserFriend.Image,
                    IsOnline = p.UserFriend.IsOnline,
                    CheckFriend = check,
                    IsFriend=true,
                    HasRequestPending=hasPending,
                    
                });
            }
            return Ok(result);
        }



        //  POST api/<FriendController>
        [Authorize]
        [HttpPost("NewFriend")]
        public async Task<IActionResult> Post([FromBody] SendFollowFriendDto value)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }
            var item = new Friend
            {
                UserFriendId = value.FriendId,
                UserId = userId,
            };
            await friendService.Add(item);
            return Ok(item);
        }


        [Authorize]
        [HttpDelete("UnFollow/{friendMail}")]
        public async Task<IActionResult> DeleteFriend(string friendMail)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized(new { message = "user not found" });
            }
            var friend = await _userManager.FindByEmailAsync(friendMail);
            if (friend == null) { return Ok(new { message = "friend not found" }); }
            var existFriend = await friendService.GetFriendByUserAndFriendId(userId, friend.Id);
            if (existFriend == null) return Ok(new { message = "friend not found" });
            await friendService.Delete(existFriend);
            await hubContext.Clients.User(userId).SendAsync("UpdateUserActivity");
            return Ok(new { message = "accept request succesfuly" });

        }


        [Authorize]
        [HttpGet("GetFriendContacts/{friendMail}")]
        public async Task<IActionResult> GetFriendContacts(string friendMail)
        {
            var friend = await _userManager.FindByEmailAsync(friendMail);
            if (friend == null) { return Ok(new { message = "friend not found" }); }
            var dto = new FriendContactInfoDto{
            Number=friend?.PhoneNumber,
            Email=friend?.Email};
            return Ok(dto);
        }


    }
}
