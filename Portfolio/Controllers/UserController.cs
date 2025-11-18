using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs.User;

namespace Portfolio.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet()]
        [ProducesResponseType(typeof(GetUserDetailsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserById([FromQuery] Guid id)
        {
            var user = await _userService.GetUserDetailsAsync(id);

            return Ok(user);
        }
    }
}
