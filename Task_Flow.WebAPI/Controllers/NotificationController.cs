using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.DataAccess.Concrete;
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
        private readonly INotificationSettingService notificationSettingService;
        private readonly IRecentActivityService recentActivityService;
        private readonly IRequestNotificationService requestNotificationService;
        private readonly UserManager<CustomUser> _userManager;

        public NotificationController(INotificationService notificationService, IUserService userService, INotificationSettingService notificationSettingService, IRecentActivityService recentActivityService, IRequestNotificationService requestNotificationService, UserManager<CustomUser> userManager)
        {
            this.notificationService = notificationService;
            this.userService = userService;
            this.notificationSettingService = notificationSettingService;
            this.recentActivityService = recentActivityService;
            this.requestNotificationService = requestNotificationService;
            _userManager = userManager;
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
                return BadRequest(new { message = "User not authenticated." });
            }

            var list = await requestNotificationService.GetRequestNotifications(userId);
            var items = list.Where(i => i.IsAccepted==false).Select(p =>
            {
                return new
                {
                    Text = p.Text,
                    Username = p.Sender?.UserName,
                    Path = p.Sender.Image,
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
                return BadRequest(new { message = "User not authenticated." });
            }

            var list = await requestNotificationService.GetRequestNotifications(userId);
            var items = list.Where(i => i.IsAccepted == false).OrderByDescending(p => p.Id).Take(2).
                Select(p =>
            {
                return new
                {
                    Text = p.Text,
                    Username = p.Sender?.UserName,
                    Path = p.Sender.Image,
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
                return BadRequest(new { message = "User not authenticated." });
            }

            if (!int.TryParse(userId, out int id))
            {
                return BadRequest(new { message = "Invalid user ID." });
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
                return BadRequest(new { message = "User not authenticated." });
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

            var list = await requestNotificationService.GetRequestNotifications(userId);

            return Ok(list.Where(l => l.IsAccepted==false).Count());
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


        //notification setting 
        [Authorize]
        [HttpGet("NotificationSetting")]
        public async Task<IActionResult> GetNotificationSetting()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized(new { message = "user not found" });
            }
             
            var item = await notificationSettingService.GetOrCreateNotificationSetting(userId);

            return Ok(new { success = true, message = "notification setting" });
        } 
        [Authorize]
        [HttpPost("UpdatedNotificationSetting")]
        public async Task<IActionResult> UpdateNotificationSetting(NotificationSettingDto dto)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized(new { message = "User not found" });
            }
             
            var item = await notificationSettingService.GetNotificationSetting(userId);

            if (item == null)
            {
                return NotFound(new{message= "Notification settings not found for the user."});
            }
             
            item.DeadlineReminders = dto.DeadlineReminders;
            item.FriendshipOffers = dto.FriendshipOffers;
            item.IncomingComments = dto.IncomingComments;
            item.InternalTeamMessages = dto.InternalTeamMessages;
            item.NewProjectProposals = dto.NewProjectProposals; 
            await notificationSettingService.Update(item);

            return Ok(new { success = true, message = "Update successful" });
        }


        //////// Recent Activity ////////
        ///
        [Authorize]
        [HttpGet("RecentActivity")]
        public async Task<IActionResult> GetRecentActivity()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized(new { message = "user not found" });
            }

            var items = await recentActivityService.GetRecentActivities(userId);
            var list = items.Select(l => new RecentActivityDto
            {
                Text = l.Text,
                Type = l.Type,
                Created=l.Created,
            });

            return Ok(list);
        }
        [Authorize]
        [HttpPost("NewRecentActivity")]

        public async Task<IActionResult> AddRecentActivity(RecentActivityDto dto)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized(new { message = "user not found" });
            }
            var item = new RecentActivity
            {
                UserId = userId,
                Text = dto.Text,
                Type = dto.Type,  
            };
            await recentActivityService.Add(item);
            return Ok(new { message = "Activity added successfully" });
        }


        // Bildirimleri Getirme
        [Authorize]
        [HttpGet("RequestNotification")]
        public async Task<IActionResult> GetRequestNotification()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized(new { message = "user not found" });
            }

            var items = await requestNotificationService.GetRequestNotifications(userId);
            var onlyNotAccepted=items.Where(t=>t.IsAccepted==false).ToList();
            var list = items.Select(l => new RequestNotificationDto
            {
                Text = l.Text,
                SenderId = l.SenderId,
                ReceiverEmail = l.Receiver.Email, 
                IsAccepted= l.IsAccepted,   
              
            });

            return Ok(list);
        }

        // request notification
        [Authorize]
        [HttpPost("NewRequestNotification")]
        public async Task<IActionResult> AddRequestNotification(RequestNotificationDto dto)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized(new { message = "user not found" });
            }

            var receiverUser = await _userManager.FindByEmailAsync(dto.ReceiverEmail);
            if (receiverUser == null)
            {
                return BadRequest(new { message = "Receiver not found" });
            }

            var item = new RequestNotification
            {
                Text = dto.Text,
                SenderId = userId,
                ReceiverId = receiverUser.Id,  
                IsAccepted=dto.IsAccepted,
            };

            await requestNotificationService.Add(item);

            return Ok(new
            {
                message = "Activity added successfully",
                data = new RequestNotificationDto
                {
                    Text = item.Text,
                   // SenderId = item.SenderId,
                    ReceiverEmail = receiverUser.Email, 
                }
            });
        }

    }
}
