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
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        private readonly IUserService _userService;

        public ProjectController(IProjectService projectService, IUserService userService)
        {
            _projectService = projectService;
            _userService = userService;
        }

        [Authorize]
        [HttpGet("UserProjects")]
        public async Task<IActionResult> GetUserProjects()
        {

            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var id = int.Parse(userId);
            var item = await _userService.GetUserById(userId);
            if (userId == null)
            {
                return Unauthorized("Invalid token or user not found.");
            }

            var projects = await _projectService.GetProjects();
            var userProjects = projects
                .Where(p => p.CreatedById == userId)
                .Select(p => new ProjectDto
                {
                    CreatedById = p.CreatedById,
                    Description = p.Description,
                    IsCompleted = p.IsCompleted,
                    Title = p.Title,
                });

            return Ok(userProjects);
        }
        [Authorize]

        [HttpGet("UserProjectCount")]
        public async Task<IActionResult> GetUserProjectCount()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var id = int.Parse(userId);
            var item = await _userService.GetUserById(userId);
            if (userId == null)
            {
                return Unauthorized("Invalid token or user not found.");
            }
            var count = await _projectService.GetUserProjectCount(item.Id);

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
                CreatedById = item.CreatedById,
                Description = item.Description,
                IsCompleted = item.IsCompleted,
                Title = item.Title,
            };
            return Ok(project);

        }
        [HttpGet("ProjectTaskCount/{id}")]
        public async Task<IActionResult> GetProjectTaskCount(int id)
        {
            var item = await _projectService.GetProjectById(id);
            if (item == null)
            {
                return NotFound();
            }
            var count=item.TaskForUsers?.Count();
            
            return Ok(count);

        }

        // POST api/<ProjectController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProjectDto value)
        {

            var item = new Project
            {
                CreatedById = value.CreatedById,
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
    }
}
