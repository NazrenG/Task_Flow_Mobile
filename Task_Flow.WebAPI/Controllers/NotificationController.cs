using Task_Flow.Business.Cocrete;
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
        private readonly IFriendService friendService;
        private readonly MailService mailService;

        public NotificationController(INotificationService notificationService, IUserService userService, INotificationSettingService notificationSettingService, IRecentActivityService recentActivityService, IRequestNotificationService requestNotificationService, UserManager<CustomUser> userManager, IFriendService friendService, MailService mailService)
        {
            this.notificationService = notificationService;
            this.userService = userService;
            this.notificationSettingService = notificationSettingService;
            this.recentActivityService = recentActivityService;
            this.requestNotificationService = requestNotificationService;
            _userManager = userManager;
            this.friendService = friendService;
            this.mailService = mailService;
            
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
            var items = list.Where(i => i.IsAccepted == false).Select(p =>
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

           
            var list = await notificationService.GetNotifications();
            var items = list.Where(i => i.UserId == userId && i.IsCalendarMessage == true).Select(p =>
            {
                return new
                {
                    Id=p.Id,
                    Text = p.Text,
                    Date=p.Created,
                };
            }).ToList();
            return Ok(items);
        }
        [HttpDelete("DeletedCalendarMessage/{id}")]
        public async Task<IActionResult> DeletedCalendarMessage(int id)
        {
            var item = await notificationService.GetNotificationById(id);
            if (item == null) { return BadRequest(new { message = "not found message" }); }
            await notificationService.Delete(item);
            return Ok(new { message="delete message succesfuly"});
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

            return Ok(list.Where(l => l.IsAccepted == false).Count());
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
        public async Task<IActionResult> Delete(int id)
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
                var newNotificationSetting = new NotificationSetting
                {
                    UserId=userId,
                    NewTaskWithInProject = dto.NewTaskWithInProject,
                    FriendshipOffers = dto.FriendshipOffers,
                    ProjectCompletationDate = dto.ProjectCompletationDate,
                    TaskDueDate = dto.TaskDueDate,
                    InnovationNewProject = dto.InnovationNewProject,
                };
                await notificationSettingService.Add(newNotificationSetting);
                return Ok(new { message = "new notification service." });
            }

            item.NewTaskWithInProject = dto.NewTaskWithInProject;
            item.FriendshipOffers = dto.FriendshipOffers;
            item.ProjectCompletationDate = dto.ProjectCompletationDate;
            item.InnovationNewProject = dto.InnovationNewProject;
            item.TaskDueDate = dto.TaskDueDate;
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
                Created = l.Created,
            }).ToList();

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


        // request notification
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
            var onlyNotAccepted = items.Where(t => t.IsAccepted == false).ToList();
            var list = items.Select(l => new
            {
                RequestId=l.Id,
                Text = l.Text,
                SenderName = $"{l.Sender.Firstname} {l.Sender.Lastname}",
                Image = l.Sender.Image,

            }).ToList();

            return Ok(list);
        }


        [Authorize]
        [HttpPost("NewRequestNotification")]
        public async Task<IActionResult> AddRequestNotification(RequestNotificationDto dto)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized(new { message = "user not found" });
            }
            var sender=await userService.GetUserById(userId);
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
                IsAccepted = dto.IsAccepted,
            };

            await requestNotificationService.Add(item);

            mailService.SendEmail(receiverUser.Email, $"You have new request to {sender.Firstname} {sender.Lastname} ");


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
        [HttpDelete("DeleteRequestNotification/{requestId}")]
        public async Task<IActionResult> DeleteRequestNotification(int requestId)
        {

            var request = await requestNotificationService.GetRequestNotificationById(requestId);
            if (request == null) { return BadRequest(new { message = "request not found" }); }
            await requestNotificationService.Delete(request);
            return Ok(new { message = "delete request notification succesfully" });
        }
        [Authorize]
        [HttpPut("AcceptRequestNotification/{requestId}")]
        public async Task<IActionResult> PutAcceptRequestNotification(int requestId)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized(new { message = "user not found" });
            }
            var request = await requestNotificationService.GetRequestNotificationById(requestId);
            if (request == null) { return BadRequest(new { message = "request not found" }); }
            request.IsAccepted = true;
            await requestNotificationService.Update(request);
            await friendService.Add(new Friend
            {
                UserId = userId,
                UserFriendId = request.SenderId,
            });

            return Ok(new { message = "accept request succesfuly" });
        }

    }
}
