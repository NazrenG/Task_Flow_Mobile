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
    public class ProjectActivityController : ControllerBase
    {
        private readonly IProjectActivityService projectActivityService;
        private readonly IUserService userService;
        private readonly IProjectService projectService;

        public ProjectActivityController(IProjectActivityService projectActivityService,IUserService userService,IProjectService projectService)
        {
            this.projectActivityService = projectActivityService;
            this.userService = userService;
            this.projectService = projectService;
        }

      
        [Authorize]
        [HttpGet("TeamMemberActivities")]
        public async Task<IActionResult> GetTeamActivities() {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user=await userService.GetUserById(userId);
            var list = new List<ProjectActivity>();
            var projects = await projectService.GetProjects(userId);
            foreach (var item in projects)
            {
                var activities = await projectActivityService.GetAllByProjectId(item.Id);
                list.AddRange(activities);
            }
            var activity = list.Where(u => u.UserId != userId).ToList();
            var dtoList=new List<ProjectActivityDto>();
            foreach (var item in activity)
            {
                var username = await userService.GetUserById(item.UserId);
                var projectname=await projectService.GetProjectById(item.ProjectId);
                var dto = new ProjectActivityDto
                {
                    Username = username.UserName,
                    ProjectName=projectname.Title,
                    CreateDate=item.CreateTime,
                    Text=item.Text
                };

                dtoList.Add(dto);
            }

            return Ok(new {List= dtoList });


        }
    }
}
