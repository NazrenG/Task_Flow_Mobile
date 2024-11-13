using Microsoft.AspNetCore.Authorization;
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

        public FriendController(IFriendService friendService, IUserService userService)
        {
            this.friendService = friendService;
            _userService = userService;
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

            var list = await _userService.GetUsers();//butun userler
            var friends=await friendService.GetFriends(userId);//dostlari
            var allFriendList = friends.Select(l=>l.UserFriendId).ToList();
            var item=new List<CustomUser>();
            foreach (var user in list)
            {
                if (user.Id != userId && !allFriendList.Contains(user.Id))
                {
                    // Dost olmayan kullanıcıyı ekle
                    item.Add(user);
                }
            }
            var items = item.Select(p =>
            {
                return new FriendDto
                {
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

        //// GET api/<FriendController>/5
        //[HttpGet("{id}")]
        //public async Task<IActionResult> Get(int id)
        //{
        //    var item = await friendService.GetFriendsById(id);
        //    if (item == null)
        //    {
        //        return NotFound();
        //    }
        //    var friend = new FriendDto
        //    {
        //        UserFriendId = item.UserFriendId,
        //        UserId = item.UserId,
        //    };
        //    return Ok(friend);
        //}

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


        // DELETE api/<FriendController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await friendService.GetFriendsById(id);
            if (item == null)
            {
                return NotFound();
            }
            await friendService.Delete(item);
            return Ok();
        }
    }
}
