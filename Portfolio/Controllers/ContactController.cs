using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs.Contact;
using Portfolio.Core.Models;
using Portfolio.WebApi.Extensions;

namespace Portfolio.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IProfessionalLinkService _contactService;

        public ContactController(IProfessionalLinkService contactService)
        {
            _contactService = contactService;
        }

        [HttpPost("add")]
        [ProducesResponseType(typeof(List<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddContacts([FromBody] List<AddProfessionalLink> contacts)
        {
            var userId = User.GetUserId()!.Value;

            if (contacts is null)
                throw new ValidationException("Request body cannot be null.");

            var addedIds = await _contactService.AddProfessionalLinksAsync(userId, contacts);
            return Created(string.Empty, addedIds);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ProfessionalLinkModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetContactById([FromRoute] Guid id)
        {
            var userId = User.GetUserId()!.Value;
            var contact = await _contactService.GetProfessionalLinkByIdAsync(id, userId);
            if (contact == null)
                return NotFound();

            return Ok(contact);
        }

        [HttpPatch("patch")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PatchContact([FromBody] PatchProfessionalLink patch)
        {
            var userId = User.GetUserId()!.Value;

            if (patch is null)
                throw new ValidationException("Request body cannot be null.");

            var updated = await _contactService.PatchProfessionalLinkAsync(userId, patch);
            if (!updated)
                return NoContent();

            return Ok(true);
        }

        [HttpDelete("delete")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteContacts([FromBody] List<Guid> contactIds)
        {
            var userId = User.GetUserId()!.Value;

            if (contactIds == null || !contactIds.Any())
                throw new ValidationException("Contact IDs cannot be null or empty.");

            var deleted = await _contactService.DeleteProfessionalLinksAsync(userId, contactIds);
            if (!deleted)
                return BadRequest("Failed to delete the specified contacts.");

            return Ok(true);
        }
    }
}
