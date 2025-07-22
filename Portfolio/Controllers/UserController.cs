using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Contracts.Services;

namespace Portfolio.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try
            {
                var user = await _userService.GetUserDetails(id);

                return Ok(user);
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException)
                {
                    return NotFound(ex.Message);
                }
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }
    }
}