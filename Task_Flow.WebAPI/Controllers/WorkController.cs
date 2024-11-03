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
    public class WorkController : ControllerBase
    {
        private readonly ITaskService taskService;
        private readonly IUserService userService;

        public WorkController(ITaskService taskService, IUserService userService)
        {
            this.taskService = taskService;
            this.userService = userService;
        }



        // GET: api/<WorkController>
        [Authorize]
        [HttpGet("UserTasks")]
        public async Task<IEnumerable<WorkDto>> GetUserTasks()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var list = await taskService.GetTasks(userId);
            var items = list.Select(p =>
            {
                return new WorkDto
                {
                    CreatedById = p.CreatedById,
                    Description = p.Description,
                    Deadline = p.Deadline,
                    Priority = p.Priority,
                    Status = p.Status,
                    Title = p.Title,
                    ProjectId = p.ProjectId,
                };
            });
            return items;
        }


        // GET api/<WorkController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await taskService.GetTaskById(id);
            if (item == null)
            {
                return NotFound();
            }
            var work = new WorkDto
            {
                CreatedById = item.CreatedById,
                Description = item.Description,
                Deadline = item.Deadline,
                Priority = item.Priority,
                Status = item.Status,
                Title = item.Title,
                ProjectId = item.ProjectId,
            };
            return Ok(work);
        }
        [Authorize]
        [HttpGet("UserTasksCount")]
        public async Task<IActionResult> GetUserTaskCount()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var item = await taskService.GetTasks(userId);
            
            return Ok(item.Count());
        }

        // POST api/<WorkController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] WorkDto value)
        {

            var item = new Work
            {
                CreatedById = value.CreatedById,
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

        // PUT api/<WorkController>/5
        [HttpPut("ChangeTitle/{id}")]
        public async Task<IActionResult> PutTitle(int id, [FromBody] string value)
        {
            var item = await taskService.GetTaskById(id);

            if (item == null)
            {
                return NotFound();
            } 
            //item.Description = value.Description;
            //item.Deadline = value.Deadline;
            //   item.Priority = value.Priority;
            //item.Status = value.Status;
            item.Title = value; 
            await taskService.Update(item);
            return Ok();
        }
        [HttpPut("ChangeDescription/{id}")]
        public async Task<IActionResult> PutDescription(int id, [FromBody] string value)
        {
            var item = await taskService.GetTaskById(id);

            if (item == null)
            {
                return NotFound();
            }
            item.Description = value; 
            await taskService.Update(item);
            return Ok();
        }

        [HttpPut("ChangeDeadLine/{id}")]
        public async Task<IActionResult> PutDeadLine(int id, [FromBody] DateTime value)
        {
            var item = await taskService.GetTaskById(id);

            if (item == null)
            {
                return NotFound();
            } 
            item.Deadline = value; 
            await taskService.Update(item);
            return Ok();
        }

        [HttpPut("ChangeStatus/{id}")]
        public async Task<IActionResult> PutStatus(int id, [FromBody] string value)
        {
            var item = await taskService.GetTaskById(id);

            if (item == null)
            {
                return NotFound();
            } 
            item.Status = value; 
            await taskService.Update(item);
            return Ok();
        }
        [HttpPut("ChangePriority/{id}")]
        public async Task<IActionResult> PutPriority(int id, [FromBody] string value)
        {
            var item = await taskService.GetTaskById(id);

            if (item == null)
            {
                return NotFound();
            }
              item.Priority = value; 
            await taskService.Update(item);
            return Ok();
        }

        // DELETE api/<WorkController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await taskService.GetTaskById(id);
            if (item == null)
            {
                return NotFound();
            }
            await taskService.Delete(item);
            return Ok(item);
        }
        [Authorize]
        [HttpGet("DailyTask")]
        public async Task<IActionResult> GetDailyTask()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tasks=await taskService.GetTasks(userId);
            var todayTasks=tasks.OrderBy(t=>t.StartTime).ToList();
            return Ok(todayTasks);

        }

        [Authorize]
        [HttpGet("ToDoTask")]
        public async Task<IActionResult> GetToDoTask()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tasks = await taskService.GetToDoTask(userId); 
            return Ok(tasks);

        }

        [Authorize]
        [HttpGet("InProgressTask")]
        public async Task<IActionResult> GetInProgressTask()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tasks = await taskService.GetInProgressTask(userId); 
            return Ok(tasks);

        }

        [Authorize]
        [HttpGet("DoneTask")]
        public async Task<IActionResult> GetDoneTask()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tasks = await taskService.GetDoneTask(userId); 
            return Ok(tasks);

        }

    }
}
