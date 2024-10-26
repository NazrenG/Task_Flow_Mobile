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
    public class TeamMemberController : ControllerBase
    {

        private readonly ITeamMemberService _teamMemberService;

        public TeamMemberController(ITeamMemberService teamMemberService)
        {
            _teamMemberService = teamMemberService;
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
                    Role=t.Role,

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
                Role = item.Role,
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
                Role = value.Role,
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
    }
}
