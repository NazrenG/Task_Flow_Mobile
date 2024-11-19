using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendController : ControllerBase
    {
        private readonly IFriendService friendService;
        private readonly IUserService _userService;
        private readonly UserManager<CustomUser> _userManager;

        public FriendController(IFriendService friendService, IUserService userService, UserManager<CustomUser> userManager)
        {
            this.friendService = friendService;
            _userService = userService;
            _userManager = userManager;
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

            var list = await _userService.GetUsers();
            var friends = await friendService.GetFriends(userId);
            var allFriendList = friends.Select(l => l.UserFriendId).ToList();
            var item = new List<CustomUser>();
            foreach (var user in list)
            {
                if (user.Id != userId && !allFriendList.Contains(user.Id))
                {
                    item.Add(user);
                }
            }
            var items = item.Select(p =>
            {
                return new FriendDto
                {
                    Id = p.Id,
                    FriendName = p.Firstname + " " + p.Lastname,
                    FriendEmail = p.Email,
                    FriendOccupation = p.Occupation,
                    FriendPhone = p.PhoneNumber,
                    FriendPhoto = p.Image,
                    IsOnline = p.IsOnline,
                };
            });
            return Ok(items);
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

            var list = await friendService.GetFriends(userId);
            var items = list.Select(p =>
            {
                return new FriendDto
                {
                    FriendName = p.UserFriend.Firstname + " " + p.UserFriend.Lastname,
                    FriendEmail = p.UserFriend.Email,
                    FriendOccupation = p.UserFriend.Occupation,
                    FriendPhone = p.UserFriend.PhoneNumber,
                    FriendPhoto = p.UserFriend.Image,
                    IsOnline = p.UserFriend.IsOnline,
                };
            });
            return Ok(items);
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
            return Ok(new { message = "accept request succesfuly" });

        }
    }
}
