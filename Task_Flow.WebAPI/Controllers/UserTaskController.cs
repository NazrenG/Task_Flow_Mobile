﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
//using Task_Flow.WebAPI.Hubs;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTaskController : ControllerBase
    {
        private readonly IUserTaskService userTaskService;
        private readonly IUserService userService;
        private readonly UserManager<CustomUser> _userManager;
        //private readonly IHubContext<ConnectionHub> _context;
        private readonly IProjectService _projectService;

        public UserTaskController(IUserTaskService userTaskService, IUserService userService, UserManager<CustomUser> userManager,  IProjectService projectService)
        {
            this.userTaskService = userTaskService;
            this.userService = userService;
            _userManager = userManager;
            //_context = context;
            _projectService = projectService;
        }


        // GET: api/<WorkController>
        [Authorize]
        [HttpGet("UserTasks")]
        public async Task<IActionResult> GetUserTasks()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "error.userNotAuth" });
            }

            var list = await userTaskService.GetUserTasks(userId);
            var items = list.Select(p =>
            {
                return new WorkDto
                {
                    Id = p.Id,
                    CreatedById = userId,
                    Description = p.Description,
                    Deadline = p.Deadline,
                    Priority = p.Priority,
                    Status = p.Status,
                    Title = p.Title,
                    StartDate = p.StartTime,
                    Color = p.Color,
                };
            }).ToList();
            return Ok(items);
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserTask(int id)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "error.userNotAuth" });
            }
            var item = await userTaskService.GetById(id);

            var work = new WorkDto
            {
                CreatedById = userId,
                Description = item.Description,
                Deadline = item.Deadline,
                Priority = item.Priority,
                Status = item.Status,
                Title = item.Title,
                StartDate = item.StartTime,
                Color = item.Color,
            };
            return Ok(work);
        }
        [Authorize]
        [HttpPut("EditedTaskForCalendar/{id}")]
        public async Task<IActionResult> PutEditTaskForCalendar(int id, [FromBody] EditForCalendarDto value)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "error.userNotAuth" });
            }
            var item = await userTaskService.GetById(id);
 
            item.Deadline = value.Deadline; 
            item.StartTime = value.StartDate; 

            await userTaskService.Update(item); 
        //    await _context.Clients.User(userId).SendAsync("UserTaskList"); 



            return Ok(new { message = "succesfuly.updatesuccesfuly" });
        }


        [Authorize]
        [HttpPut("EditedTask/{id}")]
        public async Task<IActionResult> PutEditTask(int id, [FromBody] WorkDto value)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "error.userNotAuth" });
            }
            var item = await userTaskService.GetById(id); 

            item.Description = value.Description;
            item.Deadline = value.Deadline;
            item.Priority = value.Priority;
            item.Status = value.Status;
            item.Title = value.Title;
            item.StartTime = value.StartDate;
            item.Color = value.Color;

            await userTaskService.Update(item);
            //TaskTotalCount OnHoldTaskCount RunningTaskCount CompletedTaskCount
         
            //await _context.Clients.User(userId).SendAsync("UserTaskList");
            //await _context.Clients.User(userId).SendAsync("OnHoldTaskCount");
            //await _context.Clients.User(userId).SendAsync("RunningTaskCount");
            //await _context.Clients.User(userId).SendAsync("CompletedTaskCount");

            ////canban ucun signalr
            //await _context.Clients.User(userId).SendAsync("CanbanTaskUpdated"); 

            ////project ve view detail sehifesindeki task list
            //await _context.Clients.User(item.CreatedById).SendAsync("ProjectsTaskList");
            //await _context.Clients.User(item.CreatedById).SendAsync("ProjectDetailTaskList");

            ////view profil sehifesindeki task list 
            //await _context.Clients.User(item.CreatedById).SendAsync("UserProfileTask");

            ////dashboard-da current project
            //await _context.Clients.User(item.CreatedById).SendAsync("DashboardReceiveProject");
            ////project activity log signalr detail sehifesi
            //await _context.Clients.User(item.CreatedById).SendAsync("ProjectRecentActivityInDetail");
            //await _context.Clients.User(userId).SendAsync("ProjectRecentActivityInDetail");
            ////project activity log signalr project sehifesi
            //await _context.Clients.User(item.CreatedById).SendAsync("ProjectsRecentActivity");
            //await _context.Clients.User(userId).SendAsync("ProjectsRecentActivity");
            return Ok(new { message = "succesfuly.updatesuccesfuly" });
        }


        [Authorize]
        [HttpGet("UserTasksCount")]
        public async Task<IActionResult> GetUserTaskCount()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "error.userNotAuth" });
            }

            var item = await userTaskService.GetUserTasks(userId);

            return Ok(item.Count());
        }
        

        [Authorize]
        [HttpPost("NewTask")]
        public async Task<IActionResult> Post([FromBody] WorkDto value)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "error.userNotAuth" });
            }
            var checkTitle = await userTaskService.CheckTaskName(value.Title);
            if (checkTitle)
            {
                return BadRequest(new { message = "error.task.thisTitleAlready" });
            }

            var item = new UserTask
            {
                CreatedById = userId,
                Description = value.Description,
                Deadline = value.Deadline,
                Priority = value.Priority,
                Status = "to do",
                Title = value.Title,
                Color = value.Color,
                StartTime = value.StartDate
            };
            await userTaskService.Add(item);
            //TaskTotalCount OnHoldTaskCount RunningTaskCount CompletedTaskCount
            //await _context.Clients.User(userId).SendAsync("TaskTotalCount");

            //await _context.Clients.User(userId).SendAsync("OnHoldTaskCount");
            //await _context.Clients.User(userId).SendAsync("UserTaskList");


            return Ok(item);
        }
        [Authorize]
        [HttpDelete("DeleteUserTask/{taskId}")]
        public async Task<IActionResult> Delete(int taskId)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "error.userNotAuth" });
            }
            var item = await userTaskService.GetById(taskId);
            if (item == null)
            {
                return NotFound();
            }
            await userTaskService.Delete(item);
            //TaskTotalCount OnHoldTaskCount RunningTaskCount CompletedTaskCount
         
          //  if (item.Status=="to do") await _context.Clients.User(userId).SendAsync("OnHoldTaskCount");
          // else if (item.Status=="in progress") await _context.Clients.User(userId).SendAsync("RunningTaskCount");
          //else  if (item.Status=="done") await _context.Clients.User(userId).SendAsync("CompletedTaskCount");
          //  await _context.Clients.User(userId).SendAsync("TaskTotalCount");
            return Ok(new { message = "succesfuly.deletesuccesfuly" });
        }
        [Authorize]
        [HttpGet("DailyTask")]
        public async Task<IActionResult> GetDailyTask()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return BadRequest(new { message = "error.userNotAuth" });
            }
            var tasks = await userTaskService.GetUserTasks(userId);
            var todayTasks = tasks.Where(d => d.Deadline.Date == DateTime.Now.Date).OrderBy(t => t.StartTime).ToList();
            return Ok(todayTasks);

        }

        [Authorize]
        [HttpGet("ToDoTask")]
        public async Task<IActionResult> GetToDoTask()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "error.userNotAuth" });
            }
            var tasks = await userTaskService.GetToDoTask(userId);
            return Ok(tasks);

        }
        [Authorize]
        [HttpGet("ToDoTaskCount")]
        public async Task<IActionResult> GetToDoTaskCount()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "error.userNotAuth" });
            }
            var tasks = await userTaskService.GetToDoTask(userId);
            return Ok(tasks.Count);

        }

        [HttpGet("ToDoTaskCountForMail/{email}")]
        public async Task<IActionResult> GetToDoTaskCountForMail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest(new { message = "error.usernotFound" });
            }
            var tasks = await userTaskService.GetToDoTask(user.Id);
            return Ok(tasks.Count);

        }


        [Authorize]
        [HttpGet("InProgressTask")]
        public async Task<IActionResult> GetInProgressTask()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "error.userNotAuth" });
            }
            var tasks = await userTaskService.GetInProgressTask(userId);
            return Ok(tasks);

        }
        [Authorize]
        [HttpGet("InProgressTaskCount")]
        public async Task<IActionResult> GetInProgressTaskCountForEmail()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "error.userNotAuth" });
            }
            var tasks = await userTaskService.GetInProgressTask(userId);
            return Ok(tasks.Count);

        }
        [HttpGet("InProgressTaskCountForEmail/{email}")]
        public async Task<IActionResult> GetInProgressTaskCount(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest(new { message = "error.usernotFound" });
            }
            var tasks = await userTaskService.GetInProgressTask(user.Id);
            return Ok(tasks.Count);

        }

        [Authorize]
        [HttpGet("DoneTask")]
        public async Task<IActionResult> GetDoneTask()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "error.userNotAuth" });
            }
            var tasks = await userTaskService.GetDoneTask(userId);
            return Ok(tasks);

        }
        [Authorize]
        [HttpGet("DoneTaskCount")]
        public async Task<IActionResult> GetDoneTaskCount()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "error.userNotAuth" });
            }
            var tasks = await userTaskService.GetDoneTask(userId);
            return Ok(tasks.Count);

        }
        [HttpGet("DoneTaskCountForEmail/{email}")]
        public async Task<IActionResult> GetDoneTaskCountForEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest(new { message = "error.usernotFound" });
            }

            var tasks = await userTaskService.GetDoneTask(user.Id);
            return Ok(tasks.Count);

        }
    }
}
