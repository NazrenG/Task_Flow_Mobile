using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Task_Flow.Business.Cocrete;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Hubs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<CustomUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<ConnectionHub> _hubContext;
        private readonly IUserService _userService;
        private readonly MailService _emailService;
        private readonly SignInManager<CustomUser> _signInManager;

        public ProfileController(UserManager<CustomUser> userManager, IConfiguration configuration, IHubContext<ConnectionHub> hubContext, IUserService userService, SignInManager<CustomUser> signInManager, MailService emailService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _hubContext = hubContext;
            _userService = userService;
            _signInManager = signInManager;
            _emailService = emailService;
        }





        // POST api/<ProfileController>
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] string currentPassword,string newPassword)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Ok(new {Message= "User not authenticated." ,Code=-1});
            }
            var user=await _userService.GetUserById(userId);
            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, currentPassword);
            if (isPasswordCorrect)
            {
                await _userManager.ChangePasswordAsync(user,currentPassword, newPassword);
                return Ok(new {Message="Changes Saved!",Code=1});
            }

            return Ok(new {Message= "Error" ,Code=-1});

        }
       
        [HttpPost("CheckEmail")]
        public async Task<IActionResult> CheckEmail([FromBody] string value)
        {
         
            
            var isCheckUser = await _userService.CheckUsernameOrEmail(value);
            //mail
            if (isCheckUser)
            {
                return Ok(new { Message = "Verification code sent succesfully!", Code = _emailService.sendVerifyMail(value) });
            }

            return Ok(new { Message="Failed to send verification code!",Code=-1});

        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest("User not authenticated.");
            }
            await _hubContext.Clients.All.SendAsync("UserDisconnected", userId);

            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }


    }
}
