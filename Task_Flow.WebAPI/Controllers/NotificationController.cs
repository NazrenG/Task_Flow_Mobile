﻿using Task_Flow.Business.Cocrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.DataAccess.Concrete;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
//using Microsoft.AspNetCore.SignalR;
//using Task_Flow.WebAPI.Hubs;

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
        //private readonly IHubContext<ConnectionHub> _hub;
        private readonly IProjectService projectService;
        private readonly MailService mailService;
        private readonly ITeamMemberService memberService;

        public NotificationController(INotificationService notificationService, IUserService userService, INotificationSettingService notificationSettingService, IRecentActivityService recentActivityService, IRequestNotificationService requestNotificationService, UserManager<CustomUser> userManager, IFriendService friendService, MailService mailService, ITeamMemberService memberService, IProjectService projectService)
        {
            this.notificationService = notificationService;
            this.userService = userService;
            this.notificationSettingService = notificationSettingService;
            this.recentActivityService = recentActivityService;
            this.requestNotificationService = requestNotificationService;
            _userManager = userManager;
            this.friendService = friendService;
            this.mailService = mailService;
            //_hub = hub;
            this.memberService = memberService;
            this.projectService = projectService;
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
                return BadRequest(new { message = "error.userNotAuth" });
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
                return BadRequest(new { message = "error.userNotAuth" });
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
                return BadRequest(new { message = "error.userNotAuth" });
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
        [Authorize]
        [HttpDelete("DeletedCalendarMessage/{id}")]
        public async Task<IActionResult> DeletedCalendarMessage(int id)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "error.userNotAuth" });
            }
            var item = await notificationService.GetNotificationById(id);
            if (item == null) { return BadRequest(new { message = "error.notification.notFoundMessage" }); }
            await notificationService.Delete(item);
            //await _hub.Clients.User(userId).SendAsync("ReminderRequestList");
            //await _hub.Clients.User(userId).SendAsync("CalendarNotificationCount");
            //await _hub.Clients.User(userId).SendAsync("CalendarNotificationList2");
            ////userin loglari
            //await _hub.Clients.User(userId).SendAsync("RecentActivityUpdate1");
            return Ok(new { message= "succesfuly.notification.deletemessage" });
        }

        [Authorize]
        [HttpGet("TwoCalendarNotification")]
        public async Task<IActionResult> TakeTwoCalendarNotification()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "error.userNotAuth" });
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
                return Unauthorized(new { message = "error.usernotFound" });
            }

            var item = await notificationSettingService.GetOrCreateNotificationSetting(userId);

            return Ok(new { success = true, message = "succesfuly.notification.notificationsetting" });
        }
        [Authorize]
        [HttpPost("UpdatedNotificationSetting")]
        public async Task<IActionResult> UpdateNotificationSetting(NotificationSettingDto dto)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized(new { message = "error.usernotFound" });
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
                return Ok(new { message = "succesfuly.notification.newnotification" });
            }

            item.NewTaskWithInProject = dto.NewTaskWithInProject;
            item.FriendshipOffers = dto.FriendshipOffers;
            item.ProjectCompletationDate = dto.ProjectCompletationDate;
            item.InnovationNewProject = dto.InnovationNewProject;
            item.TaskDueDate = dto.TaskDueDate;
            await notificationSettingService.Update(item);
            //await _hub.Clients.User(userId).SendAsync("RecentActivityUpdate1"); 


            return Ok(new { success = true, message = "succesfuly.updatesuccesfuly" });
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
                return Unauthorized(new { message = "error.usernotFound" });
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
                return Unauthorized(new { message = "error.usernotFound" });
            }
            var item = new RecentActivity
            {
                UserId = userId,
                Text = dto.Text,
                Type = dto.Type,
            };
            await recentActivityService.Add(item);
            return Ok(new { message = "succesfuly.notification.activityAdded" });
        }


        // request notification
        [Authorize]
        [HttpGet("RequestNotification")]
        public async Task<IActionResult> GetRequestNotification()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized(new { message = "error.usernotFound" });
            }

       

            var items = await requestNotificationService.GetRequestNotifications(userId);
            var onlyNotAccepted = items.Where(t => t.IsAccepted == false).ToList();
            var list = onlyNotAccepted.Select(l => new
            {
                RequestId=l.Id,
                Text = l.Text,
                SenderName = $"{l.Sender.Firstname} {l.Sender.Lastname}",
                Image = l.Sender.Image,
                Typee=l.NotificationType,
                

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
                return Unauthorized(new { message = "error.usernotFound" });
            }
            var sender=await userService.GetUserById(userId);
            var receiverUser = await _userManager.FindByEmailAsync(dto.ReceiverEmail);
            if (receiverUser == null)
            {
                return BadRequest(new { message = "error.notification.receiverNotFound" });
            }

            var item = new RequestNotification
            {
                Text = dto.Text,
                SenderId = userId,
                ReceiverId = receiverUser.Id,
                IsAccepted = dto.IsAccepted,
                NotificationType=dto.NotificationType,
            };

            await requestNotificationService.Add(item);
            //notification list
            //await _hub.Clients.User(receiverUser.Id).SendAsync("RequestList2");
            //await _hub.Clients.User(receiverUser.Id).SendAsync("RequestCount");
            //await _hub.Clients.User(receiverUser.Id).SendAsync("RequestList");


            //await _hub.Clients.User(sender.Id).SendAsync("InwokeSendFollow",receiverUser.Id);
            if(dto.NotificationType== "FriendRequest")
            {
   mailService.SendEmail(receiverUser.Email, $"You have new friendship request to {sender.Firstname} {sender.Lastname} ");

            } 
            else
            {
                mailService.SendEmail(receiverUser.Email, $"You have a new project proposal from {sender.Firstname} {sender.Lastname}");
            }
            return Ok(new
            {
                message = "succesfuly.notification.activityAdded",
                data = new RequestNotificationDto
                {
                    Text = item.Text,
                    // SenderId = item.SenderId,
                    ReceiverEmail = receiverUser.Email,
                }
            });
        }
        [Authorize]
        [HttpDelete("DeleteRequestNotification/{requestId}")]
        public async Task<IActionResult> DeleteRequestNotification(int requestId)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized(new { message = "error.usernotFound" });
            }

            var request = await requestNotificationService.GetRequestNotificationById(requestId);
            if (request == null) { return BadRequest(new { message = "error.notification.requestNotFaund" }); }
         

            await requestNotificationService.Delete(request);
            //await _hub.Clients.User(userId).SendAsync("RequestList2");
            //await _hub.Clients.User(userId).SendAsync("RequestCount");
            //await _hub.Clients.User(userId).SendAsync("RequestList");  
            var item = new RecentActivity
            {
                UserId = userId,
                Text = "Delete request",
                Type = "Notification",
            };
            await recentActivityService.Add(item);
           // await _hub.Clients.User(userId).SendAsync("RecentActivityUpdate1");
            return Ok(new { message = "succesfuly.notification.deleteRequest" });
        }
        [Authorize]
        [HttpPut("AcceptRequestNotification/{requestId}")]
        public async Task<IActionResult> PutAcceptRequestNotification(int requestId)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized(new { message = "error.usernotFound" });
            }
            var request = await requestNotificationService.GetRequestNotificationById(requestId);
            if (request == null) { return BadRequest(new { message = "error.notification.requestNotFaund" }); }
            request.IsAccepted = true;
            
            await requestNotificationService.Update(request);
            if(request.NotificationType == "ProjectRequest")
            {
                var project = await projectService.GetProjectByName(request.SenderId,request.ProjectName);
                await memberService.Add(new TeamMember
                {
                    ProjectId = project.Id,
                    UserId=request.ReceiverId
                });

            }
            else if(request.NotificationType == "FriendRequest")
            {
                await friendService.Add(new Friend
                {
                    UserId = request.SenderId,
                    UserFriendId= userId,
                    IsFriend=true,
                });

          //  await _hub.Clients.User(request.SenderId).SendAsync("UpdateMessageFriendList");
            }

            
            //await _hub.Clients.User(request.SenderId).SendAsync("UpdateUserActivity");
            //await _hub.Clients.User(userId).SendAsync("RequestList2");
            //await _hub.Clients.User(userId).SendAsync("RequestCount");
            //await _hub.Clients.User(userId).SendAsync("RequestList");
            var item = new RecentActivity
            {
                UserId = userId,
                Text = "Accept request",
                Type = "Notification",
            };
            await recentActivityService.Add(item);
            //await _hub.Clients.User(userId).SendAsync("RecentActivityUpdate1");

            return Ok(new { message = "succesfuly.notification.acceptRequest" });
        }
       

    }
}
