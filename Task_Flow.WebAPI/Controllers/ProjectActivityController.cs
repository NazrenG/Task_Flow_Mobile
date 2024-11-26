using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Hubs;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectActivityController : ControllerBase
    {
        private readonly IProjectActivityService projectActivityService;
        private readonly IUserService userService;
        private readonly IProjectService projectService;
        private readonly IHubContext<ConnectionHub> hubContext;

        public ProjectActivityController(IProjectActivityService projectActivityService,IUserService userService,IProjectService projectService,IHubContext<ConnectionHub> hubContext )
        {
            this.projectActivityService = projectActivityService;
            this.userService = userService;
            this.projectService = projectService;
            this.hubContext = hubContext;
        }

      
        [Authorize]
        [HttpGet("TeamMemberActivities")]
        public async Task<IActionResult> GetTeamActivities() {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user=await userService.GetUserById(userId);  
            var listt = await projectActivityService.GetAll();
            var list = listt.Where(p=>p.Project.CreatedById==userId).Select(p => new
            {
                Username =user.UserName==p.User.UserName? "You":  $"{ p.User.Firstname} {p.User.Lastname}",
                ProjectName = p.Project.Title,
                CreateDate = p.CreateTime,
                Text = p.Text,
                Path = p.User.Image,

            });
            //foreach (var item in projects)
            //{
            //    var activities = await projectActivityService.GetAllByProjectId(item.Id);
            //    list.AddRange(activities);
            //}
            //var activity = list.Where(u => u.UserId != userId).ToList();
            //var dtoList=new List<ProjectActivityDto>();

            //foreach (var item in activity)
            //{
            //    var username = await userService.GetUserById(item.UserId);
            //    var projectname=await projectService.GetProjectById(item.ProjectId);
            //    var dto = new ProjectActivityDto
            //    {
            //        Username = username.UserName,
            //        ProjectName=projectname.Title,
            //        CreateDate=item.CreateTime,
            //        Text=item.Text
            //    };

            //    dtoList.Add(dto);
            //}

            return Ok(list);


        }

        [Authorize]
        [HttpGet("TeamMemberActivities/{projectId}")]
        public async Task<IActionResult> GetTeamActivitiesForProjectId(int projectId)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await userService.GetUserById(userId);
            var listt = await projectActivityService.GetAllByProjectId(projectId);
            var list = listt.Select(p => new
            {
                Username = user.UserName == p.User.UserName ? "You" : $"{p.User.Firstname} {p.User.Lastname}",
                ProjectName = p.Project.Title,
                CreateDate = p.CreateTime,
                Text = p.Text,
                Path=p.User.Image,

            });
            return Ok(list);
        }

            [Authorize]
        [HttpPost("AddTeamMemberActivities")]///sevgi
        public async Task<IActionResult> AddTeamActivities([FromBody] ProjectActivityDto dto)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await userService.GetUserById(userId);

            var project = new ProjectActivity
            {
                Text = dto.Text,
                UserId = userId,
                CreateTime = DateTime.UtcNow,

            };
            var data = await projectService.GetProjectById(dto.ProjectId);

            await projectActivityService.Add(project);
            await hubContext.Clients.User(data.CreatedById).SendAsync("RecieveRecentActivityUpdate");
            return Ok();


        }
    }
}
