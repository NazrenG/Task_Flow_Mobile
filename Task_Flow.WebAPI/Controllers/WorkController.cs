using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Task_Flow.Business.Abstract;
using Task_Flow.Business.Cocrete;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Data;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Hubs;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkController : ControllerBase
    {
        private readonly ITaskService taskService;
        private readonly IUserService userService;
        private readonly UserManager<CustomUser> _userManager;
        private readonly IProjectService projectService;
        private readonly MailService mailService;
        private readonly INotificationService notificationService;
        private readonly IHubContext<ConnectionHub> _context;
        private readonly TaskFlowDbContext _dbContext;

        public WorkController(ITaskService taskService, IUserService userService, UserManager<CustomUser> userManager, IProjectService projectService, MailService mailService, INotificationService notificationService, IHubContext<ConnectionHub> context, TaskFlowDbContext dbContext)
        {
            this.taskService = taskService;
            this.userService = userService;
            _userManager = userManager;
            this.projectService = projectService;
            this.mailService = mailService;
            this.notificationService = notificationService;
            _context = context;
            _dbContext = dbContext;
        }






        // GET: api/<WorkController>
        [Authorize]
        [HttpGet("UserTasks")]
        public async Task<IActionResult> GetUserTasks()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }

            var list = await taskService.GetTasks(userId);
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
                    ProjectId = p.ProjectId, 
                    StartDate =p.StartTime,
                };
            }).ToList();
            return Ok(items);
        }
        [HttpGet("UserProfileTask/{email}")]
        public async Task<IActionResult> GetUserProfileTask(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var list = await taskService.GetTasks(user.Id);
            var items = list.Select(p =>
            {
                return new WorkDto
                {
                  
                    CreatedById = user.Id,
                    Description = p.Description,
                    Deadline = p.Deadline,
                    Priority = p.Priority,
                    Status = p.Status,
                    Title = p.Title,
                    ProjectId = p.ProjectId,
                    StartDate = p.StartTime,
                };
            }).ToList();
            return Ok(items);
        }

        // GET api/<WorkController>/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }
            var item = await taskService.GetTaskById(id);
            if (item == null)
            {
                return NotFound();
            }
            var work = new WorkDto
            {

                CreatedById = userId,
                Description = item.Description,
                Deadline = item.Deadline,
                Priority = item.Priority,
                Status = item.Status,
                Title = item.Title,
                ProjectId = item.ProjectId,
                StartDate = item.StartTime,
                Color = item.Color,
            };
            return Ok(work);
        }

        //// PUT api/<WorkController>/5
        [Authorize]
        [HttpPut("EditedProjectTask/{id}")]
        public async Task<IActionResult> PutProjectTaskByUser(int id, [FromBody] WorkDto value)
        {

            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }
            var item = await taskService.GetTaskById(id);

            if (item == null)
            {
                return NotFound();
            }
            item.Priority = value.Priority;
            item.Status = value.Status;
            await taskService.Update(item);
            await _context.Clients.User(userId).SendAsync("OnHoldTaskCount");
            await _context.Clients.User(userId).SendAsync("RunningTaskCount");
            await _context.Clients.User(userId).SendAsync("CompletedTaskCount");
            return Ok(new {message="update succesfuly"});
        }
        //canban icinde tasklari edit etmek
        [Authorize]
        [HttpPut("EditedProjectForPmTask/{id}")]
        public async Task<IActionResult> PutProjectForPmTask(int id, [FromBody] WorkDto value)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }
            var item = await taskService.GetTaskById(id);
            var project=await projectService.GetProjectById(value.ProjectId);
           

            if (item == null)
            {
                return NotFound();
            }

            if (project.CreatedById != userId) return BadRequest(new { message = "You are not access edit task" });

            item.Title = value.Title;
            item.Description = value.Description;
            item.Deadline = value.Deadline;
            item.Color = value.Color;
            item.CreatedById = value.CreatedById;
            item.StartTime = value.StartDate;

            item.Priority = value.Priority;
            item.Status = value.Status;
            await taskService.Update(item);
            return Ok(new { message = "update succesfuly" });
        }

        [Authorize]
        [HttpGet("UserTasksCount")]
        public async Task<IActionResult> GetUserTaskCount()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }

            var item = await taskService.GetTasks(userId);
            
            return Ok(item.Count());
        }

        [HttpGet("UserTasksCountForEmail/{email}")]
        public async Task<IActionResult> GetUserTaskCountForEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest(new { message = "User not found." });
            }

            var item = await taskService.GetTasks(user.Id);

            return Ok(item.Count());
        }

        // POST api/<WorkController>
        [Authorize]
        [HttpPost("NewTask")]
        public async Task<IActionResult> Post([FromBody] WorkDto value)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }
            var member = await userService.GetUserById(value.CreatedById);
            var project = await projectService.GetProjectNameById(value.ProjectId);
            var projectCreater=await projectService.GetProjectById(value.ProjectId);

            if(projectCreater.CreatedById!=userId) return BadRequest("You do not have permission to update tasks in this project.");
            var item = new Work
            {
                CreatedById = value.CreatedById,
                Description = value.Description,
                Deadline = value.Deadline,
                Priority = value.Priority,
                Status = value.Status,
                Title = value.Title,
                Color = value.Color,
                ProjectId = value.ProjectId,
            };
            await taskService.Add(item);
            await notificationService.Add(new Notification
            { 
                UserId = value.CreatedById,
                Text = $"You have a new task in the project named {projectCreater.Title}",
                IsCalendarMessage=true,
                
            }); 
            await _context.Clients.User(value.CreatedById).SendAsync("DashboardCalendarNotificationCount");
            mailService.SendEmail(member.Email, $"Hi,{member.Firstname} {member.Lastname}.You have a new task in the project named {project} ");
            return Ok(item);
        }



        // DELETE api/<WorkController>/5
        [Authorize]
        [HttpDelete("DeleteProjectTask/{taskId}")]
        public async Task<IActionResult> Delete(int taskId, [FromQuery] int projectId)
        {
            var item = await taskService.GetTaskById(taskId);
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }
            var project=await projectService.GetProjectById(projectId);
            if(project.CreatedById!=userId) return BadRequest("You do not have permission to delete tasks in this project.");
            if (item == null)
            {
                return NotFound();
            }
            await taskService.Delete(item);
            return Ok(new {message="delete succesful"});
        }
        [Authorize]
        [HttpGet("DailyTask")]
        public async Task<IActionResult> GetDailyTask()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tasks=await taskService.GetTasks(userId);
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }
            var todayTasks=tasks.Where(d=>d.Deadline.Date==DateTime.Now.Date).OrderBy(t=>t.StartTime).ToList();
            return Ok(todayTasks); 

        }

        [Authorize]
        [HttpGet("ToDoTask")]
        public async Task<IActionResult> GetToDoTask()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }
            var tasks = await taskService.GetToDoTask(userId); 
            return Ok(tasks);

        }
        [Authorize]
        [HttpGet("ToDoTaskCount")]
        public async Task<IActionResult> GetToDoTaskCount()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }
            var tasks = await taskService.GetToDoTask(userId);
            return Ok(tasks.Count);

        } 

        [HttpGet("ToDoTaskCountForMail/{email}")]
        public async Task<IActionResult> GetToDoTaskCountForMail(string email)
        { 
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest(new { message = "User not found." });
            }
            var tasks = await taskService.GetToDoTask(user.Id);
            return Ok(tasks.Count);

        }


        [Authorize]
        [HttpGet("InProgressTask")]
        public async Task<IActionResult> GetInProgressTask()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }
            var tasks = await taskService.GetInProgressTask(userId); 
            return Ok(tasks);

        }
        [Authorize]
        [HttpGet("InProgressTaskCount")]
        public async Task<IActionResult> GetInProgressTaskCountForEmail()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }
            var tasks = await taskService.GetInProgressTask(userId);
            return Ok(tasks.Count);

        }
        [HttpGet("InProgressTaskCountForEmail/{email}")]
        public async Task<IActionResult> GetInProgressTaskCount(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest(new { message = "User not found." });
            }
            var tasks = await taskService.GetInProgressTask(user.Id);
            return Ok(tasks.Count);

        }

        [Authorize]
        [HttpGet("DoneTask")]
        public async Task<IActionResult> GetDoneTask()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }
            var tasks = await taskService.GetDoneTask(userId); 
            return Ok(tasks);

        }
        [Authorize]
        [HttpGet("DoneTaskCount")]
        public async Task<IActionResult> GetDoneTaskCount()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }
            var tasks = await taskService.GetDoneTask(userId);
            return Ok(tasks.Count);

        }
        [HttpGet("DoneTaskCountForEmail/{email}")]
        public async Task<IActionResult> GetDoneTaskCountForEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest(new { message = "User not found." });
            }
          
            var tasks = await taskService.GetDoneTask(user.Id);
            return Ok(tasks.Count);

        }

        [Authorize] 
        [HttpGet("UserWorks")]
        public async Task<IActionResult> GetUserWorks()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }

            var works = await _dbContext.Works
                .Include(w => w.Project)
                .Include(w => w.CreatedBy) 
                .Where(w => w.Project.CreatedById == userId)
                .ToListAsync();

            if (works == null || !works.Any())
            {
                return NotFound("No works found for the current user.");
            }

            var dtoList = works.Select(work => new WorkDetailsDto
            {
                    TaskId=work.Id,
                ProjectId=work.ProjectId,
                ProjectName = work.Project?.Title,
                MemberName =$"{work.CreatedBy?.Firstname} {work.CreatedBy?.Lastname}" ,  
                MemberImage = work.CreatedBy?.Image,  
                MemberMail = work.CreatedBy?.Email,  
                TaskTitle = work.Title,
                StartTime = work.StartTime ?? DateTime.Now,
                Deadline = work.Deadline,
                Status = work.Status,
                Priority = work.Priority
            }).ToList();

            return Ok(dtoList);
        }


    }
}
