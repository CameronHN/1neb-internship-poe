using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;

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

        [HttpGet()]
        [ProducesResponseType(typeof(GetUserDetailsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById([FromQuery] Guid id)
        {
            var user = await _userService.GetUserDetails(id);

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(AddUserDTO dto)
        {
            await _userService.AddUser(dto);
            return Ok();
        }
    }
}