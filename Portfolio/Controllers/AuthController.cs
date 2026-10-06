using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.Entities;
using Portfolio.WebApi.Extensions;
using Portfolio.WebApi.RateLimiting;

namespace Portfolio.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService _userService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUserService userService
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
        }

        [HttpPost("register")]
        [EnableRateLimiting(RateLimitPolicies.Auth)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.Phone,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Ok(new { Message = "User registered successfully", UserId = user.Id });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest(ModelState);
        }

        [HttpPost("login")]
        [EnableRateLimiting(RateLimitPolicies.Auth)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true
            );

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                return Ok(new { Message = "Login successful", UserId = user?.Id });
            }

            if (result.IsLockedOut)
            {
                return BadRequest("Account locked out");
            }

            return BadRequest("Invalid login attempt");
        }

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { Message = "Logged out successfully" });
        }

        [HttpPost("change-password")]
        [EnableRateLimiting(RateLimitPolicies.Auth)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null)
                return Unauthorized();

            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return NotFound("User not found");

            var result = await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword,
                model.NewPassword
            );

            if (result.Succeeded)
            {
                return Ok(new { Message = "Password updated successfully" });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest(ModelState);
        }

        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.GetUserId()!.Value;

            var user = await _userService.GetUserDetailsAsync(userId);

            return Ok(
                new
                {
                    id = userId,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    email = user.Email,
                    phoneNumber = user.PhoneNumber,
                }
            );
        }
    }

    public class RegisterDto
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }

    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
