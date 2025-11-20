using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs.ProfessionalLink;
using Portfolio.Core.Models;
using Portfolio.WebApi.Extensions;

namespace Portfolio.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProfessionalLinkController : ControllerBase
    {
        private readonly IProfessionalLinkService _professionalLinkService;

        public ProfessionalLinkController(IProfessionalLinkService professionalLinkService)
        {
            _professionalLinkService = professionalLinkService;
        }

        [HttpPost("add")]
        [ProducesResponseType(typeof(List<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddProfessionalLinks(
            [FromBody] List<AddProfessionalLink> professionalLinks
        )
        {
            var userId = User.GetUserId()!.Value;

            if (professionalLinks is null)
                throw new ValidationException("Request body cannot be null.");

            var addedIds = await _professionalLinkService.AddProfessionalLinksAsync(
                userId,
                professionalLinks
            );
            return Created(string.Empty, addedIds);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ProfessionalLinkModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProfessionalLinkById([FromRoute] Guid id)
        {
            var userId = User.GetUserId()!.Value;
            var professionalLink = await _professionalLinkService.GetProfessionalLinkByIdAsync(
                id,
                userId
            );
            if (professionalLink == null)
                return NotFound();

            return Ok(professionalLink);
        }

        [HttpPatch("patch")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PatchProfessionalLink(
            [FromBody] PatchProfessionalLink patch
        )
        {
            var userId = User.GetUserId()!.Value;

            if (patch is null)
                throw new ValidationException("Request body cannot be null.");

            var updated = await _professionalLinkService.PatchProfessionalLinkAsync(userId, patch);
            if (!updated)
                return NoContent();

            return Ok(true);
        }

        [HttpDelete("delete")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteProfessionalLinks(
            [FromBody] List<Guid> professionalLinkIds
        )
        {
            var userId = User.GetUserId()!.Value;

            if (professionalLinkIds == null || !professionalLinkIds.Any())
                throw new ValidationException("ProfessionalLink IDs cannot be null or empty.");

            var deleted = await _professionalLinkService.DeleteProfessionalLinksAsync(
                userId,
                professionalLinkIds
            );
            if (!deleted)
                return BadRequest("Failed to delete the specified professionalLinks.");

            return Ok(true);
        }
    }
}
