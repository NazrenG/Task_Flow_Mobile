using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.DataAccess.Concrete;
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
        private readonly SignInManager<CustomUser> _signInManager;
        private readonly Dictionary<string, string> _verificationCodes = new();

        public ProfileController(UserManager<CustomUser> userManager, IConfiguration configuration, IHubContext<ConnectionHub> hubContext, IUserService userService, SignInManager<CustomUser> signInManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _hubContext = hubContext;
            _userService = userService;
            _signInManager = signInManager;
        }





        // POST api/<ProfileController>
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] string currentPassword, string newPassword)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest("User not authenticated.");
            }
            var user = await _userService.GetUserById(userId);
            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, currentPassword);
            if (isPasswordCorrect)
            {
                await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
                return Ok();
            }

            return BadRequest("Error");

        }
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto value)
        {
            //maili gonderirsen eger dogrudursa true qaytarir
            var isCheckUser = await _userService.CheckUsernameOrEmail(value.NameOrEmail);
            if (isCheckUser) return Ok("find email succesful");

            var code = new Random().Next(1000, 9999).ToString();
            _verificationCodes[value.NameOrEmail] = code;

            // Mail göndermek hissesini yaz,code -u ora gonder

return Ok("Verification code sent"); 

        }
        [HttpPost("verify-code")]//4 reqemli kod duzdurse
       
        public IActionResult VerifyCode(VerifyCodeDto model)
        {
            if (_verificationCodes.TryGetValue(model.Email, out var code) && code == model.Code)
            {
                return Ok("Code verified");
            }
            return BadRequest("Invalid code");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return NotFound("User not found");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (result.Succeeded)
            {
                _verificationCodes.Remove(model.Email);
                return Ok("Password reset successful");
            }
            return BadRequest(result.Errors);
        }

        [Authorize]
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest("User not authenticated.");
            }
            await _hubContext.Clients.All.SendAsync("UserDisconnected", userId);
            var user = await _userService.GetUserById(userId);
            user.IsOnline = false;
            await _userService.Update(user);
            await _signInManager.SignOutAsync();
            return Ok("logout succesfuly");
        }

        [Authorize]
        [HttpPost("EditedProfile")]
        public async Task<IActionResult> EditProfile(UserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data provided.");
            }

            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest("User not authenticated.");
            }

            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var temp = dto.Fullname?.Split(" ");
            user.Firstname = temp != null && temp.Length > 0 ? temp[0] : user.Firstname;
            user.Lastname = temp != null && temp.Length > 1 ? temp[1] : user.Lastname;

            user.Birthday = dto.Birthday;
            user.Email = dto.Email;
            user.Country = dto.Country;
            user.PhoneNumber = dto.Phone;
            user.Occupation = dto.Occupation;
            user.Gender = dto.Gender;
            user.Image = dto.Image;
            await _userService.Update(user);
            return Ok("Edit successful");
        }

    }

}
