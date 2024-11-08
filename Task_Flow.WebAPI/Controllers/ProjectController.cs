using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.DataAccess.Concrete;
using Task_Flow.Entities.Data;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly TaskFlowDbContext _context;
        private readonly IUserService _userService;
        private readonly ITeamMemberService _teamMemberService;

        public ProjectController(TaskFlowDbContext dbContext,IProjectService projectService, IUserService userService, ITeamMemberService teamMemberService)
        {
            _projectService = projectService;
            _userService = userService;
            _teamMemberService = teamMemberService;
            _context = dbContext;
        }
        [Authorize]
        [HttpGet("ExtendedProjectList")]
        public async Task<IActionResult> GetExtendedProjectList()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid token or user not found.");
            }

            var item = await _userService.GetUserById(userId);
            if (item == null)
            {
                return NotFound("User not found.");
            }
             
            var projectList=await _projectService.GetProjects(userId);  
        var list=  projectList.Select(p => new ExtendedProjectListDto
                {
                    Title = p.Title,
                    TotalTask = p.TaskForUsers.Count, 
                    CompletedTask = p.TaskForUsers.Count(t => t.Status == "done"), // Tamamlanan görev sayısı
                    ParticipantsPath = p.TeamMembers
                        .Select(tm => tm.User.Image) 
                        .ToList(),
                    Color = p.Color,
                }).ToList() ;

            return Ok(list);
        }



        [Authorize]
        [HttpGet("UserProjectCount")]
        public async Task<IActionResult> GetUserProjectCount()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            if (userId == null)
            {
                return Unauthorized("Invalid token or user not found.");
            }
            // var item = await _userService.GetUserById(userId);
            var count = await _projectService.GetUserProjectCount(userId);

            return Ok(count);
        }

        // GET api/<ProjectController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _projectService.GetProjectById(id);
            if (item == null)
            {
                return NotFound();
            }
            var project = new ProjectDto
            {
                Owner = item.CreatedBy?.UserName,
                Description = item.Description,
                IsCompleted = item.IsCompleted,
                Title = item.Title,
            };
            return Ok(project);

        }
        [HttpGet("ProjectTaskCount/{id}")]
        public async Task<IActionResult> GetProjectTaskCount(int id)
        {

            var item= await _context.Projects
       .Include(p => p.TaskForUsers) 
       .FirstOrDefaultAsync(p => p.Id == id);
         //   var item = await _projectService.GetProjectById(id);
            if (item == null)
            {
                return NotFound();
            }
            var count = item.TaskForUsers?.Count;

            return Ok(count);

        }

        // POST api/<ProjectController>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProjectDto value)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            var item = new Project
            {
                CreatedById = userId,
                CreatedAt= DateTime.UtcNow, 
                StartDate=value.StartDate,
                EndDate=value.EndDate,
                Description = value.Description,
                IsCompleted = value.IsCompleted,
                Title = value.Title,
            };
            await _projectService.Add(item);
            return Ok(item);
        }

        // PUT api/<ProjectController>/5

        [HttpPut("ChangeTitle/{id}")]
        public async Task<IActionResult> PutTitle(int id, [FromBody] string value)
        {
            var item = await _projectService.GetProjectById(id);

            if (item == null)
            {
                return NotFound();
            }
            item.Title = value;
            await _projectService.Update(item);
            return Ok();
        }

        [HttpPut("ChangeDescription/{id}")]
        public async Task<IActionResult> PutDescription(int id, [FromBody] string value)
        {
            var item = await _projectService.GetProjectById(id);

            if (item == null)
            {
                return NotFound();
            }
            item.Description = value;
            await _projectService.Update(item);
            return Ok();
        }

        [HttpPut("ChangeCompleted/{id}")]
        public async Task<IActionResult> PutCompleted(int id, [FromBody] bool value)
        {
            var item = await _projectService.GetProjectById(id);

            if (item == null)
            {
                return NotFound();
            }
            item.IsCompleted = value;
            await _projectService.Update(item);
            return Ok();
        }
        // DELETE api/<ProjectController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _projectService.GetProjectById(id);
            if (item == null)
            {
                return NotFound();
            }
            await _projectService.Delete(item);
            return Ok(item);
        }


        [Authorize]
        [HttpGet("OnGoingProject")]
        public async Task<IActionResult> GetOnGoingProject()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var projects = await _projectService.GetOnGoingProject(userId);
            return Ok(projects);

        }

        [Authorize]
        [HttpGet("PendingProject")]
        public async Task<IActionResult> GetPendingProject()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var projects = await _projectService.GetPendingProject(userId);
            return Ok(projects);

        }

        [Authorize]
        [HttpGet("CompletedTask")]
        public async Task<IActionResult> GetCompletedTask()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var projects = await _projectService.GetCompletedTask(userId);
            return Ok(projects);

        }
    }
}
