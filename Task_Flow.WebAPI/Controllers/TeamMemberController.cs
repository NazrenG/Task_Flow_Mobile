using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using MimeKit;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Task_Flow.Business.Abstract;
using Task_Flow.Business.Cocrete;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using System.Runtime.InteropServices;
//using Task_Flow.WebAPI.Hubs;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamMemberController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly ITeamMemberService _teamMemberService;
        private readonly IProjectService _projectService;
        private readonly MailService mailService;
        //private readonly IHubContext<ConnectionHub> _hub;
        private readonly  IRequestNotificationService _requestNotificationService;
        private readonly INotificationSettingService _notificationSettingService;

        public TeamMemberController(ITeamMemberService teamMemberService, IUserService userService, IProjectService projectService, IRequestNotificationService requestNotificationService,  INotificationSettingService notificationSettingService,MailService mailServicse)
        {
            _userService = userService;
            _teamMemberService = teamMemberService;
            _projectService = projectService;
            this.mailService = mailServicse;
            _requestNotificationService = requestNotificationService;
            //_hub = hub;
            
            _notificationSettingService = notificationSettingService;
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

        [Authorize]
        [HttpPost("UpdateTeamMemberCollections")]///Sevgi
        public async Task<IActionResult> UpdateTeamMembersAsTeam([FromBody] TeamMemberCollectionDto dto)
        {
            if (dto == null || dto.Members == null || !dto.Members.Any())
            {
                return Ok(new { Message = "succesfuly.teammember.nomember" });
            }

            try
            {
                var senderId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var sender = await _userService.GetUserById(senderId);
                var list = await _teamMemberService.GetTaskMemberListById(dto.ProjectId);
                //await _teamMemberService.RemoveMembers(list);

                foreach (var member in dto.Members)
                {
                    var user = await _userService.GetUserById(member.Id);
                    var project = await _projectService.GetProjectNameById(dto.ProjectId);
                    //var teamMembers = await _teamMemberService.GetTaskMemberListById(dto.ProjectId);

                    //var isCheck = await _teamMemberService.GetTaskMemberById(user.Id);
                    if (user == null)
                    {
                        return NotFound(new { Message = "error.usernotFound" });
                    }
                    if (member.IsRequested==true)
                    {
                        var existingRequests = await _requestNotificationService.GetNotificationsBySenderId(senderId);
                        foreach (var req in existingRequests.Where(r => r.ReceiverId ==member.Id&& r.NotificationType == "ProjectRequest" && r.ProjectName == project))
                        {
                            await _requestNotificationService.Delete(req);
                        }
                        continue; 
                    }

                    if (member.IsSelected == true)
                    {
                        var existingMember = list.FirstOrDefault(tm => tm.UserId == user.Id);
                        if (existingMember != null)
                        {
                            await _teamMemberService.DeleteTeamMemberAsync(dto.ProjectId,existingMember.UserId);
                        }
                        continue; // skip sending new request
                    }

                    //var teamMember = new TeamMember
                    //{
                    //    ProjectId = dto.ProjectId,
                    //    UserId = user.Id,
                    //};
                    var request = new RequestNotification
                    {
                        IsAccepted = false,
                        ReceiverId = user.Id,
                        SenderId = senderId,
                        NotificationType = "ProjectRequest",
                        ProjectName = project,
                        SentDate = DateTime.UtcNow,
                        Text = "Hi, I am " + sender.Firstname + " " + sender.Lastname + ". I want to invite you to my project named: " + project,
                    };
                    await _requestNotificationService.Add(request);
                    //notification list
                    //await _hub.Clients.User(user.Id).SendAsync("RequestList2");
                    //await _hub.Clients.User(user.Id).SendAsync("RequestCount");
                    //await _hub.Clients.User(user.Id).SendAsync("RequestList");
                    //proyektde istirrak ucun egere icaze varsa mail gedir
                    var notificationSetting = await _notificationSettingService.GetNotificationSetting(user.Id);
                    if (notificationSetting?.NewTaskWithInProject==true)
                    {
                        mailService.SendEmail(user.Email, $"Hi,{user.Firstname} {user.Lastname}.You have a new task in the project named {project} ");

                    }
                  

                    //await _teamMemberService.Add(teamMember);
                }

                return Ok(new { Message = "succesfuly.teammember.addmember" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while adding team members.", Details = ex.Message });
            }
        }
        
        
        
        
        [Authorize]
        [HttpGet("GetUsersByProject/{projectId}")]//nezrin+sevgi
        public async Task<IActionResult> GetUsersByProject(int projectId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();
            var allUsers = await _userService.GetUsers();
            var allReq = await _requestNotificationService.GetAll();
            var projectName=await _projectService.GetProjectNameById(projectId);
            //var currentProjectReq = allReq.Select(p => p.ProjectName == projectName);
            var projectUsers = await _teamMemberService.GetTaskMemberListById(projectId);
            var selectedUserIds = projectUsers.Select(tm => tm.UserId).ToHashSet();
            var requestedUserIds = allReq.Where(p=>p.ProjectName==projectName).Select(ri=>ri.ReceiverId).ToHashSet();

           


            var list = allUsers.Where(u => u.Id != currentUserId).Select(u => new TeamUserDto
            {
                Id = u.Id,
                Name = $"{u.Firstname} {u.Lastname}",
                Email=u.Email,
               IsRequested=requestedUserIds.Contains(u.Id),
                IsSelected = selectedUserIds.Contains(u.Id) 
            })
            .OrderByDescending(u => u.IsSelected)
            .ThenByDescending(u => u.IsRequested)
            .ToList();

            return Ok(list);
        }
 

        [Authorize]
        [HttpPost("TeamMemberCollections")]///Sevgi
        public async Task<IActionResult> AddTeamMembersAsTeam([FromBody] TeamMemberCollectionDto dto)
        {
            var senderId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var sender = await _userService.GetUserById(senderId);
            if (dto == null || dto.Members == null || !dto.Members.Any() )
            {
                return Ok(new {Message= "succesfuly.teammember.nomember" });
            }

            try
            {

                foreach (var username in dto.Members)
                {
                    var user = await _userService.GetUserById(username.Id);
                    //var isCheck = await _teamMemberService.GetTaskMemberById(user.Id);
                    if (user == null)
                    {
                        return NotFound(new { Message = $"User '{username}' not found." });
                    }

                    //var teamMember = new TeamMember
                    //{
                    //    ProjectId = dto.ProjectId,
                    //    UserId = user.Id,
                    //};

                    //await _teamMemberService.Add(teamMember);
                    var project =await _projectService.GetProjectNameById(dto.ProjectId);

                    var request = new RequestNotification
                    {
                        IsAccepted = false,
                        ReceiverId = user.Id,
                        SenderId = senderId,
                        NotificationType = "ProjectRequest",
                        ProjectName =project,
                        SentDate = DateTime.UtcNow,
                        Text = "Hi, I am "+sender.Firstname+" "+sender.Lastname+ ". I want to invite you to my project named: " + project,
                    };
                    await _requestNotificationService.Add(request);
                    //await _hub.Clients.User(user.Id).SendAsync("RequestList");
                    //await _hub.Clients.User(user.Id).SendAsync("RequestList2");
                    //await _hub.Clients.User(user.Id).SendAsync("RequestCount");
                    mailService.SendEmail(user.Email, sender.Firstname + "" + sender.Lastname + " invited you to their project " + project);
                    ///signalr
                }

                return Ok(new { Message = "succesfuly.teammember.addmember" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while adding team members.", Details = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("TeammemberRemover")]
        public async Task<IActionResult> RemoveMember([FromBody] RemoveTeamMemberDto dto)
        {
           var requests= await _requestNotificationService.GetNotificationsByProjectName(dto.Title);
            var request = requests.FirstOrDefault(n => n.ReceiverId == dto.RecieverId);
            if (request == null)
            {
                return Ok(new { Code = 404 });
            }
                
            await _requestNotificationService.Delete(request);

            return Ok(new { Code = 200 });
        }

        [Authorize]
        [HttpDelete("MemberRemove")]
        public async Task<IActionResult> RemoveTM([FromBody]RemoveMemberDto dto)
        {
            var user=await _userService.GetOneUSerByUsername(dto.Username);
            var currentUserId=  HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await _teamMemberService.DeleteTeamMemberAsync(dto.ProjectId,user.Id);
            var project = await _projectService.GetProjectById(dto.ProjectId);
            mailService.SendEmail(user.Email, "You were removed from project " + project.Title + " at " + DateTime.UtcNow.ToShortDateString() + " by PM");
         //   await _hub.Clients.User(currentUserId).SendAsync("ReceiveProjectUpdate");
            return Ok();

        }


        [Authorize]
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetMembersByProjectId([FromRoute] int id)
        {
            var list = await _teamMemberService.GetTaskMemberListById(id);

            var dtoList = new List<ExtendedTeamMemberDto>();

            foreach (var item in list)
            {
                var user =await _userService.GetUserById(item.UserId);
                dtoList.Add(new ExtendedTeamMemberDto { Username = user.UserName, Firstname = user.Firstname,Lastname = user.Lastname, ImgPath = user.Image ,IsRequest=false, IsAccepted =true});

            }
            var project=await _projectService.GetProjectById(id);

            var requests = await _requestNotificationService.GetNotificationsByProjectName(project.Title);


            foreach (var item in requests)
            {
                var user = await _userService.GetUserById(item.ReceiverId);
                dtoList.Add(new ExtendedTeamMemberDto { Username = user.UserName, Firstname = user.Firstname, Lastname = user.Lastname, ImgPath = user.Image, IsRequest = true, IsAccepted=item.IsAccepted });

            }


            
                return Ok(new {List = dtoList});
            
            //return Ok(new {List = dtoList });
        }


    }
}
