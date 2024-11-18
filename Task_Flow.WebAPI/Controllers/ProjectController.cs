using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
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
        private readonly ITaskService _taskService;
        private readonly ITeamMemberService _teamMemberService;
        private readonly IProjectActivityService _projectActivity;
        public ProjectController(TaskFlowDbContext dbContext, IProjectService projectService, IUserService userService, ITeamMemberService teamMemberService, ITaskService taskService, IProjectActivityService productActivity)
        {
            _projectService = projectService;
            _userService = userService;
            _teamMemberService = teamMemberService;
            _context = dbContext;
            _taskService = taskService;
            _projectActivity = productActivity;
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

            var projectList = await _projectService.GetProjects(userId);
            var list = projectList.Select(p => new ExtendedProjectListDto
            {
                Id = p.Id,
                Title = p.Title,
                TotalTask = p.TaskForUsers.Count,
                CompletedTask = p.TaskForUsers.Count(t => t.Status == "done"),
                ParticipantsPath = p.TeamMembers
                            .Select(tm => tm.User.Image)
                            .ToList(),
                Color = p.Color,
            }).ToList();

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
        [Authorize]
        [HttpPost("ProjectWithTitle")]
        public async Task<IActionResult> GetProjectWithTitle([FromBody] string title)
        {
            try
            {
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not authorized." });
                }

                if (string.IsNullOrEmpty(title))
                {
                    return BadRequest(new { message = "Title is required." });
                }
                var project = await _projectService.GetProjectByName(userId, title);
                if (project == null)
                {
                    return NotFound(new { message = "Project not found." });
                }

                return Ok(project);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", details = ex.Message });
            }
        }





        // Pryektin icindeki tasklar CANBAN 

        [HttpGet("ProjectTaskCanban/{projectId}")]
        public async Task<IActionResult> Get(int projectId)
        {
            var item = await _projectService.GetProjectById(projectId);
            if (item == null)
            {
                return NotFound();
            }
            var projectTasks = await _taskService.GetByProjectId(projectId);
             
            var items = projectTasks.Select(p =>
            {
                return new CanbanTaskDto
                {
                    Id = p.Id,
                    CreatedById = p.CreatedById,
                    Description = p.Description,
                    Deadline = p.Deadline,
                    Priority = p.Priority,
                    Status = p.Status,
                    Title = p.Title,
                    StartDate = p.StartTime,
                    Color = p.Color,  
                    ParticipantPath = p.CreatedBy.Image,
                    ParticipantName = $"{p.CreatedBy.Firstname} {p.CreatedBy.Lastname}",
                    ParticipantEmail=p.CreatedBy.Email,
                };
            }).ToList();
            return Ok(items);


        }
        //canbanda tasklarin statusunu deyisdirmek 
        [Authorize]
        [HttpPut("UpdateTaskStatus/{id}")]
        public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] UpdateTaskStatusDto updateTaskStatusDto)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var task = await _taskService.GetTaskById(id);

            if (task == null)
                return NotFound("Task not found.");

            var project = await _projectService.GetProjectById(task.ProjectId);

            //proyekt sahibi deyise bilsin
            if (project.CreatedById != userId)
                return BadRequest("You do not have permission to update tasks in this project.");

            task.Status = updateTaskStatusDto.NewStatus;
            await _taskService.Update(task);

            return Ok("Task status updated successfully.");
        }

        [Authorize]
        [HttpGet("AllProjectsUserOwn")]
        public async Task<IActionResult> GetUserOwnProjects()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var list = await _projectService.GetProjects(userId);

            return Ok(list);

        }

        [Authorize]
        [HttpGet("UserAddedProjects")]
        public async Task<IActionResult> GetUserAddedProjects()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var list = await _teamMemberService.GetProjectListByUserIdAsync(userId);
            var projects = new List<Project>();
            foreach (var item in list)
            {
                projects.Add(await _projectService.GetProjectById(item.ProjectId));
            }
            return Ok(projects);

        }

        [HttpGet("ProjectTaskCount/{id}")]
        public async Task<IActionResult> GetProjectTaskCount(int id)
        {

            var item = await _context.Projects
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
                CreatedAt = DateTime.UtcNow,
                StartDate = value.StartDate,
                EndDate = value.EndDate,
                Description = value.Description,
                IsCompleted = value.IsCompleted,
                Title = value.Title,
            };
            await _projectService.Add(item);
            var projectId = await _projectService.GetProjectByName(userId, value.Title);
            await _projectActivity.Add(new ProjectActivity { UserId = userId, ProjectId = projectId.Id, Text = "created a new Project named: " + item.Title });
            return Ok(item);
        }

        // PUT api/<ProjectController>/5
        [Authorize]
        [HttpPut("ChangeTitle/{id}")]
        public async Task<IActionResult> PutTitle(int id, [FromBody] string value)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var item = await _projectService.GetProjectById(id);

            if (item == null)
            {
                return NotFound();
            }
            item.Title = value;
            await _projectService.Update(item);
            await _projectActivity.Add(new ProjectActivity { UserId = userId, ProjectId = id, Text = "Changed Project Title to: " + item.Title });
            return Ok();
        }

        [Authorize]
        [HttpPut("ChangeDescription/{id}")]
        public async Task<IActionResult> PutDescription(int id, [FromBody] string value)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var item = await _projectService.GetProjectById(id);

            if (item == null)
            {
                return NotFound();
            }
            item.Description = value;
            await _projectService.Update(item);
            await _projectActivity.Add(new ProjectActivity { UserId = userId, ProjectId = id, Text = "Changed Project Description" });
            return Ok();
        }
        [Authorize]
        [HttpPut("ChangeCompleted/{id}")]
        public async Task<IActionResult> PutCompleted(int id, [FromBody] bool value)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var item = await _projectService.GetProjectById(id);

            if (item == null)
            {
                return NotFound();
            }
            item.IsCompleted = value;
            await _projectService.Update(item);
            await _projectActivity.Add(new ProjectActivity { UserId = userId, ProjectId = id, Text = "Project Completed" });
            return Ok();
        }
        // DELETE api/<ProjectController>/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var item = await _projectService.GetProjectById(id);
            if (item == null)
            {
                return NotFound();
            }
            await _projectActivity.Add(new ProjectActivity { UserId = userId, ProjectId = id, Text = "Project (" + item.Title + ") Deleted!" });
            await _projectService.Delete(item);
            return Ok(item);
        }


        [Authorize]
        [HttpGet("OnGoingProject")]
        public async Task<IActionResult> GetOnGoingProject()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Invalid token or user not found.");
            }
            var projects = await _projectService.GetOnGoingProject(userId);
            return Ok(projects);

        }
        [Authorize]
        [HttpGet("PendingProject")]
        public async Task<IActionResult> GetPendingProject()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Invalid token or user not found.");
            }
            var projects = await _projectService.GetPendingProject(userId);
            return Ok(projects);

        }
        [Authorize]
        [HttpGet("CompletedProject")]
        public async Task<IActionResult> GetCompletedProject()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Invalid token or user not found.");
            }
            var projects = await _projectService.GetCompletedTask(userId);
            return Ok(projects);

        }

        [Authorize]
        [HttpGet("OnGoingProjectCount")]
        public async Task<IActionResult> GetOnGoingProjectCount()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Invalid token or user not found.");
            }
            var projects = await _projectService.GetOnGoingProject(userId);
            return Ok(projects.Count == null ? 0 : projects.Count);

        }

        [Authorize]
        [HttpGet("PendingProjectCount")]
        public async Task<IActionResult> GetPendingProjectCount()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Invalid token or user not found.");
            }
            var projects = await _projectService.GetPendingProject(userId);
            return Ok(projects.Count == null ? 0 : projects.Count);

        }

        [Authorize]
        [HttpGet("CompletedTaskCount")]
        public async Task<IActionResult> GetCompletedTaskCount()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Invalid token or user not found.");
            }
            var projects = await _projectService.GetCompletedTask(userId);
            return Ok(projects.Count == null ? 0 : projects.Count);

        }

        [Authorize]
        [HttpGet("ProjectNames")]
        public async Task<IActionResult> GetProjectNames()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Invalid token or user not found.");
            }
            var projects = await _projectService.GetProjects(userId);
            var names = new List<string>();
            foreach (var project in projects)
            {
                names.Add(project.Title);
            }
            return Ok(new { Names = names });
        }

        [Authorize]

        [HttpPost("TasksDependingMonths")]

        public async Task<IActionResult> GetTasksForChart([FromBody] string projectName)
        {

            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Invalid token or user not found.");
            }
            List<string> months = new List<string>();
            DateTime currentDate = DateTime.Now;
            var completedTasks = new List<int>();
            var onGoingTasks = new List<int>();
            var year = DateTime.UtcNow.Year;

            var project = await _projectService.GetProjectByName(userId, projectName);

            for (int i = 0; i < 6; i++)
            {
                int monthIndex = (currentDate.Month - i + 12) % 12;
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthIndex + 1);
                var data = await _taskService.GetTaskSummaryByMonthAsync(project.Id, monthIndex, year);
                months.Insert(0, monthName);
                completedTasks.Add(data[0]);
                onGoingTasks.Add(data[1]);
                if (monthName == "January") year--;
            }
            completedTasks.Reverse();
            onGoingTasks.Reverse();



            return Ok(new { Complated = completedTasks, OnGoing = onGoingTasks });
        }

        [Authorize]//userin istirak etdiyi layiheler=> dashboardda
        [HttpGet("ProjectInvolved")]
        public async Task<IActionResult> ProjectInvolved()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Invalid token or user not found.");
            }
            var participantProjects = await _teamMemberService.GetProjectListByUserIdAsync(userId);

            var projectList = new List<ExtendedProjectListDto>();

            foreach (var item in participantProjects)
            {
                var project = await _projectService.GetProjectById(item.ProjectId);
                projectList.Add(new ExtendedProjectListDto
                {
                    Id = project.Id,
                    Title = project.Title,
                    TotalTask = project.TaskForUsers!.Count(),
                    CompletedTask = project.TaskForUsers!.Count(t => t.Status == "done"),
                    ParticipantsPath = project.TeamMembers!
                        .Select(tm => tm.User.Image)!
                        .ToList(),
                    Color = project.Color,
                });
            }
            return Ok(projectList);


        }
    }
}
