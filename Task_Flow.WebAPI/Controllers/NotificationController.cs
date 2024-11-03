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
    public class NotificationController : ControllerBase
    {

        private readonly INotificationService notificationService;
        private readonly IUserService userService;

        public NotificationController(INotificationService notificationService, IUserService userService)
        {
            this.notificationService = notificationService;
            this.userService = userService;
        }



        [Authorize]
        // GET: api/<NotificationController>
        //userin bildirimleri
        [HttpGet("Notifications")]
        public async Task<IActionResult> Get()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest("User not authenticated.");
            }

            var list = await notificationService.GetNotifications();
            var items = list.Where(i => i.UserId == userId && i.IsCalendarMessage == false).Select(p =>
            {
                return new
                {
                    Text = p.Text,
                    Username = p.User?.UserName,
                };
            });
            return Ok(items);
        }
        [Authorize]
        [HttpGet("TwoNotification")]
        public async Task<IActionResult> TakeTwoMessage()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest("User not authenticated.");
            }


            var list = await notificationService.GetNotifications();
            var items = list.Where(i => i.UserId == userId && i.IsCalendarMessage == false).OrderByDescending(p => p.Id).Take(2).
                Select(p =>
            {
                return new
                {
                    Text = p.Text,
                    Username = p.User?.UserName,
                };
            });
            return Ok(items);
        }

        [Authorize]
        [HttpGet("CalendarNotifications")]
        public async Task<IActionResult> GetCalendarNotifications()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest("User not authenticated.");
            }

            if (!int.TryParse(userId, out int id))
            {
                return BadRequest("Invalid user ID.");
            }
            var list = await notificationService.GetNotifications();
            var items = list.Where(i => i.UserId == userId && i.IsCalendarMessage == true).Select(p =>
            {
                return new
                {
                    Text = p.Text,
                    Username = p.User?.UserName,
                };
            });
            return Ok(items);
        }
        [Authorize]
        [HttpGet("TwoCalendarNotification")]
        public async Task<IActionResult> TakeTwoCalendarNotification()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest("User not authenticated.");
            }



            var list = await notificationService.GetNotifications();
            var items = list.Where(i => i.UserId == userId && i.IsCalendarMessage)
                            .OrderByDescending(p => p.Id)
                            .Take(2)
                            .Select(p => new
                            {
                                Text = p.Text,
                                Username = p.User?.UserName,
                            });

            return Ok(items);
        }


        [Authorize]
        //userin bildirim sayi
        [HttpGet("UserNotificationCount")]
        public async Task<IActionResult> GetCount()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var list = await notificationService.GetNotifications();

            return Ok(list.Where(l => l.UserId == userId && l.IsCalendarMessage == false).Count());
        }

        [Authorize]
        //userin calendar ucun olan bildirim sayi
        [HttpGet("CalendarNotificationCount")]
        public async Task<IActionResult> GetCalendarNotificationCount()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var list = await notificationService.GetNotifications();

            return Ok(list.Where(l => l.UserId == userId && l.IsCalendarMessage == true).Count());
        }


        // POST api/<NotificationController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] NotificationDto value)
        {

            var item = new Notification
            {
                Text = value.Text,
                UserId = value.UserId,
            };
            await notificationService.Add(item);
            return Ok(item);
        }


        // DELETE api/<NotificationController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var item = await notificationService.GetNotificationById(id);
            if (item == null) return NotFound();
            await notificationService.Delete(item);
            return Ok();
        }
    }
}
