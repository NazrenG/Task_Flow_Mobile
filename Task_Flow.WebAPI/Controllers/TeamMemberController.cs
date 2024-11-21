using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamMemberController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly ITeamMemberService _teamMemberService;

        public TeamMemberController(ITeamMemberService teamMemberService,IUserService userService)
        {
            _teamMemberService = teamMemberService;
            _userService = userService;
        }

        [HttpGet("AllMember")]
        public async Task<IEnumerable<TeamMemberDto>> Get()
        {
           var items= await _teamMemberService.TeamMembers();
            var list = items.Select(t =>
            {
                return  new TeamMemberDto
                {
                    ProjectId=t.ProjectId,
                    UserId=t.UserId,

                };
            }); 
            return list;
        }
         
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMember(int id)
        {
         var item=await _teamMemberService.GetTaskMemberById(id);
            if (item == null)
            {
               return NotFound(item);
            }
            return Ok(new TeamMemberDto
            {
                ProjectId = item.ProjectId,
                UserId = item.UserId,
            });
        }

       
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TeamMemberDto value)
        {
            if (value == null)
            {
                return BadRequest();
            }
            var item = new TeamMember
            {
                ProjectId = value.ProjectId,
                UserId = value.UserId,
               
            };
           await _teamMemberService.Add(item);
            return Ok(item);
        }

      
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] string value)
        {
            var item = await _teamMemberService.GetTaskMemberById(id);
            if (item == null)
            {
                return NotFound();
            } 
            item.UserId = value; 
            await _teamMemberService.Update(item);
            return Ok();

        }

        //[Authorize]
        //[HttpGet("TeamMembersActivity")]
        //public async Task<IActionResult> GetTeamMemberAvtivity()
        //{
        //    var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    var data = _teamMemberService.GetTaskMemberById();
        //}


        // DELETE api/<TeamMemberController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _teamMemberService.GetTaskMemberById(id);
            if (item == null)
            {
                return NotFound();
            }
            await _teamMemberService.Delete(item);
            return Ok();
        }

        [HttpPost("TeamMemberCollections")]///Sevgi
        public async Task<IActionResult> AddTeamMembersAsTeam([FromBody] TeamMemberCollectionDto dto)
        {
            if (dto == null || dto.Members == null || !dto.Members.Any() )
            {
                return BadRequest(new { Message = "Invalid input data." });
            }

            try
            {
                foreach (var username in dto.Members)
                {
                    var user = await _userService.GetOneUSerByUsername(username);
                    if (user == null)
                    {
                        return NotFound(new { Message = $"User '{username}' not found." });
                    }

                    var teamMember = new TeamMember
                    {
                        ProjectId = dto.ProjectId,
                        UserId = user.Id,
                    };

                    await _teamMemberService.Add(teamMember);
                }

                return Ok(new { Message = "Team members added successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while adding team members.", Details = ex.Message });
            }
        }
        [HttpGet("GetUsersByProject/{projectId}")]//nezrin
        public async Task<IActionResult> GetUsersByProject(int projectId)
        {
            var users = await _teamMemberService.GetTaskMemberListById(projectId);
            var list=users.Select(tm=> new TeamUserDto
            {   Id = tm.UserId,
                    Name =$"{tm.User.UserName}" ,

            }).ToList();
         
            return Ok(list);
        }
 


    }
}
