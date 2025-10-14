using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;

namespace Portfolio.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService, IUserRepository userRepository) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly IUserRepository _userRepository = userRepository;

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

        [HttpGet("skills/{userId:guid}")]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSkillsByUserId(Guid userId)
        {
            var skills = await _userService.GetAllSkillsByUserId(userId);
            return Ok(skills);
        }

        [HttpGet("education/{userId:guid}")]
        [ProducesResponseType(typeof(List<EducationItem>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllEducationItemsByUserId(Guid userId)
        {
            var educationItems = await _userRepository.GetAllEducationItemsByUserId(userId);
            return Ok(educationItems);
        }
    }
}