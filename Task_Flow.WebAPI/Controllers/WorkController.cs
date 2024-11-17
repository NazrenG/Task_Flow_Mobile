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
    public class WorkController : ControllerBase
    {
        private readonly ITaskService taskService;
        private readonly IUserService userService;
        private readonly UserManager<CustomUser> _userManager;

        public WorkController(ITaskService taskService, IUserService userService,UserManager<CustomUser> userManager)
        {
            this.taskService = taskService;
            this.userService = userService;
            this._userManager = userManager;
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
                    StartDate=p.StartTime,
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



        //// GET api/<WorkController>/5
        //[HttpGet("{id}")]
        //public async Task<IActionResult> Get(int id)
        //{
        //    var item = await taskService.GetTaskById(id);
        //    if (item == null)
        //    {
        //        return NotFound();
        //    }
        //    var work = new WorkDto
        //    {
        //        CreatedById = item.CreatedById,
        //        Description = item.Description,
        //        Deadline = item.Deadline,
        //        Priority = item.Priority,
        //        Status = item.Status,
        //        Title = item.Title,
        //        ProjectId = item.ProjectId,
        //    };
        //    return Ok(work);
        //}
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


            var item = new Work
            {
                CreatedById = userId,
                Description = value.Description,
                Deadline = value.Deadline,
                Priority = value.Priority,
                Status = value.Status,
                Title = value.Title,
                ProjectId = value.ProjectId,
            };
            await taskService.Add(item);
            return Ok(item);
        }

        //// PUT api/<WorkController>/5
        //[HttpPut("ChangeTitle/{id}")]
        //public async Task<IActionResult> PutTitle(int id, [FromBody] string value)
        //{
        //    var item = await taskService.GetTaskById(id);

        //    if (item == null)
        //    {
        //        return NotFound();
        //    } 
        //    //item.Description = value.Description;
        //    //item.Deadline = value.Deadline;
        //    //   item.Priority = value.Priority;
        //    //item.Status = value.Status;
        //    item.Title = value; 
        //    await taskService.Update(item);
        //    return Ok();
        //}
        //[HttpPut("ChangeDescription/{id}")]
        //public async Task<IActionResult> PutDescription(int id, [FromBody] string value)
        //{
        //    var item = await taskService.GetTaskById(id);

        //    if (item == null)
        //    {
        //        return NotFound();
        //    }
        //    item.Description = value; 
        //    await taskService.Update(item);
        //    return Ok();
        //}

        //[HttpPut("ChangeDeadLine/{id}")]
        //public async Task<IActionResult> PutDeadLine(int id, [FromBody] DateTime value)
        //{
        //    var item = await taskService.GetTaskById(id);

        //    if (item == null)
        //    {
        //        return NotFound();
        //    } 
        //    item.Deadline = value; 
        //    await taskService.Update(item);
        //    return Ok();
        //}

        //[HttpPut("ChangeStatus/{id}")]
        //public async Task<IActionResult> PutStatus(int id, [FromBody] string value)
        //{
        //    var item = await taskService.GetTaskById(id);

        //    if (item == null)
        //    {
        //        return NotFound();
        //    } 
        //    item.Status = value; 
        //    await taskService.Update(item);
        //    return Ok();
        //}
        //[HttpPut("ChangePriority/{id}")]
        //public async Task<IActionResult> PutPriority(int id, [FromBody] string value)
        //{
        //    var item = await taskService.GetTaskById(id);

        //    if (item == null)
        //    {
        //        return NotFound();
        //    }
        //      item.Priority = value; 
        //    await taskService.Update(item);
        //    return Ok();
        //}

        // DELETE api/<WorkController>/5
        [HttpDelete("DeleteProjectTask/{taskId}")]
        public async Task<IActionResult> Delete(int taskId)
        {
            var item = await taskService.GetTaskById(taskId);
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

    }
}
